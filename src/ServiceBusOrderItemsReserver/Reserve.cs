using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ServiceBusOrderItemsReserver;

public static class Reserve
{
    [Singleton]
    [FunctionName("Reserve")]
    public static async Task RunAsync([ServiceBusTrigger("order", Connection = "ServiceBusConnection")]string myQueueItem, ILogger log)
    {
        log.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");
        CloudStorageAccount storageAccount = CloudStorageAccount.Parse("DefaultEndpointsProtocol=https;AccountName=finalassignmentorderblob;AccountKey=iL7jmRHHuH1fhfprINk9p+3deH6j38ucisu6ukevf8tamkAEiJG1O0mtRjgmSSnNvZSRlzgvONC9+AStfVpl5A==;EndpointSuffix=core.windows.net");
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference("orders");
        string fileName = Guid.NewGuid().ToString() + ".json";
        CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
        await blob.UploadTextAsync(myQueueItem);
    }
}
