using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Reenbit.Inventory.Public.Contracts.Responses;
using MediatR;

namespace Reenbit.Inventory.Cqrs.Queries;

public record GetAllProductsQuery:IRequest<List<ProductResponse>>;

internal class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductResponse>>
{
    public GetAllProductsQueryHandler()
    {
    }
    
    public async Task<List<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        return new List<ProductResponse>();
    }
}