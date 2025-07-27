using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

public class CustomExceptionFilter : IExceptionFilter
{

    public CustomExceptionFilter()
    {
    }

    public void OnException(ExceptionContext context)
    {
        var statusCode = 500;
        var message = "Ocurrió un error inesperado.";

        if (context.Exception is ArgumentException)
        {
            statusCode = 400;
            message = context.Exception.Message;
        }
        string safeMessage = ($"Error: {context.Exception.Message} {(context.Exception.InnerException is not null ? context.Exception.InnerException : "")}")
            .Replace("\r", "")
            .Replace("\n", "")
            .Replace("\t", "")
            .Trim();

        context.HttpContext.Response.Headers.Add("message", safeMessage);

        context.Result = new ObjectResult(context.Exception.Message)
        {
            StatusCode = 500
        };

        context.ExceptionHandled = true;
    }
}
