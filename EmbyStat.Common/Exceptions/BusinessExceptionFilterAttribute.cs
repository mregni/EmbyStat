using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Serilog;

namespace EmbyStat.Common.Exceptions
{
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

                context.HttpContext.Response.StatusCode = ex.StatusCode;
                Log.Warning($"{Constants.LogPrefix.ExceptionHandler}\tApplication thrown error: {ex.Message}", ex);
                Log.Warning($"{Constants.LogPrefix.ExceptionHandler}\tFrontend will know what to do with this!");
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
                Log.Error(context.Exception, $"{Constants.LogPrefix.ExceptionHandler}\tUnhandled backend exception");

                apiError = new ApiError(msg, stack, false);
                context.HttpContext.Response.StatusCode = 500;

                Log.Error(context.Exception, msg);
            }

            context.Result = new JsonResult(JsonConvert.SerializeObject(apiError));
            base.OnException(context);
        }
    }
}
