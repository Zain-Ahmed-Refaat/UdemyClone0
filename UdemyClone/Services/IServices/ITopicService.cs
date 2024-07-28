using UdemyClone.Dto;
using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface ITopicService
    {
        Task<IEnumerable<Topic>> GetAllTopicsAsync(int pageNumber, int pageSize);
        Task<Topic> CreateTopicAsync(string name, Guid subCategoryId);
        Task<IEnumerable<Topic>> SearchTopicsAsync(string searchTerm);
        Task<Topic> UpdateTopicAsync(UpdateTopicDto updateTopicDto);
        Task<Topic> GetTopicByIdAsync(Guid id);
        Task<bool> TopicExistsAsync(Guid id);
        Task<bool> DeleteTopicAsync(Guid id);
        Task<int> GetTopicCountAsync();
    }
}
