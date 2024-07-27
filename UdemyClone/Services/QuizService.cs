using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using UdemyClone.Data;
using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Models;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext context;
        private readonly IQuizRepository _quizRepository;

        public QuizService(ApplicationDbContext context, IQuizRepository quizRepository)
        {
            this.context = context;
            this._quizRepository = quizRepository;
        }

        public async Task<bool> HasStudentTakenQuizAsync(Guid studentId, Guid quizId)
        {
            return await context.StudentQuizzes
                .AnyAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);
        }

        public async Task<bool> DidStudentPassQuizAsync(Guid studentId, Guid quizId)
        {
            var studentQuiz = await context.StudentQuizzes
                .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quizId);

            return studentQuiz?.Passed ?? false;
        }

        public async Task<Guid> GetQuizIdByLessonIdAsync(Guid lessonId)
        {
            var quiz = await context.Quizzes
                .FirstOrDefaultAsync(q => q.LessonId == lessonId);

            if (quiz == null)
            {
                throw new Exception("Quiz not found for the provided lesson ID.");
            }

            return quiz.Id;
        }

        public async Task<QuizDto> GetQuizByIdAsync(Guid quizId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
                throw new ArgumentNullException("Quiz not found.");

            return new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = quiz.Questions.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Answers = q.Answers.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList()
            };
        }

        public async Task CreateQuizAsync(CreateQuizRequest request)
        {
            // Prepare the questions list
            var questions = new List<Question>();

            foreach (var q in request.Questions)
            {
                // Create answers from request
                var answers = q.Answers.Select(a => new Answer
                {
                    Text = a.Text,
                    IsCorrect = a.IsCorrect

                }).ToList();

                // Check for exactly one correct answer
                var correctAnswers = answers.Where(a => a.IsCorrect).ToList();
                if (correctAnswers.Count != 1)
                {
                    throw new ArgumentException("Each question must have exactly one correct answer.");
                }


                var question = new Question
                {
                    Text = q.Text,
                    Answers = answers,
                    CorrectAnswerId = Guid.Empty
                };

                questions.Add(question);
            }

            var quiz = new Quiz
            {
                Title = request.Title,
                Description = request.Description,
                LessonId = request.LessonId,
                Questions = questions
            };

            await _quizRepository.AddAsync(quiz);

            // Update CorrectAnswerId after saving
            foreach (var question in quiz.Questions)
            {
                // Retrieve the correct answer ID from the question's answers
                var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
                if (correctAnswer != null)
                {
                    question.CorrectAnswerId = correctAnswer.Id;
                }
            }

            // Update the quiz with correct answer IDs
            await _quizRepository.UpdateAsync(quiz);
        }


        public async Task<string> SubmitQuizAsync(Guid quizId, SubmitQuizRequest request, Guid studentId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
                throw new NotFoundException("Quiz not found.");

            var studentQuiz = new StudentQuiz
            {
                Id = Guid.NewGuid(),
                StudentId = studentId,
                QuizId = quizId,
                DateTaken = DateTime.UtcNow,
                Score = 0,
                Passed = false
            };

            await _quizRepository.AddStudentQuizAsync(studentQuiz);

            int correctAnswers = 0;

            foreach (var answer in request.Answers)
            {
                var question = quiz.Questions.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null)
                    continue;

                bool isCorrect = question.CorrectAnswerId == answer.AnswerId;

                var studentAnswer = new StudentAnswer
                {
                    Id = Guid.NewGuid(),
                    StudentQuizId = studentQuiz.Id,
                    QuestionId = answer.QuestionId,
                    AnswerId = answer.AnswerId,
                };

                await _quizRepository.AddStudentAnswerAsync(studentAnswer);

                if (isCorrect)
                    correctAnswers++;
            }

            studentQuiz.Score = correctAnswers;
            studentQuiz.Passed = correctAnswers >= (quiz.Questions.Count * 0.7);

            await _quizRepository.UpdateStudentQuizAsync(studentQuiz);

            return $"Quiz Submitted Successfully\nCheck Your Results!";
        }


        public async Task<QuizResultDto> GetQuizResultAsync(Guid quizId, Guid studentId)
        {
            var studentQuiz = await _quizRepository.GetStudentQuizAsync(studentId, quizId);
            if (studentQuiz == null)
                throw new ArgumentNullException("Quiz result not found.");

            return new QuizResultDto
            {
                Score = studentQuiz.Score,
                Passed = studentQuiz.Passed,
                DateTaken = studentQuiz.DateTaken
            };
        }

        public async Task DeleteQuizAsync(Guid quizId)
        {
            await _quizRepository.DeleteQuizAsync(quizId);
        }

    }
}
