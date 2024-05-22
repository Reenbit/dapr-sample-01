using System;
using System.Collections.Generic;
using Reenbit.Ordering.Domain.EntitiesNodes;

namespace Reenbit.Ordering.Public.Contracts.Responses;

public class OrderResponse
{
    public Guid OrderId { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public List<OrderItem> OrderItems { get; set; }
}