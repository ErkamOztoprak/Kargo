# Kargo Backend 📦

ASP.NET Core 6 RESTful API for the Kargo parcel delivery platform. Provides user authentication, parcel management, delivery tracking, and trust scoring system with MSSQL database and Swagger documentation.

## Quick Start

```bash
cd KargoBackEnd
dotnet restore
dotnet ef database update
dotnet run
```

Backend: `http://localhost:5000`
Swagger: `http://localhost:5000/swagger`

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Database Schema](#database-schema)
- [Development](#development)
- [Deployment](#deployment)

## Features

✨ **Authentication & Authorization**
- User registration and login
- JWT token-based authentication
- Secure password hashing
- Token refresh mechanism
- Role-based access control

📦 **Parcel Management**
- Create and manage parcel listings
- Track parcel status
- Search and filter parcels
- Update delivery information
- Archive completed deliveries

⭐ **Trust & Rating System**
- Calculate user trust scores
- Rate couriers and senders
- Maintain delivery history
- Display user ratings
- Recommended courier system

🔐 **Security**
- JWT token validation
- CORS protection
- Input validation
- SQL injection prevention
- Secure API endpoints

## Technology Stack

- **Framework**: ASP.NET Core 6
- **Language**: C#
- **Database**: MSSQL (Microsoft SQL Server)
- **ORM**: Entity Framework Core 6
- **Authentication**: JWT Bearer Tokens
- **API Documentation**: Swagger/OpenAPI
- **API Pattern**: RESTful API

## Installation

### Requirements

- .NET SDK 6 or higher
- SQL Server (Express or Full)
- Visual Studio or VS Code

### Steps

```bash
# Clone repository
git clone https://github.com/ErkamOztoprak/Kargo.git
cd Kargo/KargoBackEnd

# Restore dependencies
dotnet restore

# Update database
dotnet ef database update

# Run application
dotnet run
```

## Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=KargoDB;User Id=sa;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "Key": "your-secret-key-minimum-32-characters-long",
    "Issuer": "kargo-app",
    "Audience": "kargo-users",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### appsettings.Development.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=KargoDB_Dev;Trusted_Connection=true;"
  },
  "Jwt": {
    "Key": "dev-secret-key-minimum-32-characters-long",
    "Issuer": "http://localhost:5000",
    "Audience": "http://localhost:4200",
    "ExpirationMinutes": 120
  }
}
```

## API Endpoints

### 🔓 Authentication (Public)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/refresh` | Refresh JWT token |

### 🔐 Authentication (Protected)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/logout` | User logout |
| GET | `/api/auth/profile` | Get current user profile |

## Request/Response Examples

### Register

**Request:**
```json
POST /api/auth/register
{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePass123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Success Response (200):**
```json
{
  "id": "guid-string",
  "username": "johndoe",
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "trustScore": 0
}
```

### Login

**Request:**
```json
POST /api/auth/login
{
  "email": "john@example.com",
  "password": "SecurePass123!"
}
```

**Success Response (200):**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresIn": 3600,
  "user": {
    "id": "guid-string",
    "username": "johndoe",
    "email": "john@example.com"
  }
}
```

### Parcel Endpoints

**Get All Parcels:**
```json
GET /api/parcels
Response: [{ id, description, weight, status, senderId, receiverId... }]
```

**Create Parcel:**
```json
POST /api/parcels
{
  "description": "Electronics package",
  "weight": 2.5,
  "pickupLocation": "Main Gate",
  "deliveryLocation": "Dorm A, Room 101"
}
```

**Get Parcel Details:**
```json
GET /api/parcels/{id}
Response: { id, description, weight, status, createdDate... }
```

**Update Parcel:**
```json
PUT /api/parcels/{id}
{
  "status": "InTransit",
  "currentLocation": "Campus Center"
}
```

### Delivery Endpoints

**Get All Deliveries:**
```json
GET /api/deliveries
Response: [{ id, parcelId, deliverId, startDate, endDate, status... }]
```

**Create Delivery:**
```json
POST /api/deliveries
{
  "parcelId": "parcel-guid",
  "description": "Ready to deliver"
}
```

**Update Delivery Status:**
```json
PUT /api/deliveries/{id}
{
  "status": "Completed",
  "completionDate": "2026-02-28T15:30:00Z"
}
```

### User Endpoints

**Get User Profile:**
```json
GET /api/users/{id}
Authorization: Bearer TOKEN
Response: { id, username, email, firstName, lastName, trustScore, createdAt... }
```

**Get User History:**
```json
GET /api/users/{id}/history
Response: [{ deliveryId, parcelDescription, status, date... }]
```

**Get Trust Score:**
```json
GET /api/users/{id}/trust-score
Response: { score: 4.5, totalRatings: 12, averageRating: 4.5 }
```

### Rating Endpoints

**Create Rating:**
```json
POST /api/ratings
{
  "ratedUserId": "user-guid",
  "score": 5,
  "comment": "Great service!"
}
```

**Get User Ratings:**
```json
GET /api/ratings/user/{userId}
Response: [{ id, raterId, raterName, score, comment, createdAt... }]
```

## HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | OK - Successful request |
| 201 | Created - Resource created |
| 400 | Bad Request - Invalid data |
| 401 | Unauthorized - Invalid token |
| 403 | Forbidden - Access denied |
| 404 | Not Found - Resource not found |
| 409 | Conflict - Duplicate resource |
| 500 | Server Error |

## Database Schema

### Users Table
```sql
CREATE TABLE Users (
    UserId UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(100) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(50),
    LastName NVARCHAR(50),
    Avatar NVARCHAR(255),
    TrustScore DECIMAL(3,2) DEFAULT 0,
    CreatedAt DATETIME DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME
);
```

### Parcels Table
```sql
CREATE TABLE Parcels (
    ParcelId UNIQUEIDENTIFIER PRIMARY KEY,
    SenderId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(UserId),
    ReceiverId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(UserId),
    Description NVARCHAR(500),
    Weight DECIMAL(8,2),
    PostedDate DATETIME DEFAULT GETUTCDATE(),
    PickupLocation NVARCHAR(255),
    DeliveryLocation NVARCHAR(255),
    Status NVARCHAR(50) DEFAULT 'Pending',
    CreatedAt DATETIME DEFAULT GETUTCDATE()
);
```

### Deliveries Table
```sql
CREATE TABLE Deliveries (
    DeliveryId UNIQUEIDENTIFIER PRIMARY KEY,
    ParcelId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Parcels(ParcelId),
    DeliverId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(UserId),
    StartDate DATETIME,
    EndDate DATETIME,
    CurrentLocation NVARCHAR(255),
    Status NVARCHAR(50) DEFAULT 'Accepted',
    CreatedAt DATETIME DEFAULT GETUTCDATE()
);
```

### Ratings Table
```sql
CREATE TABLE Ratings (
    RatingId UNIQUEIDENTIFIER PRIMARY KEY,
    RaterId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(UserId),
    RatedUserId UNIQUEIDENTIFIER FOREIGN KEY REFERENCES Users(UserId),
    Score INT CHECK (Score >= 1 AND Score <= 5),
    Comment NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETUTCDATE()
);
```

## Development

### Project Structure

```
KargoBackEnd/
├── Controllers/                   # API endpoints
│   ├── AuthController.cs
│   ├── ParcelsController.cs
│   ├── DeliveriesController.cs
│   ├── UsersController.cs
│   └── RatingsController.cs
├── Models/                        # Domain models
│   ├── User.cs
│   ├── Parcel.cs
│   ├── Delivery.cs
│   └── Rating.cs
├── DTOs/                          # Data Transfer Objects
│   ├── LoginRequest.cs
│   ├── RegisterRequest.cs
│   └── ...
├── Services/                      # Business logic
│   ├── AuthService.cs
│   ├── ParcelService.cs
│   └── ...
├── Data/
│   └── AppDbContext.cs            # EF Core DbContext
├── Migrations/                    # Database migrations
├── appsettings.json
├── Program.cs                     # Application setup
└── KargoBackEnd.csproj
```

### Running

**Debug Mode:**
```bash
dotnet run
```

**Watch Mode (auto-restart):**
```bash
dotnet watch run
```

**Release Build:**
```bash
dotnet publish -c Release -o ./publish
```

### Database Operations

```bash
# Create database
dotnet ef database update

# Add new migration
dotnet ef migrations add AddNewTable

# Remove last migration
dotnet ef migrations remove

# View migrations
dotnet ef migrations list

# Drop database
dotnet ef database drop
```

## Security Best Practices

✅ **Implemented:**
- JWT token authentication
- Secure password hashing
- EF Core parameterized queries (SQL injection prevention)
- CORS configuration
- Input validation

⚠️ **Recommendations:**
- Use HTTPS in production
- Implement rate limiting
- Add request logging and monitoring
- Use strong JWT secrets (min 32 chars)
- Enable HSTS headers
- Regular security updates
- Implement audit logging

## Troubleshooting

### Database Connection Error
```
Cannot open database "KargoDB"
```
**Solution:**
- Ensure SQL Server is running
- Check connection string syntax
- Verify database exists or run migrations
- Check user permissions

### JWT Token Invalid
```
Invalid token / Token expired
```
**Solution:**
- Verify JWT:Key matches
- Check token format (Bearer eyJ...)
- Use refresh token endpoint
- Verify server time sync

### Port Already in Use
```
Failed to bind to address
```
**Solution:**
```bash
# Kill process on port 5000
netstat -ano | findstr :5000
taskkill /PID PID_NUMBER /F
```

### CORS Issues
```
Access-Control-Allow-Origin header error
```
**Solution:**
- Check Program.cs CORS configuration
- Verify frontend URL is whitelisted
- Ensure credentials are handled correctly

## Performance Tips

- Enable query result caching
- Use database indexes for frequently queried columns
- Implement pagination for list endpoints
- Use async/await for all database operations
- Monitor query performance with EF Core logging
- Compress API responses

## Testing

```bash
# Run all tests
dotnet test

# Run specific test
dotnet test --filter TestMethodName

# Generate coverage report
dotnet test /p:CollectCoverageHistory=true
```

## Deployment

### Azure Deployment

```bash
# Publish for production
dotnet publish -c Release

# Create deployment package
# Deploy to Azure App Service
```

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app
COPY . .
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 80
ENTRYPOINT ["dotnet", "KargoBackEnd.dll"]
```

## Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add feature'`)
4. Push branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

## License

This project is licensed under the MIT License.

---

**For questions and support, please open an issue on GitHub!**
