using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Reenbit.Ordering.Domain.Entities;
using Reenbit.Ordering.Public.Contracts.Responses;
using MediatR;
using Reenbit.Ordering.Public.Events;
using OrderItem = Reenbit.Ordering.Domain.EntitiesNodes.OrderItem;

namespace Reenbit.Ordering.Cqrs.Commands;

public record PlaceOrderCommand(Guid CustomerId, List<OrderItem> Items): IRequest<OrderResponse>;

internal class PlaceOrderCommandHandler(DaprClient daprClient) : IRequestHandler<PlaceOrderCommand, OrderResponse>
{
    public async Task<OrderResponse> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order();
        order.Create(request.CustomerId, request.Items);

        // persist order to the database

        // publish message to Service Bus
        OrderPlaced @event = CreateOrderPlacedEvent(order);
        
        await daprClient.PublishEventAsync(pubsubName: "pubsub", topicName: "order-placed", @event, cancellationToken);

        return new OrderResponse
        {
            CustomerId = request.CustomerId,
            OrderId = order.OrderId,
            OrderItems = request.Items
        };
    }

    private static OrderPlaced CreateOrderPlacedEvent(Order order)
    {
        return new OrderPlaced(order.OrderId,
            order.Items
                .Select(x => new Reenbit.Ordering.Public.Events.OrderItem { Count = x.Count, ProductId = x.ProductId })
                .ToList());
    }
}