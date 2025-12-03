using Marten.Linq.QueryHandlers;
using System.Collections;

namespace Catalog.API.Products.GetProduct;

public record GetProductQuery() : IQuery<GetProductResults>;

public record GetProductResults(IEnumerable<Product> Products);

internal class GetProductQueryHandler(IDocumentSession session) :
IQueryHandler<GetProductQuery, GetProductResults>
{
    public async Task<GetProductResults> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        
       var product = await session.Query<Product>().ToListAsync(cancellationToken);


        return new GetProductResults(product);
    }
}

