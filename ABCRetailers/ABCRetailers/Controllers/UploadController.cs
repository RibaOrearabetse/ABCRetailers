using ABCRetailers.Models;
using ABCRetailers.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ABCRetailers.Controllers
{
    public class UploadController : Controller
    {
        private readonly IAzureStorageService _storageService;
        private readonly ILogger<UploadController> _logger;

        public UploadController(IAzureStorageService storageService, ILogger<UploadController> logger)
        {
            _storageService = storageService;
            _logger = logger;
        }

        // GET: Upload
        [HttpGet]
        public async Task<IActionResult> Index(string? orderId = null, string? customerName = null)
        {
            var vm = new FileUploadModel
            {
                OrderId = orderId,
                CustomerName = customerName
            };
            var orders = await _storageService.GetAllEntitiesAsync<Order>();
            var orderItems = orders
                .Select(o => new SelectListItem
                {
                    Value = o.RowKey,
                    Text = $"{o.OrderId} - {o.Username} - {o.ProductName}"
                })
                .ToList();
            ViewBag.Orders = new SelectList(orderItems, "Value", "Text", orderId);

            var customers = await _storageService.GetAllEntitiesAsync<CustomerDetails>();
            var customerItems = customers
                .Select(c => new SelectListItem
                {
                    Value = string.IsNullOrWhiteSpace(c.Username) ? $"{c.Name} {c.Surname}" : c.Username,
                    Text = $"{c.Name} {c.Surname} - {c.Username} - {c.Email}"
                })
                .ToList();
            ViewBag.Customers = new SelectList(customerItems, "Value", "Text", customerName);

            // Get uploaded files
            var uploadedFiles = await GetUploadedFilesAsync();
            ViewBag.UploadedFiles = uploadedFiles;

            return View(vm);
        }

        // POST: Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FileUploadModel model)
        {
            if (model.ProofOfPayment == null || model.ProofOfPayment.Length == 0)
            {
                ModelState.AddModelError(nameof(model.ProofOfPayment), "Please select a file to upload.");
            }

            if (!ModelState.IsValid)
            {
                var orders = await _storageService.GetAllEntitiesAsync<Order>();
                var orderItems = orders
                    .Select(o => new SelectListItem
                    {
                        Value = o.RowKey,
                        Text = $"{o.OrderId} - {o.Username} - {o.ProductName}"
                    })
                    .ToList();
                ViewBag.Orders = new SelectList(orderItems, "Value", "Text", model.OrderId);

                var customers = await _storageService.GetAllEntitiesAsync<CustomerDetails>();
                var customerItems = customers
                    .Select(c => new SelectListItem
                    {
                        Value = string.IsNullOrWhiteSpace(c.Username) ? $"{c.Name} {c.Surname}" : c.Username,
                        Text = $"{c.Name} {c.Surname} - {c.Username} - {c.Email}"
                    })
                    .ToList();
                ViewBag.Customers = new SelectList(customerItems, "Value", "Text", model.CustomerName);

                // Get uploaded files
                var uploadedFiles = await GetUploadedFilesAsync();
                ViewBag.UploadedFiles = uploadedFiles;

                return View(model);
            }

            try
            {
                // Upload the proof to the private container created during initialization
                var blobFileName = await _storageService.UploadFileAsync(model.ProofOfPayment, "payment-proofs");

                // Optionally notify via queue with context
                var message = $"{{\"orderId\":\"{model.OrderId}\",\"customerName\":\"{model.CustomerName}\",\"file\":\"{blobFileName}\"}}";
                await _storageService.SendMessageAsync("order-notifications", message);

                // On payment upload: set order to Processing and deduct inventory
                if (!string.IsNullOrWhiteSpace(model.OrderId))
                {
                    var order = await _storageService.GetEntityAsync<Order>("Order", model.OrderId);
                    if (order != null)
                    {
                        order.Status = "Processing";
                        await _storageService.UpdateEntityAsync(order);

                        var product = await _storageService.GetEntityAsync<Product>("Product", order.ProductId);
                        if (product != null)
                        {
                            var previous = product.StockAvailable;
                            product.StockAvailable = Math.Max(0, product.StockAvailable - order.Quantity);
                            await _storageService.UpdateEntityAsync(product);

                            var stockMsg = System.Text.Json.JsonSerializer.Serialize(new
                            {
                                type = "stock-update",
                                productId = product.ProductId,
                                productName = product.Name,
                                change = -order.Quantity,
                                previous,
                                current = product.StockAvailable,
                                reason = "payment-proof-uploaded",
                                orderId = order.OrderId
                            });
                            await _storageService.SendMessageAsync("stock-updates", stockMsg);
                        }
                    }
                }

                TempData["Success"] = "Proof of payment uploaded successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading proof of payment");
                ModelState.AddModelError(string.Empty, $"Upload failed: {ex.Message}");
                return View(model);
            }
        }

        private async Task<List<UploadedFileInfo>> GetUploadedFilesAsync()
        {
            try
            {
                var blobFiles = await _storageService.ListBlobsAsync("payment-proofs");
                var files = new List<UploadedFileInfo>();

                foreach (var blob in blobFiles)
                {
                    files.Add(new UploadedFileInfo
                    {
                        FileName = blob.Name,
                        FileSize = FormatFileSize(blob.Size),
                        UploadDate = blob.LastModified.DateTime,
                        FileType = blob.ContentType,
                        OrderId = ExtractOrderIdFromFileName(blob.Name),
                        CustomerName = ExtractCustomerFromFileName(blob.Name)
                    });
                }

                return files;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting uploaded files");
                return new List<UploadedFileInfo>();
            }
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private string ExtractOrderIdFromFileName(string fileName)
        {
            // Extract order ID from filename if it follows a pattern
            // This is a simple implementation - adjust based on your naming convention
            var parts = fileName.Split('_');
            if (parts.Length > 1)
            {
                return parts[1]; // Assuming format: timestamp_orderId_filename
            }
            return string.Empty;
        }

        private string ExtractCustomerFromFileName(string fileName)
        {
            // Extract customer info from filename if available
            // This is a simple implementation - adjust based on your naming convention
            var parts = fileName.Split('_');
            if (parts.Length > 2)
            {
                return parts[2]; // Assuming format: timestamp_orderId_customer_filename
            }
            return string.Empty;
        }
    }

    public class UploadedFileInfo
    {
        public string FileName { get; set; } = string.Empty;
        public string FileSize { get; set; } = string.Empty;
        public DateTime UploadDate { get; set; }
        public string OrderId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}



