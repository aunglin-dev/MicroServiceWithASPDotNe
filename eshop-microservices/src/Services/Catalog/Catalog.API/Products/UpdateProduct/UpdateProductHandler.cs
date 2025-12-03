namespace Catalog.API.Products.UpdateProduct;

public record UpdateProductCommand(Guid Id, string Name, string Description, string ImageFile, List<string> Category, decimal Price) :
    ICommand<UpdateProductResult>;

public record UpdateProductResult(bool IsSuccess);

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).NotNull().WithMessage("Product Id is required");

        RuleFor(x => x.Name).NotEmpty().WithMessage("Product Name is required")
            .Length(2, 150).WithMessage("Name must be between 2 and 150 characters");

        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price Must be greater than 0");
    }
}

internal class UpdateProductHandler(IDocumentSession session, ILogger<UpdateProductHandler> logger) :
ICommandHandler<UpdateProductCommand, UpdateProductResult>
{
    public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("UpdateProductHandler.Handle called with {@command}", command);

        var product = await session.LoadAsync<Product>(command.Id, cancellationToken);

        if (product == null) throw new ProductNotFoundException(command.Id);

        product.Name = command.Name;
        product.Description = command.Description;
        product.ImageFile = command.ImageFile;
        product.Price = command.Price;
        product.Category = command.Category;
        product.Price = command.Price;


        session.Update(product);
        await session.SaveChangesAsync();

        return new UpdateProductResult(true);
    }
}

