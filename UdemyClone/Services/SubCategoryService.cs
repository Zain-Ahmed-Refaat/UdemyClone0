using Microsoft.EntityFrameworkCore;
using UdemyClone.Data;
using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class SubCategoryService : ISubCategoryService
    {
        private readonly ApplicationDbContext context;

        public SubCategoryService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<SubCategory> CreateSubCategoryAsync(string name, Guid categoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("SubCategory name cannot be null or empty.");

            if (await context.Categories.FindAsync(categoryId) == null)
                throw new ArgumentException("Invalid Category ID.");

            var subCategory = new SubCategory
            {
                Id = Guid.NewGuid(),
                Name = name,
                CategoryId = categoryId
            };

            context.SubCategories.Add(subCategory);
            await context.SaveChangesAsync();

            return subCategory;
        }

        public async Task<SubCategory> GetSubCategoryByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid SubCategory ID.");

            return await context.SubCategories.FindAsync(id);
        }

        public async Task<IEnumerable<SubCategory>> GetAllSubCategoriesAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            return await context.SubCategories
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<SubCategory> UpdateSubCategoryAsync(SubCategoryUpdateDto updateDto)
        {
            var subCategory = await context.SubCategories
                .FirstOrDefaultAsync(sc => sc.Id == updateDto.Id);

            if (subCategory == null)
            {
                throw new ArgumentNullException("Not Found");
            }


            subCategory.Name = updateDto.Name;
            subCategory.CategoryId = updateDto.CategoryId;

            context.SubCategories.Update(subCategory);
            await context.SaveChangesAsync();

            return subCategory;
        }

        public async Task<bool> DeleteSubCategoryAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid SubCategory ID.");

            var subCategory = await context.SubCategories.FindAsync(id);
            if (subCategory == null)
                throw new KeyNotFoundException("SubCategory not found.");

            context.SubCategories.Remove(subCategory);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<SubCategory>> SearchSubCategoriesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be null or empty.");

            return await context.SubCategories
                .Where(sc => sc.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToListAsync();
        }


        public async Task<bool> SubCategoryExistsAsync(Guid id)
        {
            return await context.SubCategories.AnyAsync(sc => sc.Id == id);
        }

        public async Task<int> GetSubCategoryCountAsync()
        {
            return await context.SubCategories.CountAsync();
        }
    }
}
