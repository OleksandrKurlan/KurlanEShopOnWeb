using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace OrderItemReserver;

public static class Reserve
{
    [FunctionName("Reserve")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log, ExecutionContext executionContext)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var config = new ConfigurationBuilder()
                        .SetBasePath(executionContext.FunctionAppDirectory)
                        .AddJsonFile("local.settings.json", true, true)
                        .AddEnvironmentVariables().Build();

        CloudStorageAccount storageAccount = CloudStorageAccount.Parse(config["BlobConnectionString"]);
        CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
        CloudBlobContainer container = blobClient.GetContainerReference("order-items");
        string fileName = Guid.NewGuid().ToString() + ".json";
        CloudBlockBlob blob = container.GetBlockBlobReference(fileName);
        await blob.UploadTextAsync(requestBody);

        return new OkObjectResult($"Order has been saved to {fileName} file.");
    }
}
