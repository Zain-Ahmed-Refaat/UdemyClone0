﻿using Microsoft.EntityFrameworkCore;
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
        private readonly IStudentService studentService;

        public QuizService(ApplicationDbContext context, IQuizRepository quizRepository, IStudentService studentService)
        {
            this.context = context;
            this._quizRepository = quizRepository;
            this.studentService = studentService;
        }

        public async Task<bool> CanStudentRetakeQuizAsync(Guid quizId, Guid studentId)
        {

            var lastAttempt = await _quizRepository.GetLatestStudentQuizAttemptAsync(quizId, studentId);

            if (lastAttempt == null)
                return false; 

            return !lastAttempt.Passed;
        }


        public async Task<string> RetakeQuizAsync(Guid quizId, SubmitQuizRequest request, Guid studentId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
                throw new NotFoundException("Quiz not found.");

            var courseId = quiz.Lesson.CourseId;

            if (courseId == Guid.Empty)
                throw new ArgumentNullException(nameof(courseId), "Course ID cannot be empty.");


            bool isEnrolled = await IsStudentEnrolledInCourseAsync(studentId, courseId);
            if (!isEnrolled)
            {
                throw new Exception("You are not Enrolled in the Course Associated with This quiz.");
            }

            var lastAttempt = await _quizRepository.GetLatestStudentQuizAttemptAsync(quizId, studentId);
            if (lastAttempt == null || lastAttempt.Passed)
                throw new InvalidOperationException("You cannot retake this quiz.");

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

            return studentQuiz.Passed ? "You passed the quiz!" : "You did not pass the quiz.";
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

        public async Task<QuizDto> GetQuizByIdAsync(Guid quizId, Guid studentId)
        {
            if (quizId == Guid.Empty)
                throw new ArgumentNullException(nameof(quizId), "Quiz ID cannot be empty.");

            var quiz = await _quizRepository.GetByIdAsync(quizId);

            if (quiz == null)
                throw new ArgumentNullException(nameof(quiz), "Quiz not found.");

            if (quiz.Lesson == null)
                throw new ArgumentNullException(nameof(quiz.Lesson), "Lesson associated with the quiz cannot be null.");

            var courseId = quiz.Lesson.CourseId;

            if (courseId == Guid.Empty)
                throw new ArgumentNullException(nameof(courseId), "Course ID cannot be empty.");

            bool isEnrolled = await IsStudentEnrolledInCourseAsync(studentId, courseId);
            if (!isEnrolled)
            {
                throw new Exception("You are not Enrolled in the Course Associated with This quiz.");
            }

            return new QuizDto
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                Questions = quiz.Questions?.Select(q => new QuestionDto
                {
                    Id = q.Id,
                    Text = q.Text,
                    Answers = q.Answers?.Select(a => new AnswerDto
                    {
                        Id = a.Id,
                        Text = a.Text
                    }).ToList()
                }).ToList() ?? new List<QuestionDto>()
            };
        }

        private async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId)
        {
            if (studentId == Guid.Empty || courseId == Guid.Empty)
                return false;

            var enrolledCourses = await studentService.GetCoursesByStudentAsync(studentId);
            return enrolledCourses?.Any(c => c.Id == courseId) ?? false;
        }


        public async Task CreateQuizAsync(CreateQuizRequest request)
        {

            var questions = new List<Question>();

            foreach (var q in request.Questions)
            {

                var answers = q.Answers.Select(a => new Answer
                {
                    Text = a.Text,
                    IsCorrect = a.IsCorrect

                }).ToList();

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

            foreach (var question in quiz.Questions)
            {
                var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect);
                if (correctAnswer != null)
                {
                    question.CorrectAnswerId = correctAnswer.Id;
                }
            }

            await _quizRepository.UpdateAsync(quiz);
        }


        public async Task<string> SubmitQuizAsync(Guid quizId, SubmitQuizRequest request, Guid studentId)
        {
            var quiz = await _quizRepository.GetByIdAsync(quizId);
            if (quiz == null)
                throw new NotFoundException("Quiz not found.");

            var courseId = quiz.Lesson.CourseId;

            if (courseId == Guid.Empty)
                throw new ArgumentNullException(nameof(courseId), "Course ID cannot be empty.");


            bool isEnrolled = await IsStudentEnrolledInCourseAsync(studentId, courseId);
            if (!isEnrolled)
            {
                throw new Exception("You are not Enrolled in the Course Associated with This quiz.");
            }

            var lastAttempt = await _quizRepository.GetLatestStudentQuizAttemptAsync(quizId, studentId);

            if (lastAttempt != null && lastAttempt.Passed)
                return "You have already passed this quiz. No need to retake.";

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

            return "Quiz Submitted Successfully!\nCheck Your Results!";
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

        public async Task<bool> IsInstructorOwnerOfQuizAsync(Guid instructorId, Guid quizId)
        {
            var quiz = await context.Quizzes
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null)
            {
                return false;
            }

            return quiz.Lesson.Course.InstructorId == instructorId;
        }

        public async Task<List<QuizResultDto>> GetQuizResultsByIdAsync(Guid quizId)
        {
            var quizResults = await context.StudentQuizzes
                .Where(sq => sq.QuizId == quizId)
                .Select(sq => new QuizResultDto
                {
                    StudentId = sq.StudentId,
                    Score = sq.Score,
                    Passed = sq.Passed,
                    DateTaken = sq.DateTaken
                })
                .ToListAsync();

            return quizResults;
        }


        public async Task DeleteQuizAsync(Guid quizId)
        {
            await _quizRepository.DeleteQuizAsync(quizId);
        }

    }
}
