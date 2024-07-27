using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize);
        Task<Category> UpdateCategoryAsync(Guid categoryId, string newCategoryName);
        Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm);
        Task<Category> CreateCategoryAsync(string categoryName);
        Task<bool> DeleteCategoryAsync(Guid categoryId);
        Task<bool> CategoryExistsAsync(Guid categoryId);
        Task<Category> GetCategoryByIdAsync(Guid id);
        Task<int> GetCategoryCountAsync();

    }
}
