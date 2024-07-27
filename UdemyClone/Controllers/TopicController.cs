using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UdemyClone.Dto;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService topicService;

        public TopicController(ITopicService topicService)
        {
            this.topicService = topicService;
        }

        [HttpPost("Create-Topic")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTopic([FromBody] CreateTopicDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || dto.SubCategoryId == Guid.Empty)
                return BadRequest("Invalid data.");

            try
            {
                var topic = await topicService.CreateTopicAsync(dto.Name, dto.SubCategoryId);
                return CreatedAtAction(nameof(GetTopicById), new { id = topic.Id }, topic);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-Topic-By-Id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTopicById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Topic ID.");

            try
            {
                var topic = await topicService.GetTopicByIdAsync(id);
                if (topic == null)
                    return NotFound("Topic not found.");

                return Ok(topic);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-All-Topics")]
        public async Task<IActionResult> GetAllTopics(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than zero.");

            try
            {
                var topics = await topicService.GetAllTopicsAsync(pageNumber, pageSize);
                return Ok(topics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPatch("Update-Topic")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateTopic(Guid id, [FromBody] UpdateTopicDto dto)
        {
            if (id == Guid.Empty || string.IsNullOrWhiteSpace(dto?.NewName))
                return BadRequest("Invalid data.");

            try
            {
                var updatedTopic = await topicService.UpdateTopicAsync(id, dto.NewName);
                return Ok(updatedTopic);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("Delete-Topic")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTopic(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid Topic ID.");

            try
            {
                var result = await topicService.DeleteTopicAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Search-Topics")]
        public async Task<IActionResult> SearchTopics([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be null or empty.");

            try
            {
                var topics = await topicService.SearchTopicsAsync(searchTerm);
                return Ok(topics);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-Topic-Count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTopicCount()
        {
            try
            {
                var count = await topicService.GetTopicCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
