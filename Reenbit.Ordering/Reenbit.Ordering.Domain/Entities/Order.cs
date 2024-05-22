using System;
using System.Collections.Generic;

namespace Reenbit.Ordering.Domain.Entities;

public class Order
{
    public string Id => GetDocumentId(OrderId);

    public static string GetDocumentId(Guid orderId)
        => "orders/" + orderId;
    
    public Guid OrderId { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public List<EntitiesNodes.OrderItem> Items { get; set; }
    
    public DateTime Created { get; set; }
    
    public DateTime? Updated { get; set; }

    public void Create(Guid customerId, List<EntitiesNodes.OrderItem> items)
    {
        OrderId = Guid.NewGuid();
        Created = DateTime.UtcNow;
        CustomerId = customerId;
        Items = items;
    }
}