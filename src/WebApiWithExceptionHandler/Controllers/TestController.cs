using System;
using Microsoft.AspNetCore.Mvc;
using WebApiWithExceptionHandler.Dto;

namespace WebApiWithExceptionHandler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            throw new Exception("Ooops! Some Exception occured");
        }

        [HttpPost]
        public void Post(TestDto testDto)
        {
        }
    }
}