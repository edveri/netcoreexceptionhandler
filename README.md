# NetCoreExceptionHandler

Global exception handling for ASP .NET Core 3 application, handling both 
misc. exception and model validation errors. 

Aiming to follow RFC7807 standard:
 https://tools.ietf.org/html/rfc7807

All credits go to: Filip W. and this post: 
https://www.strathweb.com/2018/07/centralized-exception-handling-and-request-validation-in-asp-net-core/

Tests and refactoring will be added in the future, for now this serves as a simple example. 

## Example Response

{
    "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
    "title": "An error occurred!",
    "status": 400,
    "detail": "adsdsdas",
    "instance": "urn:exampleapp:error:2d81a3bf-8ca5-4a30-aedc-dbaf4f220d32"
}