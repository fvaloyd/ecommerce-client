using Ecommerce.Client.BackendClient.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Ecommerce.Client.Filters;

public class BackendExceptionFilter : IExceptionFilter
{
    private readonly IDictionary<Type, Action<ExceptionContext>> _exceptionHandlers;
    public BackendExceptionFilter()
    {
        _exceptionHandlers = new Dictionary<Type, Action<ExceptionContext>>
        {
            {typeof(BadRequestException), BadRequestHandle},
            {typeof(NotFoundException), NotFoundHandle},
            {typeof(InternalServerErrorException), InternalServerErrorHandle}
        };
    }

    public void OnException(ExceptionContext context)
    {
        HandleException(context);
    }

    private void HandleException(ExceptionContext context)
    {
        Type type = context.Exception.GetType();

        if (_exceptionHandlers.ContainsKey(type))
        {
            _exceptionHandlers[type].Invoke(context);
            return;
        }
    }

    private void NotFoundHandle(ExceptionContext context)
    {
        var exception = (NotFoundException)context.Exception;

        context.Result = new RedirectToPageResult("/NotFound", routeValues: exception.Reason);
        context.ExceptionHandled = true;
    }

    private void BadRequestHandle(ExceptionContext context)
    {
        var exception = (BadRequestException)context.Exception;

        context.Result = new RedirectToPageResult("/NotFound", routeValues: exception.Reason);
        context.ExceptionHandled = true;
    }

    private void InternalServerErrorHandle(ExceptionContext context)
    {
        var exception = (InternalServerErrorException)context.Exception;

        context.Result = new RedirectToPageResult("/NotFound", routeValues: exception.Reason);
        context.ExceptionHandled = true;
    }
}