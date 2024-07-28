using Microsoft.EntityFrameworkCore;
using UdemyClone.Data;
using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Models;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly ApplicationDbContext context;

        public InstructorService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<Course> CreateCourseAsync(Course course)
        {
            if (course.InstructorId == Guid.Empty)
            {
                throw new ArgumentException("Instructor ID cannot be empty.", nameof(course.InstructorId));
            }

            var existingTopic = await context.Topics.FindAsync(course.TopicId);
            if (existingTopic == null)
            {
                throw new KeyNotFoundException("Topic not found.");
            }

            context.Courses.Add(course);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error saving the course to the database.", ex);
            }

            return course;
        }

        public async Task<Course> UpdateCourseAsync(CourseModel model, Guid instructorId)
        {
            var course = await context.Courses
                .Where(c => c.Id == model.Id && c.InstructorId == instructorId)
                .FirstOrDefaultAsync();

            if (course == null)
                return null;

            course.Name = model.Name;
            course.Description = model.Description;

            context.Courses.Update(course);
            await context.SaveChangesAsync();

            return course;
        }


        public async Task<IEnumerable<CourseEnrollmentsDto>> GetInstructorCoursesEnrollmentsAsync(Guid instructorId)
        {
            var courses = await context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Include(c => c.StudentCourses) 
                .ThenInclude(sc => sc.Student)
                .ToListAsync();

            // Map the data to the DTO
            var courseEnrollments = courses.Select(course => new CourseEnrollmentsDto
            {
                CourseId = course.Id,
                CourseName = course.Name,
                Enrollments = course.StudentCourses.Select(sc => new StudentDto
                {
                    Id = sc.Student.Id,
                    UserName = sc.Student.UserName,
                    Email = sc.Student.Email
                }).ToList()
            }).ToList();

            return courseEnrollments;
        }


        public async Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(Guid instructorId)
        {
            return await context.Courses
                .Where(c => c.InstructorId == instructorId)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    InstructorId = instructorId,
                    Topic = c.Topic.Name
                })
                .ToListAsync();
        }

        public async Task<(IEnumerable<Course> Courses, int TotalPages)> GetAllCoursesAsync(int pageNumber, int pageSize)
        {
            var totalCourses = await context.Courses.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCourses / (double)pageSize);

            var courses = await context.Courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (courses, totalPages);
        }

        public async Task<bool> DeleteCourseAsync(Guid courseId, Guid instructorId)
        {
            var course = await context.Courses
                .Where(c => c.Id == courseId && c.InstructorId == instructorId)
                .FirstOrDefaultAsync();

            if (course == null)
                return false;

            context.Courses.Remove(course);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EnrollInCourseAsync(Guid instructorId, Guid courseId)
        {

            var course = await context.Courses.FindAsync(courseId);

            if (course == null || course.InstructorId == instructorId)
                return false;

            return true; 
        }

        public async Task<CourseDto> GetCourseByIdAsync(Guid courseId)
        {
            var course = await context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description
                })
                .FirstOrDefaultAsync();

            return course;
        }
    }
}
