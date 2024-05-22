using System;
using System.Collections.Generic;
using Reenbit.Ordering.Domain.EntitiesNodes;

namespace Reenbit.Ordering.Public.Contracts.Requests;

public class PlaceOrderRequest
{
    public Guid CustomerId { get; set; }
    
    public List<OrderItem> Items { get; set; }
}