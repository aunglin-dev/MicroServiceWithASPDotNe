var builder = WebApplication.CreateBuilder(args);

//Add DI services
builder.Services.AddCarter();
builder.Services.AddMediatR(config => config.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddMarten(options => options.Connection(builder.Configuration.GetConnectionString("Database")!))
.UseLightweightSessions();




var app = builder.Build();


// Configure the Http Request Pipeline
app.MapCarter();
app.Run();
