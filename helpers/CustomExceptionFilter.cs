using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

public class CustomExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        var statusCode = 500;
        var message = "Ocurrió un error inesperado.";

        if (context.Exception is ArgumentException)
        {
            statusCode = 400;
            message = context.Exception.Message;
        }

        string safeMessage = $"Error: {context.Exception.Message}";

        if (context.Exception.InnerException is not null)
        {
            safeMessage += $" | Inner: {context.Exception.InnerException.Message}";
        }

        safeMessage = safeMessage
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\t", "")
            .Trim();

        context.HttpContext.Response.Headers["message"] = safeMessage;

        context.Result = new ObjectResult(new
        {
            error = context.Exception.Message,
            inner = context.Exception.InnerException?.Message
        })
        {
            StatusCode = statusCode
        };

        context.ExceptionHandled = true;
    }
}
