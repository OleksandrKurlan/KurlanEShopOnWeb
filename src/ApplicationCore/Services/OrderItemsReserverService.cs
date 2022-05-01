using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderItemsReserverService
{
    private readonly string _connectionString;
    private readonly ILogger<OrderItemsReserverService> _logger;

    public OrderItemsReserverService(string connectionString, ILogger<OrderItemsReserverService> logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public async Task Reserve(Order order)
    {
        var client = new HttpClient();
        var content = new StringContent(JsonSerializer.Serialize(order), Encoding.UTF8, "application/json");
        var response = await client.PostAsync(_connectionString, content);
        _logger.LogInformation(await response.Content.ReadAsStringAsync());
    }
}
