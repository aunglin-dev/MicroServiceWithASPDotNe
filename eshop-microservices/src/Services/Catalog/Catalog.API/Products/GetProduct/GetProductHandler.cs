namespace Catalog.API.Products.GetProduct;

public record GetProductQuery(int? PageNumnber = 1, int? PageSize = 10) : IQuery<GetProductResults>;

public record GetProductResults(IEnumerable<Product> Products);

internal class GetProductQueryHandler(IDocumentSession session) :
    IQueryHandler<GetProductQuery, GetProductResults>
{
    public async Task<GetProductResults> Handle(GetProductQuery query, CancellationToken cancellationToken)
    {
        var product = await session.Query<Product>()
            .ToPagedListAsync(query.PageNumnber ?? 1, query.PageSize ?? 10, cancellationToken);


        return new GetProductResults(product);
    }
}