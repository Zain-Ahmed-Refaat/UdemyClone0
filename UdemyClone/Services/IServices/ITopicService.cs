using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface ITopicService
    {
        Task<IEnumerable<Topic>> GetAllTopicsAsync(int pageNumber, int pageSize);
        Task<Topic> CreateTopicAsync(string name, Guid subCategoryId);
        Task<IEnumerable<Topic>> SearchTopicsAsync(string searchTerm);
        Task<Topic> UpdateTopicAsync(Guid id, string newName);
        Task<Topic> GetTopicByIdAsync(Guid id);
        Task<bool> TopicExistsAsync(Guid id);
        Task<bool> DeleteTopicAsync(Guid id);
        Task<int> GetTopicCountAsync();
    }
}
