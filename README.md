# Kargo - Parcel Delivery Web Application 📦

A comprehensive web application designed to solve the problem of dormitory students receiving parcels. This platform enables secure parcel delivery management through trusted couriers with built-in tracking and rating systems.

## Problem

Students face challenges receiving their parcels because dormitories don't accept direct deliveries inside the building. Kargo provides a solution through a listing and trust-based system.

## Solution

A comprehensive platform that allows students to:
- Post parcel delivery requests
- Browse available delivery options
- Track parcels in real-time
- Rate and build trust with couriers
- View delivery history and ratings

## Table of Contents

- [Features](#features)
- [Project Structure](#project-structure)
- [Technology Stack](#technology-stack)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Endpoints](#api-endpoints)
- [Database](#database)
- [Development](#development)
- [Contributing](#contributing)
- [License](#license)

## Features

✨ **User Management**
- User registration and authentication
- JWT token-based security
- User profile with delivery history
- Trust score calculation

📦 **Parcel Delivery**
- Post parcel delivery listings
- Browse available parcels to deliver
- Real-time parcel tracking
- Delivery status updates

⭐ **Trust & Rating System**
- Trust score metric for users
- Rating system for couriers
- Trusted courier recommendations
- User reliability tracking
- Verified courier badges

🔐 **Security**
- JWT token-based authentication
- Secure password hashing
- Role-based access control
- Protected API endpoints

## Project Structure

```
Kargo/
├── KargoBackEnd/
│   ├── Controllers/                  # API controllers
│   ├── Models/                       # Data models
│   │   ├── User.cs
│   │   ├── Parcel.cs
│   │   ├── Delivery.cs
│   │   └── DTOs/
│   ├── Services/                     # Business logic
│   ├── Data/
│   │   └── AppDbContext.cs           # EF Core context
│   ├── Migrations/                   # Database migrations
│   ├── appsettings.json              # Configuration
│   └── KargoBackEnd.csproj
│
├── KargoFrontEnd/
│   ├── KargoFrontEnd/
│   ├── src/
│   │   ├── app/
│   │   │   ├── components/
│   │   │   ├── services/
│   │   │   ├── models/
│   │   │   └── app.component.ts
│   │   ├── assets/
│   │   ├── styles.css
│   │   └── main.ts
│   ├── angular.json
│   ├── package.json
│   └── tsconfig.json
│
├── Kargo.sln                         # Solution file
└── README.md
```

## Technology Stack

### Backend
- **Framework**: ASP.NET Core 6
- **Language**: C#
- **Database**: MSSQL (Microsoft SQL Server)
- **ORM**: Entity Framework Core
- **Authentication**: JWT (JSON Web Tokens)
- **API Documentation**: Swagger/OpenAPI
- **API Pattern**: RESTful API

### Frontend
- **Framework**: Angular
- **Language**: TypeScript
- **Styling**: Bootstrap 5
- **Http Client**: Angular HttpClientModule
- **State Management**: RxJS
- **Build Tool**: Angular CLI

## Prerequisites

Before you begin, ensure you have the following installed:

- **.NET SDK 6** or higher
- **SQL Server** (Express or Full edition)
- **Node.js** (v14 or higher)
- **npm** (v6 or higher)
- **Visual Studio** or **Visual Studio Code**
- **Git**

## Installation

### Clone the Repository

```bash
git clone https://github.com/ErkamOztoprak/Kargo.git
cd Kargo
```

### Backend Setup

```bash
cd KargoBackEnd

# Restore NuGet packages
dotnet restore

# Create and apply database migrations
dotnet ef database update

# Build the backend
dotnet build
```

### Frontend Setup

```bash
cd KargoFrontEnd/KargoFrontEnd

# Install npm dependencies
npm install

# Build frontend assets
npm run build
```

## Configuration

### Backend Configuration

Edit `KargoBackEnd/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=KargoDB;User Id=sa;Password=YOUR_PASSWORD;"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-characters",
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

### Frontend Configuration

API base URL is typically configured in `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

## Running the Application

### Start the Backend

```bash
cd KargoBackEnd
dotnet run
```

Backend will start at: `http://localhost:5000`
Swagger UI: `http://localhost:5000/swagger`

### Start the Frontend

In a new terminal:

```bash
cd KargoFrontEnd/KargoFrontEnd
ng serve
# or
npm start
```

Frontend will start at: `http://localhost:4200`

### Access the Application

Open your browser and navigate to:
```
http://localhost:4200
```

## API Endpoints

### Authentication Routes

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/register` | Register new user |
| POST | `/api/auth/login` | User login |
| POST | `/api/auth/logout` | User logout |
| POST | `/api/auth/refresh` | Refresh token |

### Parcel Routes

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/parcels` | List all parcels |
| GET | `/api/parcels/{id}` | Get parcel details |
| POST | `/api/parcels` | Create new parcel listing |
| PUT | `/api/parcels/{id}` | Update parcel |
| DELETE | `/api/parcels/{id}` | Delete parcel |

### Delivery Routes

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/deliveries` | List all deliveries |
| GET | `/api/deliveries/{id}` | Get delivery details |
| POST | `/api/deliveries` | Create new delivery |
| PUT | `/api/deliveries/{id}` | Update delivery status |

### User Routes

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/users/{id}` | Get user profile |
| GET | `/api/users/{id}/history` | Get delivery history |
| GET | `/api/users/{id}/trust-score` | Get trust score |
| PUT | `/api/users/{id}` | Update user profile |

### Rating Routes

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/ratings` | Create rating |
| GET | `/api/ratings/user/{id}` | Get user ratings |
| PUT | `/api/ratings/{id}` | Update rating |

## Database

### Schema Overview

**Users Table:**
- UserId (Primary Key)
- Username (unique)
- Email (unique)
- PasswordHash
- FirstName
- LastName
- Avatar
- TrustScore
- CreatedAt
- UpdatedAt

**Parcels Table:**
- ParcelId (Primary Key)
- SenderId (Foreign Key)
- ReceiverId (Foreign Key)
- Description
- Weight
- PostedDate
- PickupLocation
- DeliveryLocation
- Status

**Deliveries Table:**
- DeliveryId (Primary Key)
- ParcelId (Foreign Key)
- DelivererId (Foreign Key)
- StartDate
- EndDate
- Status
- CurrentLocation

**Ratings Table:**
- RatingId (Primary Key)
- RaterId (Foreign Key)
- RatedUserId (Foreign Key)
- Score (1-5)
- Comment
- CreatedAt

### Database Setup

```bash
# Create database
dotnet ef database update

# View migrations
dotnet ef migrations list

# Add new migration
dotnet ef migrations add MigrationName

# Revert last migration
dotnet ef database update PreviousMigration
```

## Development

### Backend Development

**Debug Mode:**
```bash
dotnet run
```

**Watch Mode:**
```bash
dotnet watch run
```

**Build Release:**
```bash
dotnet publish -c Release -o ./publish
```

### Frontend Development

**Development Server:**
```bash
ng serve
```

**Build for Production:**
```bash
ng build --prod
```

**Generate Components:**
```bash
ng generate component component-name
ng generate service service-name
```

### Running Tests

**Backend Tests:**
```bash
dotnet test
```

**Frontend Tests:**
```bash
ng test
```

## Security Best Practices

✅ **Implemented:**
- JWT token authentication
- Password hashing (bcrypt)
- CORS protection
- SQL injection prevention (EF Core)
- Input validation

⚠️ **Recommendations:**
- Use HTTPS in production
- Implement rate limiting
- Add request validation middleware
- Use secure password policies
- Implement two-factor authentication
- Regular security audits
- Keep dependencies updated

## Troubleshooting

### Database Connection Issues
```
Cannot open database
```
**Solution:**
- Check SQL Server is running
- Verify connection string
- Check user permissions
- Ensure database exists

### JWT Token Expired
```
Unauthorized: Token expired
```
**Solution:**
- Use refresh token endpoint
- Increase token expiration time
- Check server time synchronization

### CORS Issues
```
Access to XMLHttpRequest blocked
```
**Solution:**
- Configure CORS in backend
- Ensure frontend URL is whitelisted
- Check allowed methods and headers

### Port Already in Use
```
Address already in use
```
**Solution:**
```bash
# Change port in launchSettings.json
# or kill existing process
netstat -ano | findstr :5000
taskkill /PID PID_NUMBER /F
```

## Performance Tips

- Enable caching for frequently accessed data
- Implement pagination for large datasets
- Use lazy loading in Angular components
- Optimize database queries with indexes
- Compress API responses
- Implement CDN for static assets

## Deployment

### Backend Deployment (Azure)

```bash
# Publish for production
dotnet publish -c Release -o ./publish

# Create deployment package
dotnet publish -c Release
```

### Frontend Deployment

```bash
# Build for production
ng build --prod

# Can be deployed to:
# - Azure Static Web Apps
# - Firebase Hosting
# - GitHub Pages
# - Netlify
```

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Coding Standards

- Follow C# naming conventions (PascalCase)
- Use meaningful variable names
- Add XML documentation comments
- Write unit tests for new features
- Keep functions small and focused

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

For support, issues, or questions:
- Open an issue on GitHub
- Check existing issues for solutions
- Contact the development team

## Project Links

- [GitHub Repository](https://github.com/ErkamOztoprak/Kargo)
- [Backend Documentation](./KargoBackEnd/README.md)
- [Frontend Documentation](./KargoFrontEnd/README.md)

---

**Made with ❤️ for dormitory students**
