using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Security.Authentication;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using System.Text.Json;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace AzureCosmosDbOrderItemReserver;

public static class ReserveToCosmosDb
{
    [FunctionName("ReserveToCosmosDb")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log, 
        ExecutionContext executionContext)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

        var config = new ConfigurationBuilder()
            .SetBasePath(executionContext.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        MongoClientSettings settings = MongoClientSettings.FromUrl(
          new MongoUrl(config.GetValue<string>("CosmosDBConnection")));
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        var mongoClient = new MongoClient(settings);
        var order = BsonSerializer.Deserialize<BsonDocument>(requestBody);
        mongoClient.GetDatabase("EShop").GetCollection<BsonDocument>("Orders").InsertOne(order);

        return new OkObjectResult($"Order [{order.GetValue("Id")}] has been saved to Orders collection.");
    }
}
