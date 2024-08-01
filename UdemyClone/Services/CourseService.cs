using Microsoft.EntityFrameworkCore;
using UdemyClone.Data;
using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDbContext context;

        public CourseService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Lesson> UploadLessonAsync(LessonDto model, Guid instructorId)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model), "Lesson model cannot be null.");

            if (model.CourseId == Guid.Empty)
                throw new ArgumentException("Course ID is required.", nameof(model.CourseId));

            var course = await context.Courses
                .Include(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Id == model.CourseId);

            if (course == null)
                throw new ArgumentException("Course not found.", nameof(model.CourseId));

            if (course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You do not have permission to add lessons to this course.");

            var existingLesson = await context.Lessons
                .FirstOrDefaultAsync(l => l.Name == model.Name && l.CourseId == model.CourseId);

            if (existingLesson != null)
                throw new InvalidOperationException($"A lesson with the name '{model.Name}' already exists in this course.");


            var lesson = new Lesson
            {
                Name = model.Name,
                Description = model.Description,
                CourseId = model.CourseId,
            };

            context.Lessons.Add(lesson);
            await context.SaveChangesAsync();

            return lesson;
        }
        public async Task<IEnumerable<dynamic>> GetAllLessonsAsync(Guid instructorId, Guid courseId, int pageNumber, int pageSize)
        {

            if (instructorId == Guid.Empty)
                throw new ArgumentNullException(nameof(instructorId), "Instructor ID cannot be empty.");

            if (courseId == Guid.Empty)
                throw new ArgumentNullException(nameof(courseId), "Course ID cannot be empty.");

            if (pageNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageNumber), "Page number must be greater than zero.");

            if (pageSize <= 0)
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be greater than zero.");


            var course = await context.Courses
                .Where(c => c.InstructorId == instructorId && c.Id == courseId)
                .Select(c => c.Id)
                .FirstOrDefaultAsync();

            if (course == Guid.Empty)
                throw new InvalidOperationException("Course not found or does not belong to the instructor.");

            var lessons = await context.Lessons
                .Where(l => l.CourseId == course)
                .Include(l => l.Course)
                .Include(l => l.Quizzes)
                .OrderBy(l => l.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new
                {
                    l.Id,l.Name,l.Description,l.CourseId,CourseName = l.Course.Name,Quizzes = l.Quizzes.Select(q =>new
                    {
                        q.Id,
                        q.Description
                    })
                })
                .ToListAsync();

            if (lessons == null || !lessons.Any())
                return new List<dynamic>();

            return lessons;
        }
        public async Task<Lesson> GetLessonByIdAsync(Guid id, Guid instructorId)
        {
            var lesson = await context.Lessons
                .Include(l => l.Course.Name)
                .Include(l => l.Quizzes)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (lesson == null)
                throw new ArgumentNullException("Lesson Not Found");

            if (lesson.Course.InstructorId != instructorId)
                throw new UnauthorizedAccessException("You Do Not Have Permission to Access this Lesson.");

            return lesson;
        }

    }
}
