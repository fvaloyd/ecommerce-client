using Refit;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Client.Filters;

public class ApiExceptionFilter : IExceptionFilter
{
    private readonly IDictionary<HttpStatusCode, Action<ExceptionContext>> _exceptionHandlers;
    public ApiExceptionFilter()
    {
        _exceptionHandlers = new Dictionary<HttpStatusCode, Action<ExceptionContext>>
        {
            {HttpStatusCode.BadRequest, BadRequestHandle},
            {HttpStatusCode.NotFound, NotFoundHandle},
            {HttpStatusCode.InternalServerError, InternalServerErrorHandle}
        };
    }

    public void OnException(ExceptionContext context)
    {
        HandleException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        if (context.Exception is not ApiException exception)
            return;

        if (!_exceptionHandlers.ContainsKey(exception.StatusCode))
            return;

        _exceptionHandlers[exception.StatusCode].Invoke(context);
    }

    private void NotFoundHandle(ExceptionContext context)
    {
        context.Result = new RedirectToPageResult("/NotFound");
        context.ExceptionHandled = true;
    }

    private void BadRequestHandle(ExceptionContext context)
    {
        context.Result = new RedirectToPageResult("/NotFound");
        context.ExceptionHandled = true;
    }

    private void InternalServerErrorHandle(ExceptionContext context)
    {
        context.Result = new RedirectToPageResult("/NotFound");
        context.ExceptionHandled = true;
    }
}