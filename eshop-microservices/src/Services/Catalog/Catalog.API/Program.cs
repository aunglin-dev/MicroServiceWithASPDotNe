using BuildingBlocks.Behaviors;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;

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



app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        //context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        //// using static System.Net.Mime.MediaTypeNames;
        //context.Response.ContentType = Text.Plain;

        var exceptionHandlerPathFeature =
            context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

        if (exceptionHandlerPathFeature == null) return;

        var problemDetails = new ProblemDetails
        {
            Title = exceptionHandlerPathFeature.Message,
            Status = StatusCodes.Status500InternalServerError,
            Detail = exceptionHandlerPathFeature.StackTrace,
        };

        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(exceptionHandlerPathFeature, exceptionHandlerPathFeature.Message);

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = Text.Plain;


        await context.Response.WriteAsJsonAsync(problemDetails);

    });
});


app.Run();
