using System;

namespace WebApiWithExceptionHandler.Exceptions
{
    public class ApiValidationException : Exception
    {
        public ApiValidationException(string message) : base(message)
        {
            
        }
    }
}