using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyClone.Dto;
using UdemyClone.Services.IServices;

namespace UdemyClone.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
        private readonly ISubCategoryService subCategoryService;

        public SubCategoryController(ISubCategoryService subCategoryService)
        {
            this.subCategoryService = subCategoryService;
        }

        [HttpPost("Create-SubCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubCategory([FromBody] CreateSubCategoryDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Name) || dto.CategoryId == Guid.Empty)
                return BadRequest("Invalid data.");

            try
            {
                var subCategory = await subCategoryService.CreateSubCategoryAsync(dto.Name, dto.CategoryId);
                return CreatedAtAction(nameof(GetSubCategoryById), new { id = subCategory.Id }, subCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-SubCategory-By-Id")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSubCategoryById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid SubCategory ID.");

            try
            {
                var subCategory = await subCategoryService.GetSubCategoryByIdAsync(id);
                if (subCategory == null)
                    return NotFound("SubCategory not found.");

                return Ok(subCategory);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-All-SubCategories")]
        public async Task<IActionResult> GetAllSubCategories(int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1 || pageSize < 1)
                return BadRequest("Page number and page size must be greater than zero.");

            try
            {
                var subCategories = await subCategoryService.GetAllSubCategoriesAsync(pageNumber, pageSize);
                return Ok(subCategories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("Update-SubCategory")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> UpdateSubCategory([FromBody] SubCategoryUpdateDto updateDto)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedSubCategory = await subCategoryService.UpdateSubCategoryAsync(updateDto);

                if (updatedSubCategory == null)
                {
                    return NotFound("SubCategory not found.");
                }

                return Ok(updatedSubCategory);
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

        [HttpGet("Search-SubCategories")]
        public async Task<IActionResult> SearchSubCategories([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be null or empty.");

            try
            {
                var subCategories = await subCategoryService.SearchSubCategoriesAsync(searchTerm);
                return Ok(subCategories);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Get-SubCategory-Count")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSubCategoryCount()
        {
            try
            {
                var count = await subCategoryService.GetSubCategoryCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Delete-SubCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubCategory(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Invalid SubCategory ID.");

            try
            {
                var result = await subCategoryService.DeleteSubCategoryAsync(id);
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

    }
}
