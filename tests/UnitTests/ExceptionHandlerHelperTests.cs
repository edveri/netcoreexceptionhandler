using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using WebApiWithExceptionHandler;
using WebApiWithExceptionHandler.Exceptions;

namespace UnitTests
{
    public class ExceptionHandlerHelperTests
    {
        private DefaultHttpContext _defaultHttpContext;
        private IDictionary<int, ClientErrorData> _clientErrorMapping;
        private Mock<ILogger<Startup>> _mockLogger;
        private Mock<IExceptionHandlerFeature> _mockExceptionHandlerFeature;

        private ExceptionHandlerHelper _exceptionHandlerHelper;
        [SetUp]
        public void SetUp()
        {
            _defaultHttpContext = new DefaultHttpContext();
            _defaultHttpContext.Response.Body = new MemoryStream();
            _mockExceptionHandlerFeature = new Mock<IExceptionHandlerFeature>();
            _defaultHttpContext.Features.Set(_mockExceptionHandlerFeature.Object);
            _mockLogger = new Mock<ILogger<Startup>>();
            
            _exceptionHandlerHelper = new ExceptionHandlerHelper();

        }
        
        [Test]
        public async Task HandleExceptionAsync_WithApiValidationException_ProblemDetailsTitleEqualsErrorOccured()
        {
            //Arrange 
            var exception = new ApiValidationException("ApiValidationErrorOccured");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);
            
            
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_defaultHttpContext.Response.Body);
            var content = await reader.ReadToEndAsync();
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

            //Assert 
            Assert.AreEqual("An error occurred!", problemDetails.Title);
        }
        
        [Test]
        public async Task HandleExceptionAsync_WithApiValidationException_ProblemDetailsStatusEquals400()
        {
            //Arrange 
            var exception = new ApiValidationException("ApiValidationErrorOccured");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);
            
             
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_defaultHttpContext.Response.Body);
            var content = await reader.ReadToEndAsync();
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

            //Assert 
            Assert.AreEqual(400, problemDetails.Status);
        }
        
        [Test]
        public async Task HandleExceptionAsync_WithApiValidationException_ProblemDetailsDetailEqualsExceptionMessage()
        {
            //Arrange 
            var exception = new ApiValidationException("ApiValidationErrorOccured");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);
            
            
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_defaultHttpContext.Response.Body);
            var content = await reader.ReadToEndAsync();
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

            //Assert 
            Assert.AreEqual(exception.Message, problemDetails.Detail);
        }
        
        [Test]
        public async Task HandleException_WithExceptionNotBeingApiValidationException_DetailsDoesNotContainExceptionDetails()
        {
            //Arrange 
            var exception = new ArgumentException("Do not show this message");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);
            
            
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_defaultHttpContext.Response.Body);
            var content = await reader.ReadToEndAsync();
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

            //Assert 
            Assert.AreEqual("An unhandled Error occured, the error is logged", problemDetails.Detail);
        }
        
        [Test]
        public async Task HandleException_WithExceptionNotBeingApiValidationException_StatusCode500()
        {
            //Arrange 
            var exception = new ArgumentException("Do not show this message");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);
            //Assert 
            Assert.AreEqual(500, _defaultHttpContext.Response.StatusCode);
        }

        
        [Test]
        public async Task HandleExceptionAsync_ProblemDetailsContainsUrn()
        {
            //Arrange 
            var exception = new ApiValidationException("ApiValidationErrorOccured");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);
            
            
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_defaultHttpContext.Response.Body);
            var content = await reader.ReadToEndAsync();
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

            //Assert 
            StringAssert.Contains("urn:flavorbox:error:", problemDetails.Instance);
        }
        
        [Test]
        public async Task HandleExceptionAsync__ProblemDetailsTypeEqualsUrlFromClientErrorMapping()
        {
            //Arrange 
            var exception = new ApiValidationException("ApiValidationErrorOccured");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);
            
            
            _defaultHttpContext.Response.Body.Seek(0, SeekOrigin.Begin);
            using var reader = new StreamReader(_defaultHttpContext.Response.Body);
            var content = await reader.ReadToEndAsync();
            var problemDetails = JsonConvert.DeserializeObject<ProblemDetails>(content);

            //Assert 
            StringAssert.Contains(_clientErrorMapping[400].Link, problemDetails.Type);
        }

        [Test]
        public async Task HandleExceptionAsync_ContentTypeSetToProblemJson()
        {
            //Arrange 
            var exception = new ApiValidationException("ApiValidationErrorOccured");
            _clientErrorMapping = new Dictionary<int, ClientErrorData>{ {400, new ClientErrorData{Link = "https://400.com"}}};
            _mockExceptionHandlerFeature.Setup(m => m.Error).Returns(exception);
            
            //Act 
            await _exceptionHandlerHelper.HandleExceptionAsync(_defaultHttpContext, _clientErrorMapping, _mockLogger.Object);

            var contentType = _defaultHttpContext.Response.ContentType;
            
            //Assert 
            StringAssert.Contains("application/problem+json", contentType);
        }
        

    }

   
}