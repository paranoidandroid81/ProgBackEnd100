using Microsoft.AspNetCore.Mvc;
using System;
using LibraryApi.Services;
using System.Text.Json.Serialization;

namespace DemoApi.Controllers
{
    public class StatusController : Controller
    {
        private readonly IGenerateEnrollmentIds _enrollmentGenerator;

        public StatusController(IGenerateEnrollmentIds enrollmentGenerator)
        {
            _enrollmentGenerator = enrollmentGenerator;
        }

        [HttpGet("/status")]
        public IActionResult GetStatus()
        {
            var response = new ServerStatus
            {
                Id = 99,
                StatusMessage = "Cool",
                CheckedAt = DateTime.Now
            };
            return Ok(response);
        }

        //using route data
        [HttpGet("/blogs/{year::int:min(2015)}/{month:int:range(1,12)}/{day:int:range(1,31)}")]
        public IActionResult GetBlogPostsFor(int year, int month, int day)
        {
            return Ok($"Giving the block posts for {day}/{month}/{year}");
        }

        [HttpGet("/employees")]
        public IActionResult GetEmployeesForDepartment([FromQuery]string dept = "all")
        {
            return Ok($"Getting employees for department {dept}");
        }

        [HttpGet("/whoami")]
        public IActionResult WhoAmI([FromHeader(Name = "User-Agent")]string ua)
        {
            return Ok($"I see you are running {ua}");
        }

        [HttpPost("/enrollments")]
        public IActionResult AddEnrollment([FromBody] EnrollmentRequest request)
        {
            var id = _enrollmentGenerator.GetNewId();
            return Ok($"[{id}]: You are enrolled in {request.ClassEnrolledFor} for {request.NumberOfDays} days, {request.Student}");
        }
    }


    public class EnrollmentRequest
    {
        [JsonPropertyName("class")]
        public string ClassEnrolledFor { get; set; }
        public string Student { get; set; }
        public int NumberOfDays { get; set; }
    }


    public class ServerStatus
    {
        public int Id { get; set; }
        public string StatusMessage { get; set; }
        public DateTime CheckedAt { get; set; }
    }
}
