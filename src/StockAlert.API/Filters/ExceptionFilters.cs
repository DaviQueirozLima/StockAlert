using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StockAlert.Communication.Responses;
using StockAlert.Exception;

namespace StockAlert.API.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            switch (context.Exception)
            {
                case ValidationException validationException:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                    context.Result = new BadRequestObjectResult(
                        new ResponseErrorJson(validationException.Errors)
                    );
                    break;

                case UnauthorizedException unauthorizedException:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Result = new UnauthorizedObjectResult(
                        new ResponseErrorJson(unauthorizedException.Message)
                    );
                    break;

                case NotFoundException notFoundException:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Result = new NotFoundObjectResult(
                        new ResponseErrorJson(notFoundException.Message)
                    );
                    break;

                default:
                    context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Result = new ObjectResult(new
                    {
                        message = context.Exception.Message,
                        innerException = context.Exception.InnerException?.Message,
                        stackTrace = context.Exception.StackTrace
                    });
                    break;
            }

            context.ExceptionHandled = true;
        }
    }
}