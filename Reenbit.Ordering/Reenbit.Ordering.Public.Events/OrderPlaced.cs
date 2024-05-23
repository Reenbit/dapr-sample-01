using System;
using System.Collections.Generic;

namespace Reenbit.Ordering.Public.Events;

public class OrderPlaced(Guid orderId, List<OrderItem> items)
{
    public DateTime When { get; } = DateTime.UtcNow;
    
    public Guid OrderId { get; set; } = orderId;

    public List<OrderItem> Items { get; set; } = items;
}

public class OrderItem
{
    public Guid ProductId { get; set; }
    
    public int Count { get; set; }
}
