using System;

namespace Reenbit.Ordering.Domain.EntitiesNodes;

public class OrderItem
{
    public Guid ProductId { get; set; }
    
    public int Count { get; set; }
}