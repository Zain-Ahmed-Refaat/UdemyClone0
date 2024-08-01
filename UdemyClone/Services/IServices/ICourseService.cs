using UdemyClone.Dto;
using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface ICourseService
    {
        Task<IEnumerable<dynamic>> GetAllLessonsAsync(Guid instructorId, Guid courseId, int pageNumber, int pageSize);
        Task<Lesson> UploadLessonAsync(LessonDto model, Guid instructorId);
        Task<Lesson> GetLessonByIdAsync(Guid id, Guid instructorId);
    }
}
