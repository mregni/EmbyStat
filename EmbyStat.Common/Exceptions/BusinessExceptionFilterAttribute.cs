using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Rollbar;

namespace EmbyStat.Common.Exceptions;

public class BusinessExceptionFilterAttribute : ExceptionFilterAttribute
{
    public override void OnException(ExceptionContext context)
    {
        ApiError apiError;
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
            }
            else if (ex.InnerException != null)
            {
                RollbarLocator.RollbarInstance.Error(ex.InnerException);
            }

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
        }

        context.Result = new JsonResult(apiError);
        context.ExceptionHandled = true;
        base.OnException(context);
    }
}