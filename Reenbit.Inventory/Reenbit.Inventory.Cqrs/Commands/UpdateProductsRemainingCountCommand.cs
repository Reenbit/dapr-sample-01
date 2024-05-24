using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using MediatR;
using Reenbit.Inventory.Domain.Entities;
using Reenbit.Ordering.Public.Events;

namespace Reenbit.Inventory.Cqrs.Commands;

public record UpdateProductsRemainingCountCommand(List<OrderItem> OrderedItems) : IRequest;

internal class UpdateProductsRemainingCountCommandHandler(DaprClient daprClient): IRequestHandler<UpdateProductsRemainingCountCommand>
{
    private const string _stateStore = "statestore";
    
    public async Task<Unit> Handle(UpdateProductsRemainingCountCommand request, CancellationToken cancellationToken)
    {
        var products = await daprClient.GetStateAsync<List<Product>>(_stateStore, "products", cancellationToken: cancellationToken);

        foreach (var orderedItem in request.OrderedItems)
        {
            var product = products.FirstOrDefault(x => x.ProductId == orderedItem.ProductId);

            product?.UpdateRemainingAmount(product.RemainingCount - orderedItem.Count);
        }
        
        await daprClient.SaveStateAsync(_stateStore, "products", products, cancellationToken: cancellationToken);

        return Unit.Value;
    }
}