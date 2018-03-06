using System;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Helpers
{
	public class WebExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			if (context.Exception is WebException ex)
			{
				// handle explicit 'known' API errors
				
			}
			else if (context.Exception is UnauthorizedAccessException)
			{
				context.HttpContext.Response.StatusCode = 401;

				// handle logging here
			}

			base.OnException(context);
		}
	}
}
