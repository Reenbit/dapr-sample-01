using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Reenbit.Inventory.Public.Contracts.Responses;
using MediatR;
using Reenbit.Inventory.Domain.Entities;

namespace Reenbit.Inventory.Cqrs.Queries;

public record GetAllProductsQuery:IRequest<List<ProductResponse>>;

internal class GetAllProductsQueryHandler(DaprClient daprClient) : IRequestHandler<GetAllProductsQuery, List<ProductResponse>>
{
    private const string _stateStore = "statestore";
    
    public async Task<List<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await daprClient.GetStateAsync<List<Product>>(_stateStore, "products", cancellationToken: cancellationToken);

        return products
            .Select(x => new ProductResponse
            {
                Id = x.ProductId, Title = x.Title, RemainingCount = x.RemainingCount,
                Description = x.Description
            }).ToList();
    }
}