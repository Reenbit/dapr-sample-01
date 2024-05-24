using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Reenbit.Ordering.Public.Events;

namespace Reenbit.Inventory.Cqrs.Commands;

public record UpdateProductsRemainingCountCommand(List<OrderItem> OrderedItems) : IRequest;

internal class UpdateProductsRemainingCountCommandHandler: IRequestHandler<UpdateProductsRemainingCountCommand>
{
    public Task<Unit> Handle(UpdateProductsRemainingCountCommand request, CancellationToken cancellationToken)
    {
        
    }
}