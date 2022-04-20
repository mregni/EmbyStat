using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rollbar;
using Serilog;

namespace EmbyStat.Common.Exceptions;

public class BusinessExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ApiError apiError;
        var logger = context.HttpContext.RequestServices.GetService<ILogger<BusinessExceptionFilterAttribute>>();
        if (context.Exception is BusinessException ex)
        {
#if !DEBUG
                string stack = null;
#else
            var stack = ex.StackTrace;
#endif
            apiError = new ApiError(ex.Message, stack, true);
            context.Exception = null;

            if (ex.InnerException?.InnerException != null)
            {
                RollbarLocator.RollbarInstance.Error(ex.InnerException.InnerException);
                logger?.LogWarning(ex.InnerException.InnerException, "BusinessException occured");
            }
            else if (ex.InnerException != null)
            {
                RollbarLocator.RollbarInstance.Error(ex.InnerException);
                logger?.LogWarning(ex.InnerException, "BusinessException occured");
            }

            logger?.LogWarning(ex, "Top BusinessException");
            context.HttpContext.Response.StatusCode = ex.StatusCode;
        }
        else
        {
#if !DEBUG
                var msg = "An unhandled error occurred.";
                string stack = null;
#else
            var msg = context.Exception.GetBaseException().Message;
            var stack = context.Exception.StackTrace;
#endif
            apiError = new ApiError(msg, stack, false);
            context.HttpContext.Response.StatusCode = 500;

            RollbarLocator.RollbarInstance.Error(context.Exception);
            logger?.LogError(context.Exception, "Exception occured");
        }

        context.Result = new JsonResult(apiError);
        context.ExceptionHandled = true;
        base.OnException(context);
    }
}