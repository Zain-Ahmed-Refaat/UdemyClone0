using Microsoft.EntityFrameworkCore;
using UdemyClone.Data;
using UdemyClone.Entities;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext context;

        public CategoryService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Category> CreateCategoryAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty", nameof(categoryName));

            var category = new Category
            {
                Name = categoryName
            };

            await context.Categories.AddAsync(category);
            await context.SaveChangesAsync();

            return category;
        }
        public async Task<Category> GetCategoryByIdAsync(Guid id)
        {
            return await context.Categories.FindAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0)
                throw new ArgumentException("Page number must be greater than zero.", nameof(pageNumber));

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than zero.", nameof(pageSize));

            return await context.Categories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Category> UpdateCategoryAsync(Guid categoryId, string newCategoryName)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID must be a valid GUID.", nameof(categoryId));

            if (string.IsNullOrWhiteSpace(newCategoryName))
                throw new ArgumentException("Category name cannot be empty.", nameof(newCategoryName));

            var category = await context.Categories.FindAsync(categoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found.");

            category.Name = newCategoryName;

            context.Categories.Update(category);
            await context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> DeleteCategoryAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID must be a valid GUID.", nameof(categoryId));

            var category = await context.Categories.FindAsync(categoryId);
            if (category == null)
                throw new InvalidOperationException("Category not found.");

            context.Categories.Remove(category);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Category>> SearchCategoriesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await context.Categories.ToListAsync();
            }

            searchTerm = searchTerm.ToLower();

            return await context.Categories
                .Where(c => c.Name.ToLower().Contains(searchTerm))
                .ToListAsync();
        }


        public async Task<bool> CategoryExistsAsync(Guid categoryId)
        {
            if (categoryId == Guid.Empty)
                throw new ArgumentException("Category ID cannot be an empty GUID.", nameof(categoryId));

            return await context.Categories
                .AnyAsync(c => c.Id == categoryId);
        }

        public async Task<int> GetCategoryCountAsync()
        {
            return await context.Categories
                .CountAsync();
        }

    }
}
