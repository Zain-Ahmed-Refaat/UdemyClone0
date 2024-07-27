using UdemyClone.Dto;
using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface ICourseService
    {
        Task<IEnumerable<Lesson>> GetAllLessonsAsync(Guid instructorId, int pageNumber, int pageSize);
        Task<Lesson> UploadLessonAsync(LessonDto model, Guid instructorId);
        Task<Lesson> GetLessonByIdAsync(Guid id, Guid instructorId);
    }
}
