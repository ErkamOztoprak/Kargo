# Parcel Delivery Web Application

**Problem:** Students struggle to receive their parcels because dormitories don't accept deliveries directly inside the building.

**Solution:** This project provides a **listing, tracking, and trust system** that allows students to receive their parcels through **trusted couriers**.

---

## Features

- User registration & authentication (JWT Authentication)
- Post and browse parcel delivery listings
- **Parcel tracking dashboard** (monitor active parcel status)
- **Trust Score**: user reliability metric
- Trusted courier recommendations (rating system)
- User profile with trust score and delivery history

---

## Tech Stack

- **Backend:** .NET 6, ASP.NET Core, Entity Framework Core, MSSQL, JWT, Swagger
- **Frontend:** Angular, TypeScript, Bootstrap

---

## Getting Started

```bash
# Backend
dotnet restore
dotnet ef database update
dotnet run   # http://localhost:5000/swagger

# Frontend
npm install
ng serve     # http://localhost:4200
```
