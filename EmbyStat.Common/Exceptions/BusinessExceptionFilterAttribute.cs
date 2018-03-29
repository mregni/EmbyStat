using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Serilog;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace EmbyStat.Common.Exceptions
{
	public class BusinessExceptionFilterAttribute : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			ApiError apiError;
			if (context.Exception is BusinessException)
			{
				var ex = context.Exception as BusinessException;
#if !DEBUG
                string stack = null;
#else
				string stack = context.Exception.StackTrace;
#endif
				apiError = new ApiError(ex.Message, stack);
				context.Exception = null;

				context.HttpContext.Response.StatusCode = ex.StatusCode;
				Log.Warning($"Application thrown error: {ex.Message}", ex);
				Log.Warning("Frontend will know what to do with this!");
			}
			else
			{
#if !DEBUG
                var msg = "An unhandled error occurred.";
                string stack = null;
#else
				var msg = context.Exception.GetBaseException().Message;
				string stack = context.Exception.StackTrace;
#endif

				apiError = new ApiError(msg, stack);
				context.HttpContext.Response.StatusCode = 500;

				Log.Error(context.Exception, msg);
			}

			// always return a JSON result
			context.Result = new JsonResult(apiError);

			base.OnException(context);
		}
	}
}
