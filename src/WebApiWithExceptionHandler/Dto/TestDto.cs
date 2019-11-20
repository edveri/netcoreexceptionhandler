using System.ComponentModel.DataAnnotations;

namespace WebApiWithExceptionHandler.Dto
{
    public class TestDto
    {
        [Required]
        public string Name { get; set; }
    }
}