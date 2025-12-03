using BuildingBlocks.Exceptions;

namespace Catalog.API.Excepiton;

public class ProductNotFoundException : NotFoundException
{
    public ProductNotFoundException(Guid Id) : base("Product", Id) 
    {
    }
}

