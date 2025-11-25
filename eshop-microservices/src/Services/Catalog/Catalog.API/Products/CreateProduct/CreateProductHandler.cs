using System.Diagnostics.Eventing.Reader;

namespace Catalog.API.Products.CreateProduct;


public record CreateProductCommand(string Name, string Description, string ImageFile, List<string> Category, decimal Price)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);


internal class CreateProductHandler(IDocumentSession session) : 
    ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        //Create Product Entity from Command Object

        Product product = new Product()
        {
            Name = command.Name,
            Description = command.Description,
            ImageFile = command.ImageFile,
            Price = command.Price,
            Category = command.Category,
        };

        //TODO
        //Save to the DB
        session.Store(product);
        await session.SaveChangesAsync();


        //Create Product Result
        return new CreateProductResult(Guid.NewGuid());

       
    }
}

