using System;
using System.Collections.Generic;

namespace Reenbit.Ordering.Public.Events;

public class OrderPlaced
{
    public OrderPlaced(Guid orderId, List<OrderItem> items)
    {
        OrderId = orderId;
        Items = items;
    }
    
    public DateTime When { get; } = DateTime.UtcNow;
    
    public Guid OrderId { get; set; }

    public List<OrderItem> Items { get; set; }
}


public class OrderItem
{
    public Guid ProductId { get; set; }
    
    public int Count { get; set; }
}
