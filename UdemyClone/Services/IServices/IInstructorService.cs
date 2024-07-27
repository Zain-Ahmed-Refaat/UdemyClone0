using UdemyClone.Dto;
using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface IInstructorService
    {
        Task<(IEnumerable<Course> Courses, int TotalPages)> GetAllCoursesAsync(int pageNumber, int pageSize);
        Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<bool> EnrollInCourseAsync(Guid instructorId, Guid courseId);
        Task<bool> DeleteCourseAsync(Guid courseId, Guid instructorId);
        Task<Course> UpdateCourseAsync(CourseDto courseDto);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId);
        Task<Course> CreateCourseAsync(Course course);
    }
}
