using Disount.Grpc;

namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotEmpty().WithMessage("Cart can not be empty");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required");
    }
}

public class StoreBasketHandler(
    IBasketRepository repository,
    DiscountProtoService.DiscountProtoServiceClient discountProto)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        ShoppingCart newCart = command.Cart;
        await DeductDiscount(newCart, cancellationToken);

        var createCart = await repository.StorBasket(newCart, cancellationToken);

        return new StoreBasketResult(createCart.UserName);
    }

    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            var coupon = discountProto.GetDiscount(new GetDiscountRequest { ProductName = item.ProductName },
                cancellationToken: cancellationToken);
            item.Price -= coupon.Amount;
        }
    }
}