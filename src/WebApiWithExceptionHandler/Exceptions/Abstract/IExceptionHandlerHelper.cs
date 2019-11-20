using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace WebApiWithExceptionHandler.Exceptions.Abstract
{
    public interface IExceptionHandlerHelper
    {
        Task HandleExceptionAsync(HttpContext context,  IDictionary<int, ClientErrorData> clientErrorMapping, ILogger<Startup> logger);
    }
}