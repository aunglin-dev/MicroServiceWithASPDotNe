namespace Catalog.API.Products.CreateProduct;


public record CreateProductCommand(string Name, string Description, string ImageFile, List<string> Category, decimal Price)
    : ICommand<CreateProductResult>;

public record CreateProductResult(Guid Id);

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("Name Is Required");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description Is Required");
        RuleFor(x => x.ImageFile).NotEmpty().WithMessage("ImageFile Is Required");
        RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0 ");
    }
}



internal class CreateProductHandler(IDocumentSession session, IValidator<CreateProductCommand> validator, ILogger<CreateProductHandler> logger) :
    ICommandHandler<CreateProductCommand, CreateProductResult>
{
    public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {

        var validatorResult = await validator.ValidateAsync(command, cancellationToken);    
        var validatorErrorsMessage = validatorResult.Errors.Select(el=> el.ErrorMessage).ToList();

        if (validatorErrorsMessage.Any()) {
            var jsonErrorMessage = JsonSerializer.Serialize(validatorErrorsMessage);
            logger.LogInformation("[CreateProductHandler] didn't pass validation, Detailed data: {Data}",jsonErrorMessage);
            throw new ValidationException(validatorErrorsMessage.FirstOrDefault());
        }


        //Create Product Entity from Command 
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

