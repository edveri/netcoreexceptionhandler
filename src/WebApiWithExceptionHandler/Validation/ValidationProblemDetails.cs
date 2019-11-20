using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWithExceptionHandler.Validation
{
    public class ValidationProblemDetails : ProblemDetails
    {
        public ICollection<ValidationError> ValidationErrors { get; set; }
    }
}