using Disount.Grpc.Models;
using Microsoft.EntityFrameworkCore;

namespace Disount.Grpc.Data;

public class DiscountContext : DbContext
{
    public DbSet<Coupon> Coupons { get; set; }

    public DiscountContext(DbContextOptions<DiscountContext> options) : base(options)
    {
    }
}