# UdemyClone

Welcome to **UdemyClone**, a comprehensive .NET Core 7 Web API project replicating some key functionalities of an online learning platform like Udemy. This application is built to manage user roles, course creation, enrollment, quizzes, and more.

## Table of Contents
- [Project Overview](#project-overview)

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Architecture](#architecture)
- [Getting Started](#getting-started)
- [Installation](#installation)
- [Configuration](#configuration)
- [Database Setup](#database-setup)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Authentication and Authorization](#authentication-and-authorization)
- [User Management](#user-management)
- [Course Management](#course-management)
- [Student Enrollment](#student-enrollment)
- [Quiz Management](#quiz-management)
- [Error Handling](#error-handling)
- [Contributing](#contributing)
- [License](#license)

 
## Project Overview

  

**UdemyClone** is a RESTful API developed using ASP.NET Core 7. It is designed to mimic an online educational platform, allowing users to create courses, enroll students, manage lessons, and administer quizzes. The application is built with a focus on clean architecture, using service-oriented design to encapsulate business logic and ensure maintainability.

  

## Features

  

- **Role-based Authentication and Authorization**: Secure access to resources using JWT tokens with distinct roles for Admin, Instructor, and Student.

- **Course Management**: Create, update, delete, and retrieve courses with pagination and search capabilities.

- **Student Enrollment**: Enroll students in courses, view enrolled courses, and manage student data.

- **Quiz and Lesson Management**: Administer quizzes linked to lessons, check student quiz results, and control lesson access based on quiz completion.

- **Error Handling and Validation**: Comprehensive validation and error handling to ensure robust API interactions.

  

## Technology Stack

  

- **.NET Core 7**: Back-end framework

- **Entity Framework Core**: ORM for database operations

- **SQL Server**: Database engine

- **JWT**: Authentication and authorization

- **Swagger**: API documentation

- **AutoMapper**: Object-object mapping

  

## Architecture

  

The application follows a layered architecture approach:

  

- **Models**: Define database entities and DTOs.

- **Repositories**: Handle data access and manipulation.

- **Services**: Encapsulate business logic and operations.

- **Controllers**: Handle HTTP requests and responses.

- **Middlewares**: Manage cross-cutting concerns like authentication and error handling.

  

## Getting Started

  

Follow these steps to set up the project locally.

  

### Installation

  

1. **Clone the repository**

  

```bash

git clone https://github.com/Zain-Ahmed-Refaat/UdemyClone0.git

```

  

2. **Navigate to the project directory**

  

```bash

cd UdemyClone0

```

  

3. **Restore the dependencies**

  

```bash

dotnet restore

```

  

### Configuration

  

Configure your application settings in `appsettings.json`:

  

```json

{

"ConnectionStrings":  {

"DefaultConnection":  "Your SQL Server connection string here"

},

"JWT":  {

"Key":  "YourSecretKeyHere",

"Issuer":  "YourIssuerHere",

"Audience":  "YourAudienceHere",

	}

}

```

  

### Database Setup

  

Run the following command to apply migrations and update the database:

  

```bash

dotnet ef database update

```

  

This command will create the required database tables based on your Entity Framework Core models.

  

### Running the Application

  

Execute the following command to start the application:

  

```bash

dotnet run

```

  

The API will be available at `https://localhost:5001` or `http://localhost:5000`.

  

## API Documentation

  

### Authentication and Authorization

  

#### Register

  

- **Endpoint**: `POST /api/auth/register`

- **Description**: Register a new user with a specific role (Admin, Instructor, or Student).

- **Request Body**:

  

```json

{

"username": "user",

"email":  "user@example.com",

"password":  "YourPassword123!",

"ConfirmPassword": "YourPassword123!", 

"role":  1     // 1-user, 2-Admin, 3-Instructor, 4-Student

}

```

  

- **Response**:

  

```json

{

"userId":  "generated-user-id",

"email":  "user@example.com",

"roles":  "Instructor"

}

```

  

#### Login

  

- **Endpoint**: `POST /api/auth/login`

- **Description**: Authenticate a user and generate a JWT token.

- **Request Body**:

  

```json

{

"email":  "user@example.com",

"password":  "YourPassword123!"

}

```

  

- **Response**:

  

```json

{

"token":  "your-jwt-token"

}

```

  

### User Management

  

#### Get All Users (Admin Only)

  

- **Endpoint**: `GET /api/users`

- **Description**: Retrieve a list of all users.

- **Authorization**: Admin

- **Response**:

  

```json

[

{

"userId":  "user-id",

"email":  "user@example.com",

"role":  "Student"

}

]

```

  

### Course Management

  

#### Create a Course (Instructor Only)

  

- **Endpoint**: `POST /api/instructor/create`

- **Description**: Create a new course by an authenticated instructor.

- **Request Body**:

  

```json

{

"name":  "Course Name",

"description":  "Course Description",

"topicId":  "topic-id"

}

```

  

- **Response**:

  

```json

{

"courseId":  "generated-course-id",

"name":  "Course Name",

"description":  "Course Description",

"topicId":  "topic-id"

}

```

  

#### Update a Course (Instructor Only)

  

- **Endpoint**: `PUT /api/instructor/update/{id}`

- **Description**: Update course details if the instructor is the creator.

- **Request Body**:

  

```json

{

"name":  "Updated Course Name",

"description":  "Updated Description"

}

```

  

- **Response**:

  

```json

{

"courseId":  "course-id",

"name":  "Updated Course Name",

"description":  "Updated Description"

}

```

  

#### Get All Courses

  

- **Endpoint**: `GET /api/courses`

- **Description**: Retrieve a paginated list of all courses.

- **Query Parameters**:



- `page`: Page number (default: 1)

- `pageSize`: Number of courses per page (default: 10)

  

- **Response**:

 

```json

{

	"currentPage":  1,

	"totalPages":  5,

	"courses":  [

					{

						"courseId":  "course-id",

						"name":  "Course Name",

						"description":  "Course Description",

						"instructor":  "Instructor Name"

					}

				]

}

```

  

### Student Enrollment

  

#### Enroll in a Course (Student Only)

  

- **Endpoint**: `POST /api/student/enroll/{courseName}`

- **Description**: Enroll a student in a course using the course name.

- **Authorization**: Student

- **Response**:

 

```json

{

"studentId":  "student-id",

"courseName":  "Course Name",

"enrollmentDate":  "2024-07-27T12:34:56Z"

}

```
  

#### Get Enrolled Courses (Student Only)

  
- **Endpoint**: `GET /api/student/courses`

- **Description**: Retrieve a list of courses a student is enrolled in.

- **Authorization**: Student

- **Response**:

  

```json
[

	{

		"courseId":  "course-id",

		"name":  "Course Name",

		"description":  "Course Description"

	}

]
```


### Quiz Management

  

#### Create a Quiz (Instructor Only)

  

- **Endpoint**: `POST /api/quiz/create`

- **Description**: Create a new quiz for a course lesson.

- **Request Body**:

  

```json
{

	"lessonId":  "lesson-id",
	"questions":  [
					{
						"questionText":  "What is .NET Core?",
						"options":  [
										"A framework",
										"A library",
										"A database",
										"An IDE"
									],
						"correctOption":  "A framework"
					}
			]
}
```

- **Response**:

 
```json
{

"quizId":  "generated-quiz-id",

"lessonId":  "lesson-id",

"questions":  [...]

}

```

  

#### Submit Quiz Answers (Student Only)

  

- **Endpoint**: `POST /api/quiz/submit`

- **Description**: Submit answers to a quiz and receive a score.

- **Request Body**:

  

```json

{

"quizId":  "quiz-id",
"answers":  [
				{
					"questionId":  "question-id",
					"selectedOption":  "A framework"
				}
			]
}
```
- **Response**:
```json
{
"score": 80,
"passed":  true,
"dateTaken":  "2024-07-27T12:34:56Z"
}
```
#### Get Quiz Status (Student Only)

- **Endpoint**: `GET /api/student/quizstatus`

- **Description**: Retrieve a student's quiz status.

- **Authorization**: Student

- **Response**:

```json
[
	{
		"score":  80,
		"passed":  true,
		"dateTaken":  "2024-07-27T12:34:56Z"
	}
]
```  

## Error Handling

The application includes robust error handling with detailed messages and status codes:

- **400 Bad Request**: Validation errors or incorrect input data.

- **401 Unauthorized**: Authentication failure or missing token.

- **403 Forbidden**: Access to the resource is denied.

- **404 Not Found**: Resource not found.

- **500 Internal Server Error**: An unexpected server error occurred.

## Contributing

Contributions are welcome! Please fork the repository and submit a pull request for any enhancements, bug fixes, or improvements.

## License  

This project is licensed under the License - see the [LICENSE](LICENSE) file for details.
