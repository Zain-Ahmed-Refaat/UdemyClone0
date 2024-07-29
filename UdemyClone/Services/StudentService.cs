using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using UdemyClone.Data;
using UdemyClone.Dto;
using UdemyClone.Entities;
using UdemyClone.Services.IServices;

namespace UdemyClone.Services
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext context;

        public StudentService(ApplicationDbContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<StudentDto>> GetCourseEnrollmentsAsync(Guid courseId)
        {
            var enrollments = await context.StudentCourses
                .Where(sc => sc.CourseId == courseId)
                .Include(sc => sc.Student)
                .Select(sc => new StudentDto
                {
                    Id = sc.Student.Id,
                    UserName = sc.Student.UserName,
                    Email = sc.Student.Email
                })
                .ToListAsync();

            return enrollments;
        }

        public async Task<object> SearchCoursesAsync(string keyword, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                keyword = string.Empty;
            }

            var lowerKeyword = keyword.ToLower();

            var query = context.Courses
                .Where(c => c.Name.ToLower().Contains(lowerKeyword))
                .OrderBy(c => c.Name);

            var totalItems = await query.CountAsync();

            var courses = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description,
                    Topic = c.Topic.Name
                })
                .ToListAsync();

            return new
            {
                Courses = courses,
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }


        public async Task<LessonDto> GetLessonAsync(Guid studentId, Guid lessonId)
        {
            var lesson = await context.Lessons
                .Include(l => l.Quizzes)
                .FirstOrDefaultAsync(l => l.Id == lessonId);

            if (lesson is null)
            {
                throw new NotFoundException("Lesson not found.");
            }

            var isEnrolled = await context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId && sc.CourseId == lesson.CourseId);

            if (!isEnrolled)
            {
                throw new Exception("You Must Subscribe in This Course To Access This Lesson.");
            }

            if (lesson.Quizzes.Any())
            {
                foreach (var quiz in lesson.Quizzes)
                {
                    var quizResult = await context.StudentQuizzes
                        .FirstOrDefaultAsync(sq => sq.StudentId == studentId && sq.QuizId == quiz.Id);

                    if (quizResult == null || !quizResult.Passed)
                    {
                        throw new UnauthorizedAccessException("You Have To Pass The Quiz to Access This Lesson.");
                    }
                }
            }

            return new LessonDto
            {
                Name = lesson.Name,
                Description = lesson.Description,
                CourseId = lesson.CourseId
            };
        }

        private async Task<IEnumerable<Lesson>> GetLessonsByCourseAsync(Guid courseId, int pageNumber, int pageSize)
        {
            return await context.Lessons
                .Where(l => l.CourseId == courseId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        private async Task<bool> IsStudentEnrolledInCourseAsync(Guid studentId, Guid courseId)
        {
            var enrolledCourses = await GetCoursesByStudentAsync(studentId);
            return enrolledCourses.Any(c => c.Id == courseId);
        }

        public async Task<IEnumerable<LessonDto>> GetLessonsByCourseAsync(Guid studentId, Guid courseId, int pageNumber, int pageSize)
        {

            var isEnrolled = await IsStudentEnrolledInCourseAsync(studentId, courseId);
            if (!isEnrolled)
            {
                throw new UnauthorizedAccessException("You are not enrolled in this course.");
            }


            var lessons = await GetLessonsByCourseAsync(courseId, pageNumber, pageSize);
            return lessons.Select(l => new LessonDto
            {
                Name = l.Name,
                Description = l.Description,
                LessonId = l.Id,
                CourseId = l.CourseId
            });
        }


        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync(int pageNumber, int pageSize)
        {
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;

            var totalStudents = await (
                from user in context.Users
                join userRole in context.UserRoles on user.Id equals userRole.UserId
                join role in context.Roles on userRole.RoleId equals role.Id
                where role.Name == "Student"
                select user
            ).CountAsync();

            var totalPages = (int)Math.Ceiling((double)totalStudents / pageSize);
            if (pageNumber > totalPages) pageNumber = totalPages;

            return await (
                from user in context.Users
                join userRole in context.UserRoles on user.Id equals userRole.UserId
                join role in context.Roles on userRole.RoleId equals role.Id
                where role.Name == "Student"
                select new StudentDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Enrollments = user.Enrollments
                        .Select(e => new EnrollmentDto
                        {
                            CourseId = e.CourseId,
                            CourseName = e.Course != null ? e.Course.Name : "N/A"
                        })
                        .ToList()
                }
            )
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        }


        public async Task EnrollCourseAsync(Guid courseId, Guid userId)
        {
 
            var course = await context.Courses.FindAsync(courseId);
            var student = await context.Users.FindAsync(userId);

            if (course == null)
            {
                throw new ArgumentException("Course not found", nameof(courseId));
            }

            if (student == null)
            {
                throw new ArgumentException("Student not found", nameof(userId));
            }

            var existingEnrollment = await context.StudentCourses
                .AnyAsync(sc => sc.CourseId == courseId && sc.StudentId == userId);

            if (existingEnrollment)
            {
                throw new InvalidOperationException("Student is already enrolled in the course");
            }

            var studentCourse = new StudentCourse
            {
                StudentId = userId,
                CourseId = courseId
            };

            context.StudentCourses.Add(studentCourse);
            await context.SaveChangesAsync();
        }

        public async Task<string> UnenrollCourseAsync(Guid userId, string courseName)
        {
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.Name == courseName);

            if (course == null)
                return "Course not found.";

            var enrollment = await context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == userId && sc.CourseId == course.Id);

            if (enrollment == null)
                return "User is not enrolled in this course.";

            context.StudentCourses.Remove(enrollment);
            await context.SaveChangesAsync();

            return "Successfully unenrolled from the course.";
        }

        public async Task<StudentDto> GetStudentByIdAsync(Guid studentId)
        {
            var student = await context.Users
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return null;

            return new StudentDto
            {
                Id = student.Id,
                UserName = student.UserName,
                Email = student.Email,
                Enrollments = student.Enrollments.Select(e => new EnrollmentDto
                {
                    CourseId = e.CourseId,
                    CourseName = e.Course.Name
                }).ToList()
            };
        }

        public async Task<string> DeleteStudentAsync(Guid studentId)
        {
            var student = await context.Users.FindAsync(studentId);

            if (student == null)
                return "Student not found.";

            context.Users.Remove(student);
            await context.SaveChangesAsync();

            return "Student deleted successfully.";
        }

        public async Task<IEnumerable<CourseDto>> GetCoursesByStudentAsync(Guid studentId)
        {
            var student = await context.Users
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == studentId);

            if (student == null)
                return null;

            return student.Enrollments.Select(e => new CourseDto
            {
                Id = e.CourseId,
                Name = e.Course.Name,
                Description = e.Course.Description,
                InstructorId = e.Course.InstructorId   
            }).ToList();
        }

    }
}