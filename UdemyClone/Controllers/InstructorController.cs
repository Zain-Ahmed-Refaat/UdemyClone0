using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Models;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorService instructorService;

        public InstructorController(IInstructorService instructorService)
        {
            this.instructorService = instructorService;
        }

        [HttpPost("Upload-Course")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateCourse([FromBody] CourseModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var instructorId = GetIdFromToken();

                var course = new Course
                {
                    Name = model.Name,
                    Description = model.Description,
                    InstructorId = instructorId,
                    TopicId = model.TopicId
                };

                var createdCourse = await instructorService.CreateCourseAsync(course);

                return CreatedAtAction(nameof(GetCourseById), new { id = createdCourse.Id }, createdCourse);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("Update-Course")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CourseDto courseDto)
        {
            if (id == Guid.Empty)
                return BadRequest("Course ID cannot be empty.");

            var instructorId = GetIdFromToken();
            var updatedCourse = await instructorService.UpdateCourseAsync(courseDto);

            if (updatedCourse == null)
                return NotFound("Course not found or not created by this instructor.");

            return Ok(updatedCourse);
        }

        [HttpGet("Get-Courses-By-Instructor")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetCoursesByInstructor(Guid instructorId)
        {
            if (instructorId == Guid.Empty)
                return BadRequest("Instructor ID cannot be empty.");

            var courses = await instructorService.GetCoursesByInstructorAsync(instructorId);

            if (courses == null || !courses.Any())
                return NotFound("No courses found for this instructor.");

            return Ok(courses);
        }

        [HttpDelete("Delete-Course")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> DeleteCourse(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Course ID cannot be empty.");

            var instructorId = GetIdFromToken();

            var result = await instructorService.DeleteCourseAsync(id, instructorId);

            if (!result)
                return NotFound("Course not found or not created by this instructor.");

            return NoContent();
        }

        [HttpGet("Get-Course-By-Id")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetCourseById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Course ID cannot be empty.");

            var course = await instructorService.GetCourseByIdAsync(id);

            if (course == null)
                return NotFound("Course not found.");

            return Ok(course);
        }

        [HttpGet("Get-All-Courses")]
        public async Task<IActionResult> GetAllCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var (courses, totalPages) = await instructorService.GetAllCoursesAsync(pageNumber, pageSize);

            if (courses.Any())
            {
                return Ok(new
                {
                    PageIndex = pageNumber,
                    TotalPages = totalPages,
                    Courses = courses
                });
            }

            return NotFound("No courses found.");
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
