using FluentValidation;
using Library_Api.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Library_Api.Infrastructure;

public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var (_, argumentValue) in context.ActionArguments)
        {
            if (argumentValue is null) continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argumentValue.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is IValidator validator)
            {
                var validationContext = new ValidationContext<object>(argumentValue);
                var validationResult = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors
                        .Select(e => e.ErrorMessage)
                        .Distinct()
                        .ToList();

                    context.Result = new BadRequestObjectResult(
                        ApiResponse.Failure("Validation failed.", errors)
                    );
                    return;
                }
            }
        }

        await next();
    }
}
