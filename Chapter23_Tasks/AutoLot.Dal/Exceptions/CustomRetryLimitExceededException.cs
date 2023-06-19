using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoLot.Dal.Exceptions
{
    public class CustomRetryLimitExceededException : CustomException
    {
        public CustomRetryLimitExceededException() { }
        public CustomRetryLimitExceededException(string message) : base(message) { }
        public CustomRetryLimitExceededException(
        string message, RetryLimitExceededException innerException)  : base(message, innerException) { }
    }
}
