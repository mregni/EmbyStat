using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Exceptions
{
    public class BusinessException : Exception
	{
		public int StatusCode { get; set; }

		public BusinessException(string message, int statusCode = 500) : base(message)
		{
			StatusCode = statusCode;
		}

		public BusinessException(Exception ex, int statusCode = 500) : base(ex.Message)
		{
			StatusCode = statusCode;
		}
	}
}
