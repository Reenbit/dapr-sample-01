using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Reenbit.Inventory.Domain.Entities;
using Reenbit.Inventory.Public.Contracts.Responses;
using MediatR;

namespace Reenbit.Inventory.Cqrs.Commands;

public record CreateProductCommand(string Title, string Description, decimal Price, int RemainingCount): IRequest<ProductResponse>;

internal class CreateProductCommandHandler(DaprClient daprClient)
    : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private const string _stateStore = "statestore";
    
    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product();
        product.Create(request.Title, request.Description, request.Price, request.RemainingCount);
        
        var products = await daprClient.GetStateAsync<List<Product>>(_stateStore, "products", cancellationToken: cancellationToken);
        products.Add(product);

        await daprClient.SaveStateAsync(_stateStore, "products", products, cancellationToken: cancellationToken);

        return new ProductResponse
        {
            Id = product.ProductId, 
            Title = product.Title, 
            Description = product.Description,
            RemainingCount = product.RemainingCount
        };
    }
}