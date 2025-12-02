using BuildingBlocks.CQRS;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BuildingBlocks.Behaviors;

public class ValidationBehavior<TRequest, TResponse>
    (IEnumerable<IValidator<TRequest>> validators,ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    :
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        logger.LogInformation("Starting validation for request type {RequestType}", typeof(TRequest).Name);

        var context = new ValidationContext<TRequest>(request);
        var validationResults =
             await Task.WhenAll(validators.Select(el => el.ValidateAsync(context, cancellationToken)));

        var failures =
            validationResults.Where(el => el.Errors.Any()).SelectMany(r => r.Errors).ToList();


        if (failures.Any())
        {
           var jsonData = JsonSerializer.Serialize(failures);
            logger.LogInformation("Validation didn't pass for request type {RequestType} , Detailed Data : {jsonData}", typeof(TRequest).Name, jsonData);
            throw new ValidationException(failures);
        }

        return await next();

    }
}
