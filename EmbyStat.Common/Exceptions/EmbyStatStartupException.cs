using System;

namespace EmbyStat.Common.Exceptions;

public class EmbyStatStartupException : Exception
{
    public EmbyStatStartupException(Exception innerException) 
        : base("EmbyStat failed to start: " + innerException.Message)
    {
        
    }
    
    public EmbyStatStartupException(Exception innerException, string message)
        : base("EmbyStat failed to start: " + message, innerException)
    {
    }
}