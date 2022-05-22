using System;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.EventGrid;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.Extensions.Logging;

namespace Microsoft.eShopWeb.ApplicationCore.Services;
public class EventGridOrderItemsReserverService
{
    private readonly string _eventGridUri;
    private readonly string _eventGridAccessKey;
    private readonly ILogger<EventGridOrderItemsReserverService> _logger;

    public EventGridOrderItemsReserverService(
        string eventGridUri, 
        string eventGridAccessKey, ILogger<EventGridOrderItemsReserverService> logger)
    {
        _eventGridUri = eventGridUri;
        _eventGridAccessKey = eventGridAccessKey;
        _logger = logger;
    }

    public async Task Reserve(Order order)
    {
        EventGridPublisherClient client = new EventGridPublisherClient(
            new Uri(_eventGridUri), 
            new AzureKeyCredential(_eventGridAccessKey));
        EventGridEvent gridEvent = new EventGridEvent("Order", "OrderCreated", "1.0", order);
        var response = await client.SendEventAsync(gridEvent);
        _logger.LogInformation(response.Content.ToString());
    }
}
