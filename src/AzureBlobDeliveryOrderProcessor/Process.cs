using System.IO;
using System.Security.Authentication;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AzureBlobDeliveryOrderProcessor;

public class Process
{
    [FunctionName("Process")]
    public static async Task RunAsync(
        [BlobTrigger("orders/{name}", 
        Connection = "AzureBlobConnection")]Stream myBlob, 
        string name, 
        ILogger log, 
        ExecutionContext executionContext)
    {
        log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

        string blob = await new StreamReader(myBlob).ReadToEndAsync();

        var config = new ConfigurationBuilder()
            .SetBasePath(executionContext.FunctionAppDirectory)
            .AddJsonFile("local.settings.json", true, true)
            .AddEnvironmentVariables()
            .Build();

        MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(config.GetValue<string>("CosmosDbConnection")));
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        var mongoClient = new MongoClient(settings);
        var order = BsonSerializer.Deserialize<BsonDocument>(blob);
        mongoClient.GetDatabase(config.GetValue<string>("DatabaseName"))
            .GetCollection<BsonDocument>(config.GetValue<string>("CollectionName"))
            .InsertOne(order);
    }
}
