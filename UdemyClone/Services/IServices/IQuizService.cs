using UdemyClone.Dto;
using UdemyClone.Models;

namespace UdemyClone.Services.IServices
{
    public interface IQuizService
    {
        Task<string> SubmitQuizAsync(Guid quizId, SubmitQuizRequest request, Guid studentId);
        Task<string> RetakeQuizAsync(Guid quizId, SubmitQuizRequest request, Guid studentId);
        Task<bool> IsInstructorOwnerOfQuizAsync(Guid instructorId, Guid quizId);
        Task<QuizResultDto> GetQuizResultAsync(Guid quizId, Guid studentId);
        Task<bool> CanStudentRetakeQuizAsync(Guid quizId, Guid studentId);
        Task<bool> HasStudentTakenQuizAsync(Guid studentId, Guid quizId);
        Task<bool> DidStudentPassQuizAsync(Guid studentId, Guid quizId);
        Task<List<QuizResultDto>> GetQuizResultsByIdAsync(Guid quizId);
        Task<QuizDto> GetQuizByIdAsync(Guid quizId, Guid StudentId);
        Task<Guid> GetQuizIdByLessonIdAsync(Guid lessonId);
        Task CreateQuizAsync(CreateQuizRequest request);
        Task DeleteQuizAsync(Guid quizId);
    }
}
