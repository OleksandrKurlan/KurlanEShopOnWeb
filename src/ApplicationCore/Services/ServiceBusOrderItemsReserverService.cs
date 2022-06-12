using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.ApplicationCore.Services;
public class ServiceBusOrderItemsReserverService
{
    private readonly string _connectionString;
    private readonly string _queueName;

    private const int NumOfMessages = 3;

    private readonly ILogger<ServiceBusOrderItemsReserverService> _logger;

    public ServiceBusOrderItemsReserverService(
        string connectionString, 
        string queueName,
        ILogger<ServiceBusOrderItemsReserverService> logger)
    {
        _connectionString = connectionString;
        _queueName = queueName;
        _logger = logger;
    }

    public async Task Reserve(Order order)
    {
        await using var client = new ServiceBusClient(_connectionString);
        await using var sender = client.CreateSender(_queueName);
        var message = new ServiceBusMessage(JsonSerializer.Serialize(order));
        await sender.SendMessagesAsync(new List<ServiceBusMessage> { message });
        _logger.LogInformation($"Message with MessageId:{message.MessageId} was sent.");
    }
}
