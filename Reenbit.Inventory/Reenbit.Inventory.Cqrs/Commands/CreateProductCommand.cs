using System.Threading;
using System.Threading.Tasks;
using Reenbit.Inventory.Domain.Entities;
using Reenbit.Inventory.Public.Contracts.Responses;
using MediatR;
using Raven.Client.Documents.Session;

namespace Reenbit.Inventory.Cqrs.Commands;

public record CreateProductCommand(string Title, string Description, decimal Price, int RemainingCount): IRequest<ProductResponse>;

internal class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponse>
{
    private readonly IAsyncDocumentSession _documentSession;

    public CreateProductCommandHandler(IAsyncDocumentSession documentSession)
    {
        _documentSession = documentSession;
    }

    public async Task<ProductResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product();
        product.Create(request.Title, request.Description, request.Price, request.RemainingCount);

        await _documentSession.StoreAsync(product, cancellationToken);

        await _documentSession.SaveChangesAsync(cancellationToken);

        return new ProductResponse
        {
            Id = product.ProductId, 
            Title = product.Title, 
            Description = product.Description,
            RemainingCount = product.RemainingCount
        };
    }
}