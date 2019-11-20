using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebApiWithExceptionHandler.Validation
 {
     public class ValidationProblemDetailsResult : IActionResult
     {
         private readonly string _errorLink;

         public ValidationProblemDetailsResult(string errorLink)
         {
             _errorLink = errorLink ?? throw new ArgumentNullException(nameof(errorLink));
         }
         
         public async Task ExecuteResultAsync(ActionContext context)
         {
             var modelStateEntries = context.ModelState.Where(e => e.Value.Errors.Count > 0).ToArray();
             var errors = new List<ValidationError>();

             var details = "See ValidationErrors for details";

             if (modelStateEntries.Any())
             {
                 if (modelStateEntries.Length == 1 && modelStateEntries[0].Value.Errors.Count == 1 &&
                     modelStateEntries[0].Key == string.Empty)
                 {
                     details = modelStateEntries[0].Value.Errors[0].ErrorMessage;
                 }
                 else
                 {
                     foreach (var modelStateEntry in modelStateEntries)
                     {
                         foreach (var modelStateError in modelStateEntry.Value.Errors)
                         {
                             var error = new ValidationError
                             {
                                 Name = modelStateEntry.Key,
                                 Description = modelStateError.ErrorMessage
                             };

                             errors.Add(error);
                         }
                     }
                 }
             }
             
             var problemDetails = new ValidationProblemDetails
             {
                 Status = 400,
                 Type = _errorLink,
                 Title = "Request Validation Error",
                 Instance = $"urn:exampleapplication:badrequest:{Guid.NewGuid()}",
                 Detail = details,
                 ValidationErrors = errors
             };

             var objectResult = new ObjectResult(problemDetails) {StatusCode = problemDetails.Status};
             await objectResult.ExecuteResultAsync(context);

         }
     }
 }