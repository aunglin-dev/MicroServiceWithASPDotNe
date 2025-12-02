using BuildingBlocks.Behaviors;

var builder = WebApplication.CreateBuilder(args);

//Add DI services
var assembly = typeof(Program).Assembly;
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(assembly);
    config.AddOpenBehavior(typeof(ValidationBehavior<,>));
});
builder.Services.AddCarter();
builder.Services.AddValidatorsFromAssembly(assembly);
builder.Services.AddMarten(options => options.Connection(builder.Configuration.GetConnectionString("Database")!))
.UseLightweightSessions();




var app = builder.Build();


// Configure the Http Request Pipeline
app.MapCarter();
app.Run();
