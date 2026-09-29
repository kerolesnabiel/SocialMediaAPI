# Social Media API

![Build and Deploy](https://github.com/kerolesnabiel/SocialMediaAPI/actions/workflows/deploy-to-monster-asp.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-10-512BD4)

A RESTful API for a social media platform built with ASP.NET Core, following Clean Architecture principles. It provides user authentication, post management, comments, likes, a follow system, search, and admin controls.

## Project Overview

This API serves as the backend for a social media platform, providing functionality for user authentication, content creation, user interaction, and administrative control. The project is organized into distinct Domain, Application, Infrastructure, and API layers, with CQRS (via MediatR) separating reads from writes in the Application layer.

## Features

- **User Management**
  - Registration, login (issuing bearer tokens via ASP.NET Identity), and token refresh.
  - Profile updates, including profile picture upload.
  - Change password.

- **Posts**
  - CRUD operations for posts, including image uploads.
  - Like/unlike functionality.
  - Feed generation from followed users.

- **Comments**
  - CRUD operations for comments on posts.
  - Like/unlike functionality.

- **Search**
  - Search posts by keyword.
  - Search users by username or name.

- **Follow System**
  - Follow and unfollow users.
  - View followers and following lists.

- **Admin Controls**
  - Assign or remove user roles.
  - Delete any user, post, or comment.

## Architecture

- **Clean Architecture** across four projects: `SocialMediaDomain`, `SocialMediaApplication`, `SocialMediaInfrastructure`, and `SocialMediaAPI`.
- **CQRS** via MediatR, with commands and queries handled independently per feature.
- **Validation** through FluentValidation validators, run automatically as a MediatR pipeline behavior before each handler executes.
- **Centralized error handling** via a `GlobalExceptionHandler` (`IExceptionHandler`) that maps domain exceptions to RFC 7807 `ProblemDetails` responses.

## Technologies Used

- **Framework**: ASP.NET Core 10 Web API
- **Database**: SQL Server with Entity Framework Core
- **Authentication**: ASP.NET Core Identity with bearer tokens
- **File Storage**: Azure Blob Storage (post images, profile pictures)
- **Libraries and Tools**:
  - `MediatR` — CQRS / mediator pattern.
  - `Mapster` — Object mapping.
  - `FluentValidation` — Request validation.
  - `Serilog` — Structured logging.
  - `Swashbuckle.AspNetCore` — Swagger / OpenAPI documentation.
  - `xUnit`, `Moq` — Testing.

## API Endpoints

### Posts

- `POST /api/posts` — Create a new post.
- `GET /api/posts` — Get all posts.
- `GET /api/posts/feed` — Get posts from followed users.
- `GET /api/posts/{id}` — Get a post by ID.
- `PATCH /api/posts/{id}` — Update a post.
- `DELETE /api/posts/{id}` — Delete a post.
- `POST /api/posts/{postId}/like` — Like/unlike a post.
- `GET /api/posts/{postId}/likes` — Get a list of users who liked a post.

### Comments

- `POST /api/posts/{postId}/comments` — Add a comment to a post.
- `GET /api/posts/{postId}/comments` — Get all comments on a post.
- `PATCH /api/posts/{postId}/comments/{commentId}` — Update a comment.
- `DELETE /api/posts/{postId}/comments/{commentId}` — Delete a comment.
- `POST /api/posts/{postId}/comments/{commentId}/like` — Like/unlike a comment.
- `GET /api/posts/{postId}/comments/{commentId}/likes` — Get a list of users who liked a comment.

### Users

- `POST /api/users/register` — Register a new user.
- `POST /api/users/login` — Log in and receive a bearer token.
- `POST /api/users/refresh` — Refresh the bearer token.
- `GET /api/users/{id}` — Get a user profile by ID.
- `PATCH /api/users/me` — Update the current user's profile.
- `DELETE /api/users/me` — Delete the current user's account.
- `POST /api/users/{id}/follow` — Follow a user.
- `DELETE /api/users/{id}/unfollow` — Unfollow a user.
- `POST /api/users/manage/info` — Change the password.

### Admin

- `POST /api/admin/userRole` — Assign a role to a user.
- `DELETE /api/admin/userRole` — Remove a role from a user.
- `GET /api/admin/users` — Get all users.
- `DELETE /api/admin/users/{userId}` — Delete a user.
- `DELETE /api/admin/posts/{postId}` — Delete a post.

### Search

- `GET /api/search/posts` — Search posts by keyword.
- `GET /api/search/users` — Search users by username or name.

## Live Demo

- On MonsterASP.NET: http://social-media.runasp.net/swagger

## CI/CD

Every push to `master` triggers a GitHub Actions workflow that restores, builds, and tests the solution, then publishes and deploys the API to MonsterASP.NET. See [`.github/workflows/deploy-to-monster-asp.yml`](.github/workflows/deploy-to-monster-asp.yml).

## Setup Instructions

1. **Clone the repository**:

   ```bash
   git clone https://github.com/kerolesnabiel/SocialMediaAPI.git
   cd SocialMediaAPI
   ```

2. **Install dependencies**:

   ```bash
   dotnet restore
   ```

3. **Configure connection strings**:

   Add a `SocialMediaDb` (SQL Server) and `BlobStorage` (Azure Blob Storage) connection string to `SocialMediaAPI/appsettings.Development.json`:

   ```json
   {
     "ConnectionStrings": {
       "SocialMediaDb": "Server=localhost;Database=SocialMediaDb;Integrated Security=SSPI;TrustServerCertificate=True",
       "BlobStorage": "<your Azure Storage connection string>"
     }
   }
   ```

4. **Run the application**:

   ```bash
   dotnet run --project SocialMediaAPI
   ```

   Pending EF Core migrations are applied automatically on startup.

5. **Access Swagger UI**:

   Open `https://localhost:{port}/swagger` in your browser. Swagger UI is only enabled when running in the `Development` environment.

## Testing

To run the tests:

```bash
dotnet test
```
