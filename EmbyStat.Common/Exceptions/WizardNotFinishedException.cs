using System;
using System.Collections.Generic;
using System.Text;

namespace EmbyStat.Common.Exceptions
{
    public class WizardNotFinishedException : Exception
    {
        public WizardNotFinishedException(string message):base(message)
        {
            
        }
    }
}
