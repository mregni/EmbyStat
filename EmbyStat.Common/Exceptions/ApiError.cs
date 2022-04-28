namespace EmbyStat.Common.Exceptions;

public class ApiError
{
	public string Message { get;  }
	public bool IsError { get;  }
	public string Stack { get;  }

	public ApiError(string message, string stack, bool isError)
	{
		Message = message;
		Stack = stack;
		IsError = isError;
	}
}