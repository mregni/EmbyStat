using System;

namespace EmbyStat.Common.Exceptions;

public class WizardNotFinishedException : Exception
{
    public WizardNotFinishedException(string message):base(message)
    {
            
    }
}