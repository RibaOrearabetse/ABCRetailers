# ABCRetailers

🌩️ CLDV6212 – Cloud Development B

Welcome to the ABC Retailers Cloud Development Project — created as part of the CLDV6212 (Cloud Development B) module.

This repository contains all coursework materials, implementation files, and documentation for event-driven architecture development using Azure Event Hubs and Azure Service Bus.

📁 Project Structure
ABCRetailers/
│
├── .git/                         → Git repository folder (hidden)
├── ABCRetailers/                 → Main project source code (Visual Studio solution)
│   ├── Controllers/
│   ├── Models/
│   ├── Views/
│   ├── appsettings.json
│   └── Program.cs
└── README.md                     → Project overview and setup guide (this file)

⚡ Getting Started
Step 1: Open the Project

Navigate to the ABCRetailers folder inside this repo.

Open the solution (.sln) file in Visual Studio 2022 or VS Code.

Step 2: Install Prerequisites

Make sure the following are installed:

✅ .NET SDK (latest stable version)

✅ Azure Functions Core Tools (if you’re running the Functions app locally)

✅ Node.js (only if your project has frontend components)

🔧 Azure Configuration
Required Azure Services

This project uses Azure for event-driven communication and data flow.
You’ll need to set up the following resources in your Azure subscription:

Event Hubs Namespace + Event Hub – for real-time streaming and analytics

Service Bus Namespace + Queues/Topics – for reliable message delivery

Local Setup

In Visual Studio, configure your connection strings:

For Azure Functions, update the local.settings.json file:

{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "",
    "ServiceBusConnection": "",
    "EventHubsConnection": ""
  }
}


For Web Apps/APIs, use environment variables or the appsettings.json file to store Azure keys and endpoints.

▶️ Running the Application

To run the main ABC Retailers project:

dotnet run


To start the Azure Functions app (if included):

func start


Make sure your configuration files are updated before running the apps locally.

📘 Documentation

See your documentation file (EventHubs_EventBus_Documentation.md) for:

The difference between Event Hubs and Service Bus

Real-world workflows for inventory updates, order notifications, and fraud detection

Recommended architecture choices for the ABC Retail case study

Official Microsoft Learn references

⚠️ Notes

Don’t upload or commit real Azure keys or secrets to GitHub.

Keep environment variables private using local.settings.json or Azure Key Vault.

Follow your module submission instructions for file packaging and upload.

📚 References

Bootstrap (2025) Bootstrap documentation. getbootstrap.com. Available at: https://getbootstrap.com/docs/
 (Accessed: 6 October 2025).

Microsoft (2025) ASP.NET Core MVC overview. Microsoft Learn. Available at: https://learn.microsoft.com/aspnet/core/mvc/overview
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Blob Storage documentation. Microsoft Learn. Available at: https://learn.microsoft.com/azure/storage/blobs/
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Event Hubs documentation. Microsoft Learn. Available at: https://learn.microsoft.com/azure/event-hubs/
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Files documentation. Microsoft Learn. Available at: https://learn.microsoft.com/azure/storage/files/
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Functions blob storage trigger. Microsoft Learn. Available at: https://learn.microsoft.com/azure/azure-functions/functions-bindings-storage-blob-trigger
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Functions queue storage trigger. Microsoft Learn. Available at: https://learn.microsoft.com/azure/azure-functions/functions-bindings-storage-queue-trigger
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Functions triggers and bindings concepts. Microsoft Learn. Available at: https://learn.microsoft.com/azure/azure-functions/functions-triggers-bindings
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Queue Storage documentation. Microsoft Learn. Available at: https://learn.microsoft.com/azure/storage/queues/
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Storage Explorer. Microsoft Learn. Available at: https://learn.microsoft.com/azure/storage/common/storage-explorer
 (Accessed: 6 October 2025).

Microsoft (2025) Azure Table Storage and Azure Tables client library. Microsoft Learn. Available at: https://learn.microsoft.com/azure/storage/tables/table-storage-overview
 (Accessed: 6 October 2025).

Microsoft (2025) OneDrive Files On-Demand in Windows. Microsoft Support. Available at: https://support.microsoft.com/office/onedrive-files-on-demand-in-windows-0e6860d3-d9f3-4971-b321-7092438fb38e
 (Accessed: 6 October 2025).

Microsoft (2025) Queues, topics, and subscriptions in Azure Service Bus. Microsoft Learn. Available at: https://learn.microsoft.com/azure/service-bus-messaging/service-bus-queues-topics-subscriptions
 (Accessed: 6 October 2025).

Microsoft (2025) Upload files in ASP.NET Core. Microsoft Learn. Available at: https://learn.microsoft.com/aspnet/core/mvc/models/file-uploads
 (Accessed: 6 October 2025).

Microsoft (2025) What is Azure Service Bus? Microsoft Learn. Available at: https://learn.microsoft.com/azure/service-bus-messaging/service-bus-messaging-overview
 (Accessed: 6 October 2025).
