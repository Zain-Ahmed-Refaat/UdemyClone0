using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService) 
        {
            this.categoryService = categoryService;
        }


        [HttpPost("Create-Category")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            try
            {
                var category = await categoryService.CreateCategoryAsync(categoryName);
                return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, category);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-Category-By-Id")]
        [Authorize]
        public async Task<IActionResult> GetCategoryById(Guid id)
        {
            var category = await categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpGet("Get-All-Categories")]
        public async Task<IActionResult> GetAllCategories(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var categories = await categoryService.GetAllCategoriesAsync(pageNumber, pageSize);
                return Ok(categories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update-Category")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(Guid categoryId, string newCategoryName)
        {
            try
            {
                var updatedCategory = await categoryService.UpdateCategoryAsync(categoryId, newCategoryName);
                return Ok(updatedCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("Search-Categories")]
        public async Task<IActionResult> SearchCategories([FromQuery] string searchTerm)
        {
            try
            {
                var categories = await categoryService.SearchCategoriesAsync(searchTerm);
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("Category-Exists")]
        public async Task<IActionResult> CategoryExists(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid category ID.");

            var exists = await categoryService.CategoryExistsAsync(id);
            return Ok(new { Exists = exists });
        }

        [HttpGet("Get-Category-Count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCategoryCount()
        {
            var count = await categoryService.GetCategoryCountAsync();
            return Ok(new { Count = count });
        }

        [HttpDelete("Delete-Category")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(Guid categoryId)
        {
            try
            {
                var result = await categoryService.DeleteCategoryAsync(categoryId);
                return result ? Ok("Category successfully deleted.") : BadRequest("Failed to delete category.");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

    }
}
