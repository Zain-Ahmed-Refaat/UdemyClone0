using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Security.Claims;
using UdemyClone.Models;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;
        }

        [HttpGet("View-Quiz")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetQuiz(Guid lessonId)
        {
            var studentId = GetIdFromToken();

            if (lessonId == Guid.Empty)
                return BadRequest("Lesson ID cannot be empty.");

            try
            {
                var quizId = await _quizService.GetQuizIdByLessonIdAsync(lessonId);

                if (quizId == Guid.Empty)
                    return BadRequest("Quiz ID is null or empty.");

                var quiz = await _quizService.GetQuizByIdAsync(quizId, studentId);

                if (quiz == null)
                    return NotFound("Quiz not found.");

                return Ok(quiz);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        [HttpPost("Create-Quiz")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> CreateQuiz([FromBody] CreateQuizRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                await _quizService.CreateQuizAsync(request);
                return CreatedAtAction(nameof(GetQuiz), new { quizId = request.LessonId }, request);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the quiz.");
            }
        }

        [HttpPost("Submit-Quiz")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> SubmitQuiz([FromQuery] Guid quizId, [FromBody] SubmitQuizRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentId = GetIdFromToken();

            try
            {
                var result = await _quizService.SubmitQuizAsync(quizId, request, studentId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, dbEx.InnerException?.Message ?? dbEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Retake-Quiz")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> RetakeQuiz([FromQuery] Guid quizId, [FromBody] SubmitQuizRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentId = GetIdFromToken();

            try
            {
                var result = await _quizService.RetakeQuizAsync(quizId, request, studentId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (DbUpdateException dbEx)
            {
                return StatusCode(500, dbEx.InnerException?.Message ?? dbEx.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("View-Quiz-Result-Student")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> GetQuizResult(Guid quizId)
        {
            var studentId = GetIdFromToken();

            try
            {
                var result = await _quizService.GetQuizResultAsync(quizId, studentId);
                return Ok(result);
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("View-Quiz-Result-Instructor")]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> GetQuizResults(Guid quizId)
        {
            var instructorId = GetIdFromToken();

            var isOwner = await _quizService.IsInstructorOwnerOfQuizAsync(instructorId, quizId);

            if (!isOwner)
            {
                return Unauthorized("You do not have permission to view this quiz.");
            }

            var quizResults = await _quizService.GetQuizResultsByIdAsync(quizId);

            if (quizResults == null || !quizResults.Any())
            {
                return NotFound("No results found for this quiz.");
            }

            return Ok(quizResults);
        }


        [HttpDelete("Delete-Quiz")]
        [Authorize(Roles = "Admin, Instructor")]
        public async Task<IActionResult> DeleteQuiz(Guid quizId)
        {
            try
            {
                await _quizService.DeleteQuizAsync(quizId);
                return Ok("Quiz deleted successfully.");
            }
            catch (NotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
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
