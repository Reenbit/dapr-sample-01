using System.Collections.Generic;
using System.Threading.Tasks;
using Dapr;
using Reenbit.Inventory.API.Constants;
using Reenbit.Inventory.Cqrs.Commands;
using Reenbit.Inventory.Cqrs.Queries;
using Reenbit.Inventory.Public.Contracts.Requests;
using Reenbit.Inventory.Public.Contracts.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Reenbit.Ordering.Public.Events;

namespace Reenbit.Inventory.API.Controllers;

/// <summary>
/// Products inventory
/// </summary>
[ApiExplorerSettings(GroupName = RoutingConstants.Documentation._clientInterface)]
[Route(RoutingConstants.Products._base)]
public class ProductsController: BaseController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductResponse>))]
    public async Task<IActionResult> GetAllProducts()
    {
        var query = new GetAllProductsQuery();

        var result = await Mediator.Send(query);

        return Ok(result);
    }
    
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    public async Task<IActionResult> CreateProduct([FromBody]CreateNewProductRequest request)
    {
        var command = new CreateProductCommand(request.Title, request.Description, request.Price, request.RemainingCount);

        var result = await Mediator.Send(command);

        return Ok(result);
    }

    [HttpPost]
    [Topic(pubsubName: "pubsub", name: "order-placed")]
    public async Task<IActionResult> UpdateProductsRemainingAmount(OrderPlaced orderPlaced)
    {
        var command = new UpdateProductsRemainingCountCommand(orderPlaced.Items);

        await Mediator.Send(command);

        return Ok();
    }
}