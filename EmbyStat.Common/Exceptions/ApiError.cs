namespace EmbyStat.Common.Exceptions
{
    public class ApiError
    {
	    public string Message { get; set; }
	    public bool IsError { get; set; }
	    public string Detail { get; set; }
		public string Stack { get; set; }

	    public ApiError(string message, string stack, bool isError)
	    {
		    Message = message;
		    Stack = stack;
		    IsError = isError;
	    }
	}
}
