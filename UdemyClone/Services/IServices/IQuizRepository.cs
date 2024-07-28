using UdemyClone.Entities;

namespace UdemyClone.Services.IServices
{
    public interface IQuizRepository
    {
        Task<StudentQuiz> GetLatestStudentQuizAttemptAsync(Guid quizId, Guid studentId);
        Task<StudentQuiz> GetStudentQuizAsync(Guid studentId, Guid quizId);
        Task AddStudentAnswerAsync(StudentAnswer studentAnswer);
        Task UpdateStudentQuizAsync(StudentQuiz studentQuiz);
        Task AddStudentQuizAsync(StudentQuiz studentQuiz);
        Task<Quiz> GetByIdAsync(Guid quizId);
        Task DeleteQuizAsync(Guid quizId);
        Task UpdateAsync(Quiz quiz);
        Task AddAsync(Quiz quiz);
        Task SaveChangesAsync();

    }

}
