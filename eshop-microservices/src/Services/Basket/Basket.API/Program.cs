using BuildingBlocks.Exceptions.Handler;
using Disount.Grpc;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Distributed;

var builder = WebApplication.CreateBuilder(args);


//Add Services to the container
var assembly = typeof(Program).Assembly;

//Application 
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssemblies(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
    config.AddOpenBehavior(typeof(LoggingBehavior<,>));
});
builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssembly(assembly);

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Data Services
builder.Services.AddMarten(opts =>
{
    opts.Connection(builder.Configuration.GetConnectionString("Database")!);
    opts.Schema.For<ShoppingCart>().Identity(x => x.UserName);
}).UseLightweightSessions();


#region Services Registration

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.Decorate<IBasketRepository, CachedBasketRepository>();


/*
builder.Services.AddScoped<IBasketRepository>(provider =>
{
    var basketRepository = provider.GetRequiredService<BasketRepository>();

    return new CachedBasketRepository(basketRepository, provider.GetRequiredService<IDistributedCache>());
});
*/

#endregion

//Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    //options.InstanceName = "Basket";
});

//GRPC Services
builder.Services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(options =>
{
    options.Address = new Uri(builder.Configuration["GrpcSettings:DiscountUrl"]!);
});


//Cross-cutting services
builder.Services.AddExceptionHandler<CustomExceptionHandler>();

//Health Check 
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("Database")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);


var app = builder.Build();

// Configure the Http Request Pipeline
app.MapCarter();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(opt => { });
app.UseHealthChecks("/health",
    new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

app.Run();