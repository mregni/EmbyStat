using EmbyStat.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rollbar;

namespace EmbyStat.Common.Exceptions
{
    public class BusinessExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            var logger = LogFactory.CreateLoggerForType(typeof(BusinessExceptionFilterAttribute), "EXCEPTION-HANDLER");

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
                    logger.Error(ex.InnerException.InnerException);
                }
                else if (ex.InnerException != null)
                {
                    RollbarLocator.RollbarInstance.Error(ex.InnerException);
                    logger.Error(ex.InnerException);
                }

                context.HttpContext.Response.StatusCode = ex.StatusCode;
                logger.Warn(ex, $"Application thrown error: {ex.Message}");
                logger.Warn($"Frontend will know what to do with this!");
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
                logger.Error(context.Exception, msg);
            }

            context.Result = new JsonResult(apiError);
            context.ExceptionHandled = true;
            base.OnException(context);
        }
    }
}
