
using Catalog.API.Products.UpdateProduct;

namespace Catalog.API.Products.DeleteProduct;

public record DeleteProductResponse(bool IsDelete);

public class DeleteProductEndPoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapDelete("/product/{Id}",async(Guid Id, ISender sender) =>
        {
            var result = await sender.Send(new DeleteProductCommand(Id));
            var response = result.Adapt<DeleteProductResponse>();

            return Results.Ok(response);

        }).WithName("DeleteProduct")
            .Produces<DeleteProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Delete Product")
            .WithDescription("Delete Product");
    }
}

