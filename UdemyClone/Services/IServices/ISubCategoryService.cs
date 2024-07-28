using UdemyClone.Dto;
using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface ISubCategoryService
    {
        Task<IEnumerable<SubCategory>> GetAllSubCategoriesAsync(int pageNumber, int pageSize);
        Task<IEnumerable<SubCategory>> SearchSubCategoriesAsync(string searchTerm);
        Task<SubCategory> CreateSubCategoryAsync(string name, Guid categoryId);
        Task<SubCategory> UpdateSubCategoryAsync(SubCategoryUpdateDto updateDto);
        Task<SubCategory> GetSubCategoryByIdAsync(Guid id);
        Task<bool> SubCategoryExistsAsync(Guid id);
        Task<bool> DeleteSubCategoryAsync(Guid id);
        Task<int> GetSubCategoryCountAsync();
    }
}
