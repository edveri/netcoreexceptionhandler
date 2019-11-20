using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS;
using Microsoft.Extensions.Logging;
using WebApiWithExceptionHandler.Exceptions.Abstract;

namespace WebApiWithExceptionHandler.Exceptions
{
    public class ExceptionHandlerHelper : IExceptionHandlerHelper
    {
        public async Task HandleExceptionAsync(HttpContext context, IDictionary<int, ClientErrorData> clientErrorMapping, ILogger<Startup> logger)
        {
            var id = Guid.NewGuid();
            var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
            var exception = errorFeature.Error;
            var problemDetails = new ProblemDetails {Title = "An error occurred!"};

            logger.LogError(id.ToString(), exception.ToString());

            
            var errorDetail = "";
            var status = (int) HttpStatusCode.InternalServerError;
            
            switch (exception)
            {
                case ApiValidationException _:
                    status = (int) HttpStatusCode.BadRequest;
                    errorDetail = exception.Message;
                    break;
                case BadHttpRequestException badHttpRequestException:
                {
                    problemDetails.Title = "Invalid request";

                    var propertyInfo = typeof(BadHttpRequestException).GetProperty("StatusCode",
                        BindingFlags.NonPublic | BindingFlags.Instance);
                    status = propertyInfo == null
                        ? (int) HttpStatusCode.InternalServerError
                        : (int) propertyInfo.GetValue(badHttpRequestException);

                    errorDetail = badHttpRequestException.Message;
                    break;
                }
                default:
                    errorDetail = "An unhandled Error occured, the error is logged";
                    break;
            }

            problemDetails.Status = status;
            problemDetails.Detail = errorDetail;
            problemDetails.Instance = $"urn:exampleapplication:error:{id}";
            clientErrorMapping.TryGetValue(status, out var type);
            problemDetails.Type = type?.Link ?? "https://tools.ietf.org/html/rfc7231";
            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = status;

            var result = JsonSerializer.Serialize(problemDetails);
            await context.Response.WriteAsync(result);
        }
    }
}