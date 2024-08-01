using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Models;

namespace UdemyClone.Services.IServices
{
    public interface IInstructorService
    {
        Task<(IEnumerable<CourseDto> Courses, int TotalPages)> GetAllCoursesAsync(int pageNumber, int pageSize);
        Task<IEnumerable<CourseEnrollmentsDto>> GetInstructorCoursesEnrollmentsAsync(Guid instructorId);
        Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(Guid instructorId);
        Task<Course> UpdateCourseAsync(Guid Id, CourseModel model, Guid instructorId);
        Task<bool> EnrollInCourseAsync(Guid instructorId, Guid courseId);
        Task<bool> DeleteCourseAsync(Guid courseId, Guid instructorId);
        Task<CourseDto> GetCourseByIdAsync(Guid courseId, Guid InstructorId, string userRole);
        Task<Course> CreateCourseAsync(Course course);
    }
}
