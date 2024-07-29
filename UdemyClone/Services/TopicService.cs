using Microsoft.EntityFrameworkCore;
using UdemyClone.Data;
using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class TopicService : ITopicService
    {
        private readonly ApplicationDbContext context;

        public TopicService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Topic> CreateTopicAsync(string name, Guid subCategoryId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Topic name cannot be null or empty.");

            if (await context.SubCategories.FindAsync(subCategoryId) == null)
                throw new ArgumentException("Invalid SubCategory ID.");

            var topic = new Topic
            {
                Id = Guid.NewGuid(),
                Name = name,
                SubCategoryId = subCategoryId
            };

            context.Topics.Add(topic);
            await context.SaveChangesAsync();

            return topic;
        }

        public async Task<Topic> GetTopicByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid Topic ID.");

            return await context.Topics.FindAsync(id);
        }

        public async Task<IEnumerable<Topic>> GetAllTopicsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1 || pageSize < 1)
                throw new ArgumentException("Page number and page size must be greater than zero.");

            return await context.Topics
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Topic> UpdateTopicAsync(UpdateTopicDto updateTopicDto)
        {

            var topic = await context.Topics
                .FirstOrDefaultAsync(t => t.Id == updateTopicDto.Id);

            if (topic == null)
            {
                return null;
            }

            topic.Name = updateTopicDto.Name;
            topic.SubCategoryId = updateTopicDto.SubCategoryId;

            context.Topics.Update(topic);
            await context.SaveChangesAsync();

            return topic;
        }

        public async Task<bool> DeleteTopicAsync(Guid id)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Invalid Topic ID.");

            var topic = await context.Topics.FindAsync(id);
            if (topic == null)
                throw new KeyNotFoundException("Topic not found.");

            context.Topics.Remove(topic);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<Topic>> SearchTopicsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                throw new ArgumentException("Search term cannot be null or empty.");

            return await context.Topics
                .Where(t => t.Name.ToLower().Contains(searchTerm.ToLower()))
                .ToListAsync();
        }


        public async Task<bool> TopicExistsAsync(Guid id)
        {
            return await context.Topics.AnyAsync(t => t.Id == id);
        }

        public async Task<int> GetTopicCountAsync()
        {
            return await context.Topics.CountAsync();
        }
    }
}
