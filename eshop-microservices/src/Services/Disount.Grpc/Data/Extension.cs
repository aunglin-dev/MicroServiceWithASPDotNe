using Microsoft.EntityFrameworkCore;

namespace Disount.Grpc.Data;

public static class Extension
{
    public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbcontext = scope.ServiceProvider.GetRequiredService<DiscountContext>();
        dbcontext.Database.MigrateAsync();

        return app;
    }
}