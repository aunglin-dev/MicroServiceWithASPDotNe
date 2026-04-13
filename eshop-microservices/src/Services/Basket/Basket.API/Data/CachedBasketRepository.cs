using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Data;

public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache)
    : IBasketRepository
{
    public async Task<ShoppingCart> GetBasket(string username, CancellationToken cancellationToken = default)
    {
        var cachedBasket = await cache.GetStringAsync(username, cancellationToken);
        if (!string.IsNullOrEmpty(cachedBasket))
            return JsonConvert.DeserializeObject<ShoppingCart>(cachedBasket)!;

        //if cache is empty 
        var basket = await repository.GetBasket(username, cancellationToken);
        await cache.SetStringAsync(username, JsonConvert.SerializeObject(basket), cancellationToken);
        return basket;
    }

    public async Task<ShoppingCart> StorBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
    {
        await repository.StorBasket(basket, cancellationToken);
        await cache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket), cancellationToken);
        return basket;
    }

    public async Task<bool> DeleteBasket(string username, CancellationToken cancellationToken = default)
    {
        await repository.DeleteBasket(username, cancellationToken);
        await cache.RemoveAsync(username, cancellationToken);
        return true;
    }
}