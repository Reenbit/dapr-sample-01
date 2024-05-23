using System.Threading;
using System.Threading.Tasks;
using Reenbit.Ordering.Public.Events;

namespace Reenbit.Inventory.Cqrs.Commands;

public class UpdateProductsRemainingCount
{
    public async Task Handle(OrderPlaced @event, CancellationToken cancellationToken)
    {
    }
}