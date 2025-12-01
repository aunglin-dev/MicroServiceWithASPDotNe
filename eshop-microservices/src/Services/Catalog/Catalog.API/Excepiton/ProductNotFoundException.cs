namespace Catalog.API.Excepiton;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException() : base("Specific Product Not Found!") 
    {
    }
}

