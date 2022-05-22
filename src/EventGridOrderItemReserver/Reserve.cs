// Default URL for triggering event grid function in the local environment.
// http://localhost:7071/runtime/webhooks/EventGrid?functionName={functionname}
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using Azure.Messaging.EventGrid;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EventGridOrderItemReserver;

public static class Reserve
{
    [FunctionName("Reverve")]
    public static async Task Run(
        [EventGridTrigger]EventGridEvent eventGridEvent, 
        ILogger log, 
        ExecutionContext executionContext)
    {
        string request = eventGridEvent.Data.ToString();

        log.LogInformation(request);

        var config = new ConfigurationBuilder()
                        .SetBasePath(executionContext.FunctionAppDirectory)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddEnvironmentVariables().Build();

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["BlobConnectionString"]);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference("order-items");
        string fileName = Guid.NewGuid().ToString() + ".json";
        CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
        await blob.UploadTextAsync(request);
    }
}
