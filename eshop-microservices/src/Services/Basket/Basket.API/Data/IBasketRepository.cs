namespace Basket.API.Data;

public interface IBasketRepository
{
    Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken = default);
    
    Task<ShoppingCart> StorBasket(ShoppingCart basket, CancellationToken cancellationToken = default);

    Task<bool> DeleteBasket(string username, CancellationToken cancellationToken = default);
}