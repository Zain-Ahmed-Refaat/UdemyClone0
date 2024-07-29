using UdemyClone.Dto;

namespace UdemyClone.Services.IServices
{
    public interface IStudentService
    {
        Task<IEnumerable<LessonDto>> GetLessonsByCourseAsync(Guid studentId, Guid courseId, int pageNumber, int pageSize);
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync(int pageNumber, int pageSize);
        Task<dynamic> SearchCoursesAsync(string keyword, int pageNumber, int pageSize);
        Task<IEnumerable<StudentDto>> GetCourseEnrollmentsAsync(Guid courseId);
        Task<IEnumerable<CourseDto>> GetCoursesByStudentAsync(Guid studentId);
        Task<string> UnenrollCourseAsync(Guid userId, string courseName);
        Task<LessonDto> GetLessonAsync(Guid studentId, Guid lessonId);
        Task<StudentDto> GetStudentByIdAsync(Guid studentId);
        Task EnrollCourseAsync(Guid courseId, Guid userId);
        Task<string> DeleteStudentAsync(Guid studentId);
    }
}
