using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UdemyClone.Dto;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService courseService;

        public CourseController(ICourseService courseService)
        {
            this.courseService = courseService;
        }

        [HttpPost("Upload-Lesson")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UploadLesson([FromBody] LessonDto model)
        {

            var instructorId = GetIdFromToken();

            try
            {
                var lesson = await courseService.UploadLessonAsync(model, instructorId);
                return CreatedAtAction(nameof(GetLessonById), new { id = lesson.Id }, lesson);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Get-All-Lessons")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetAllLessons([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var instructorId = GetIdFromToken();

            try
            {
                var lessons = await courseService.GetAllLessonsAsync(instructorId, pageNumber, pageSize);
                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Get-Lesson-By-ID")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetLessonById(Guid id)
        {

            var instructorId = GetIdFromToken();

            try
            {
                var lesson = await courseService.GetLessonByIdAsync(id, instructorId);

                if (lesson == null)
                    return NotFound("Lesson not found.");

                return Ok(lesson);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        private Guid GetIdFromToken()
        {
            var userIdClaim = User.FindFirstValue("UserID");

            var userID = string.IsNullOrEmpty(userIdClaim) ?
                         throw new UnauthorizedAccessException("User ID claim not found in the token.") :
                          Guid.Parse(userIdClaim);

            return userID;
        }
    }
}
