using UdemyClone.Dto;
using UdemyClone.Models;

namespace UdemyClone.Services.IServices
{
    public interface IQuizService
    {
        Task<string> SubmitQuizAsync(Guid quizId, SubmitQuizRequest request, Guid studentId);
        Task<QuizResultDto> GetQuizResultAsync(Guid quizId, Guid studentId);
        Task<bool> CanStudentRetakeQuizAsync(Guid quizId, Guid studentId);
        Task<bool> HasStudentTakenQuizAsync(Guid studentId, Guid quizId);
        Task<bool> DidStudentPassQuizAsync(Guid studentId, Guid quizId);
        Task<string> RetakeQuizAsync(Guid quizId, Guid studentId);
        Task<Guid> GetQuizIdByLessonIdAsync(Guid lessonId);
        Task CreateQuizAsync(CreateQuizRequest request);
        Task<QuizDto> GetQuizByIdAsync(Guid quizId);
        Task DeleteQuizAsync(Guid quizId);
    }
}
