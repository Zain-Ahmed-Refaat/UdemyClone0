using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using System.Security.Claims;
using UdemyClone.Dto;
using UdemyClone.Services;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService studentService;
        private readonly IQuizService quizService;

        public StudentController(IStudentService studentService, IQuizService quizService)
        {
            this.studentService = studentService;
            this.quizService = quizService;
        }

        [HttpGet("Get-All-Students")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudentsAsync(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var students = await studentService.GetAllStudentsAsync(pageNumber, pageSize);

                if (students == null || !students.Any())
                {
                    return NotFound("No students found.");
                }

                return Ok(new
                {
                    TotalCount = students.Count(),
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Students = students
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Get-Student-By-Id")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> GetStudentById(Guid studentId)
        {
            var student = await studentService.GetStudentByIdAsync(studentId);

            return student != null ? Ok(student) : NotFound("Student not found.");
        }

        [HttpGet("Get-Courses-By-Student")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCoursesByStudent(Guid studentId)
        {
            var courses = await studentService.GetCoursesByStudentAsync(studentId);

            return courses != null ? Ok(courses) : NotFound("Student or courses not found.");
        }

        [HttpGet("Search-Course")]
        public async Task<IActionResult> SearchCoursesAsync([FromQuery] string keyword, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await studentService.SearchCoursesAsync(keyword, pageNumber, pageSize);
            return Ok(result);
        }

        [HttpGet("View-Course-Enrollments")]
        public async Task<IActionResult> GetCourseEnrollments(Guid courseId)
        {

            var enrollments = await studentService.GetCourseEnrollmentsAsync(courseId);

            if (enrollments == null || !enrollments.Any())
            {
                return NotFound("No enrollments found for this course.");
            }

            return Ok(enrollments);
        }

        [HttpPost("Enroll-Course")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(Guid courseId)
        {
            var userId = GetIdFromToken();

            if (userId == Guid.Empty)
                return Unauthorized();

            try
            {
                await studentService.EnrollCourseAsync(courseId, userId);
                return Ok(new { Message = "Enrollment successful" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost("UnEnroll-Course")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> UnenrollCourse(string courseName)
        {
            var userId = GetIdFromToken();

            if (userId == Guid.Empty)
                return Unauthorized("User is not authenticated.");

            var result = await studentService.UnenrollCourseAsync(userId, courseName);

            return result == "Successfully unenrolled from the course."
                ? Ok(result)
                : BadRequest(result);
        }


        [HttpGet("Get-My-Courses")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetMyCourses()
        {
            var studentId = GetIdFromToken();
            var courses = await studentService.GetCoursesByStudentAsync(studentId);

            return courses != null ? Ok(courses) : NotFound("Student or courses not found.");
        }

        [HttpGet("View-Lessons-In-Course")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetLessonsByCourse(Guid courseId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var studentId = GetIdFromToken();
                var lessons = await studentService.GetLessonsByCourseAsync(studentId, courseId, pageNumber, pageSize);
                return Ok(lessons);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Watch-Lesson")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetLesson(Guid lessonId)
        {
            var studentId = GetIdFromToken();

            try
            {
                var lesson = await studentService.GetLessonAsync(studentId, lessonId);
                return Ok(lesson);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-Quiz-status")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> CheckQuizStatus(Guid lessonId)
        {

            var studentId = GetIdFromToken();

            try
            {
                var quizId = await quizService.GetQuizIdByLessonIdAsync(lessonId);
                bool hasTakenQuiz = await quizService.HasStudentTakenQuizAsync(studentId, quizId);
                bool didPassQuiz = await quizService.DidStudentPassQuizAsync(studentId, quizId);

                return Ok(new
                {
                    HasTakenQuiz = hasTakenQuiz,
                    DidPassQuiz = didPassQuiz
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("Delete-Student")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteStudent(Guid studentId)
        {
            var result = await studentService.DeleteStudentAsync(studentId);

            return result == "Student deleted successfully."
                ? Ok(result)
                : NotFound(result);
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