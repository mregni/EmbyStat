using System.Net;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EmbyStat.Web.Helpers
{
	public class WebExceptionFilter : ExceptionFilterAttribute
	{
		public override void OnException(ExceptionContext context)
		{
			if (context.Exception is WebException ex)
			{
				// handle explicit 'known' API errors
				
			}

			base.OnException(context);
		}
	}
}
