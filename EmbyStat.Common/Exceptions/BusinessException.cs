using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Exceptions
{
    public class BusinessException : Exception
	{
		public int StatusCode { get; set; }

		public BusinessException(string message, int statusCode = 500, Exception e = null) : base(message, e)
		{
			StatusCode = statusCode;
		}

		public BusinessException(Exception ex, int statusCode = 500, Exception e = null) : base(ex.Message, e)
		{
			StatusCode = statusCode;
		}
	}
}
