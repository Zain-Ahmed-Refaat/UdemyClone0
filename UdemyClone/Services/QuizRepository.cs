using Microsoft.EntityFrameworkCore;
using UdemyClone.Data;
using UdemyClone.Entities;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class QuizRepository : IQuizRepository
    {
         private readonly ApplicationDbContext _context;

         public QuizRepository(ApplicationDbContext context)
         {
             _context = context;
         }

        public async Task AddAsync(Quiz quiz)
        {
            // Add quiz
            await _context.Quizzes.AddAsync(quiz);
            await _context.SaveChangesAsync(); // Ensure IDs are generated
        }

        public async Task UpdateAsync(Quiz quiz)
        {
            // Update quiz
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync(); // Save updated changes
        }

        public async Task<Quiz> GetByIdAsync(Guid id)
        {
            // Retrieve quiz by ID
            return await _context.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task AddStudentQuizAsync(StudentQuiz studentQuiz)
        {
            // Add student quiz
            await _context.StudentQuizzes.AddAsync(studentQuiz);
            await _context.SaveChangesAsync();
        }

        public async Task AddStudentAnswerAsync(StudentAnswer studentAnswer)
        {
            // Add student answer
            await _context.StudentAnswers.AddAsync(studentAnswer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStudentQuizAsync(StudentQuiz studentQuiz)
        {
            // Update student quiz
            _context.StudentQuizzes.Update(studentQuiz);
            await _context.SaveChangesAsync();
        }

        public async Task<StudentQuiz> GetStudentQuizAsync(Guid studentId, Guid quizId)
        {
             return await _context.StudentQuizzes
                 .Include(sq => sq.StudentAnswers)
                 .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);
        }

        public async Task DeleteQuizAsync(Guid quizId)
        {
            var quiz = await _context.Quizzes.FindAsync(quizId);
            if (quiz == null)
            {
                throw new KeyNotFoundException("Quiz not found");
            }

            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

    }

} 
