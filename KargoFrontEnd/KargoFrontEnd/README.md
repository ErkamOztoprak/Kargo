# Kargo Frontend 📦

Modern Angular web application for the Kargo parcel delivery platform. Built with Angular, TypeScript, and Bootstrap, providing user authentication, parcel browsing, delivery tracking, and trust-based courier recommendations.

## Quick Start

```bash
cd KargoFrontEnd/KargoFrontEnd
npm install
ng serve
# or
npm start
```

Frontend: `http://localhost:4200`

## Table of Contents

- [Features](#features)
- [Technology Stack](#technology-stack)
- [Installation](#installation)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Development](#development)
- [Build and Deployment](#build-and-deployment)

## Features

🎨 **User Interface**
- Responsive design with Bootstrap 5
- Clean and intuitive layout
- Real-time notifications
- Mobile-optimized interface

🔐 **Authentication**
- User registration and login
- JWT token management
- Protected routes
- Session management

📦 **Parcel Management**
- Browse parcel listings
- Create delivery requests
- Search and filter parcels
- Real-time status updates
- Delivery tracking dashboard

⭐ **Trust & Rating System**
- Display user trust scores
- Rate couriers and senders
- View delivery history
- Trusted courier recommendations
- User rating display

📊 **Dashboard**
- Active deliveries overview
- Delivery history
- User profile with statistics
- Trust score dashboard
- Performance metrics

## Technology Stack

- **Framework**: Angular 12+
- **Language**: TypeScript 4.5+
- **Styling**: Bootstrap 5
- **CSS**: Native CSS with Bootstrap utilities
- **Http Client**: Angular HttpClientModule
- **State Management**: RxJS 7
- **Routing**: Angular Router
- **Forms**: Reactive Forms & Template-driven Forms
- **Testing**: Jasmine & Karma

## Installation

### Requirements

- Node.js v14 or higher
- npm v6 or higher
- Git

### Steps

```bash
# Clone repository
git clone https://github.com/ErkamOztoprak/Kargo.git
cd Kargo/KargoFrontEnd/KargoFrontEnd

# Install dependencies
npm install

# Start development server
ng serve
# or
npm start
```

Application will open at: `http://localhost:4200`

## Configuration

### API Endpoint Configuration

Create or edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5000'
};
```

Create or edit `src/environments/environment.prod.ts`:

```typescript
export const environment = {
  production: true,
  apiUrl: 'https://your-api-domain.com'
};
```

### Angular Configuration

`angular.json` contains project settings:
- Build options
- Development server configuration
- Testing setup
- Output hashing

### Bootstrap Configuration

Bootstrap 5 is included via npm and configured in `styles.css`.

## Project Structure

```
KargoFrontEnd/
├── KargoFrontEnd/
│   └── src/
│       ├── app/
│       │   ├── components/
│       │   │   ├── header/                # Navigation header
│       │   │   ├── footer/                # Footer component
│       │   │   ├── login/                 # Login page
│       │   │   ├── register/              # Registration page
│       │   │   ├── dashboard/             # Main dashboard
│       │   │   ├── parcels/               # Parcel listing
│       │   │   ├── parcel-detail/         # Parcel details
│       │   │   ├── deliveries/            # Delivery management
│       │   │   ├── profile/               # User profile
│       │   │   ├── ratings/               # Ratings display
│       │   │   └── home/                  # Home page
│       │   ├── services/
│       │   │   ├── api.service.ts         # HTTP requests
│       │   │   ├── auth.service.ts        # Authentication
│       │   │   ├── parcel.service.ts      # Parcel operations
│       │   │   ├── delivery.service.ts    # Delivery operations
│       │   │   ├── user.service.ts        # User operations
│       │   │   └── rating.service.ts      # Rating operations
│       │   ├── models/
│       │   │   ├── user.model.ts
│       │   │   ├── parcel.model.ts
│       │   │   ├── delivery.model.ts
│       │   │   └── rating.model.ts
│       │   ├── guards/
│       │   │   ├── auth.guard.ts          # Route protection
│       │   │   └── role.guard.ts          # Role-based access
│       │   ├── interceptors/
│       │   │   ├── auth.interceptor.ts    # JWT token injection
│       │   │   └── error.interceptor.ts   # Error handling
│       │   ├── app.component.ts           # Root component
│       │   ├── app.module.ts              # App module
│       │   ├── app-routing.module.ts      # Routing configuration
│       │   └── app.component.html
│       ├── assets/                        # Static assets
│       ├── styles.css                     # Global styles
│       ├── main.ts                        # Entry point
│       ├── index.html
│       └── ...config files
├── angular.json
├── package.json
├── tsconfig.json
└── README.md
```

## Components

### Authentication Components

**Login Component**
- Email/password form
- Login validation
- Error handling
- Remember me option

**Register Component**
- Registration form
- Password strength validation
- Email verification
- Terms acceptance

### Main Components

**Dashboard**
- Overview statistics
- Active deliveries
- Recent activity
- Quick actions

**Parcels Component**
- Browse available parcels
- Search and filter
- Sort by date, location, status
- Create new parcel listing
- Parcel details modal

**Deliveries Component**
- Track active deliveries
- Update delivery status
- View delivery history
- Confirmation actions

**Profile Component**
- User information
- Trust score display
- Delivery statistics
- Rating history
- Account settings

**Ratings Component**
- Display user ratings
- Rate deliveries
- View feedback
- Rating statistics

## Services

### AuthService

Handles user authentication:
```typescript
login(email: string, password: string): Observable<LoginResponse>
register(data: RegisterRequest): Observable<User>
logout(): void
getToken(): string
refreshToken(): Observable<AuthResponse>
isLoggedIn(): boolean
```

### ParcelService

Manages parcel operations:
```typescript
getParcels(): Observable<Parcel[]>
getParcel(id: string): Observable<Parcel>
createParcel(data: ParcelRequest): Observable<Parcel>
updateParcel(id: string, data: ParcelRequest): Observable<Parcel>
deleteParcel(id: string): Observable<void>
searchParcels(query: string): Observable<Parcel[]>
```

### DeliveryService

Handles delivery management:
```typescript
getDeliveries(): Observable<Delivery[]>
getDelivery(id: string): Observable<Delivery>
createDelivery(data: DeliveryRequest): Observable<Delivery>
updateDelivery(id: string, data: DeliveryRequest): Observable<Delivery>
completeDelivery(id: string): Observable<void>
```

### UserService

User profile operations:
```typescript
getProfile(id: string): Observable<User>
updateProfile(id: string, data: UserUpdate): Observable<User>
getDeliveryHistory(id: string): Observable<Delivery[]>
getTrustScore(id: string): Observable<TrustScore>
```

### RatingService

Rating management:
```typescript
getRatings(userId: string): Observable<Rating[]>
createRating(data: RatingRequest): Observable<Rating>
updateRating(id: string, data: RatingRequest): Observable<Rating>
deleteRating(id: string): Observable<void>
```

## Routing

`app-routing.module.ts` defines application routes:

```typescript
const routes: Routes = [
  { path: '', component: HomeComponent },
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { 
    path: 'dashboard', 
    component: DashboardComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'parcels', 
    component: ParcelsComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'parcels/:id', 
    component: ParcelDetailComponent, 
    canActivate: [AuthGuard] 
  },
  { 
    path: 'profile/:id', 
    component: ProfileComponent, 
    canActivate: [AuthGuard] 
  }
];
```

## Guards and Interceptors

### AuthGuard

Protects routes requiring authentication:
```typescript
canActivate(route: ActivatedRouteSnapshot): boolean {
  return this.authService.isLoggedIn();
}
```

### AuthInterceptor

Adds JWT token to all requests:
```typescript
intercept(req: HttpRequest<any>): Observable<HttpEvent<any>> {
  const token = this.authService.getToken();
  if (token) {
    req = req.clone({
      setHeaders: { Authorization: `Bearer ${token}` }
    });
  }
  return next.handle(req);
}
```

### ErrorInterceptor

Handles API errors globally:
```typescript
intercept(req: HttpRequest<any>): Observable<HttpEvent<any>> {
  return next.handle(req).pipe(
    catchError(error => this.handleError(error))
  );
}
```

## Forms

### Reactive Forms

Used for complex validation:
```typescript
this.form = this.fb.group({
  email: ['', [Validators.required, Validators.email]],
  password: ['', [Validators.required, Validators.minLength(8)]],
  firstName: ['', Validators.required],
  address: ['', Validators.required]
});
```

### Template-driven Forms

Used for simpler forms:
```html
<form #form="ngForm" (ngSubmit)="onSubmit()">
  <input [(ngModel)]="user.email" name="email" required>
  <button type="submit">Submit</button>
</form>
```

## Development

### Development Server

```bash
ng serve
# or
npm start
```

Server runs at `http://localhost:4200` with hot module replacement.

### Angular CLI Commands

```bash
# Generate component
ng generate component components/my-component
ng g c components/my-component

# Generate service
ng generate service services/my-service
ng g s services/my-service

# Generate module
ng generate module modules/my-module
ng g m modules/my-module

# Generate guard
ng generate guard guards/my-guard
ng g g guards/my-guard

# Lint check
ng lint

# Format code
ng lint --fix
```

## Testing

### Unit Tests

```bash
ng test
# or
npm test
```

Test files: `*.spec.ts`

### E2E Tests

```bash
ng e2e
```

## Build and Deployment

### Production Build

```bash
ng build --prod
# or
npm run build
```

Output directory: `dist/KargoFrontEnd/`

Build includes:
- Tree-shaking
- Code minification
- Lazy loading
- AOT compilation
- CSS/HTML minification

### Deployment Options

**GitHub Pages:**
```bash
ng build --prod --base-href="/Kargo/"
ngh --dir=dist/KargoFrontEnd
```

**Firebase Hosting:**
```bash
npm install -g firebase-tools
firebase login
firebase deploy
```

**Netlify:**
```bash
npm run build
# Deploy dist/KargoFrontEnd folder to Netlify
```

**Azure:**
```bash
npm run build
# Deploy to Azure Static Web Apps
```

## Styling

### Bootstrap 5 Classes

```html
<div class="container mt-4">
  <div class="row g-3">
    <div class="col-md-6">
      <div class="card">
        <div class="card-header">
          <h5 class="card-title">Parcels</h5>
        </div>
        <div class="card-body">
          <!-- Content -->
        </div>
      </div>
    </div>
  </div>
</div>
```

### Custom CSS

Global styles in `src/styles.css`:

```css
:root {
  --primary-color: #007bff;
  --secondary-color: #6c757d;
}

body {
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', sans-serif;
  background-color: #f8f9fa;
}

.custom-class {
  /* Custom styling */
}
```

## Performance Optimization

### Best Practices

- Use OnPush change detection: `ChangeDetectionStrategy.OnPush`
- Lazy load feature modules
- Unsubscribe from observables (use `takeUntil`)
- Use trackBy in *ngFor loops
- Implement virtual scrolling for long lists
- Preload critical data

### Performance Monitoring

```typescript
import { performance } from '@angular/common';

ngOnInit() {
  performance.mark('componentInit-start');
  // Component initialization
  performance.mark('componentInit-end');
  performance.measure('init', 'componentInit-start', 'componentInit-end');
}
```

## Troubleshooting

### CORS Error
```
Access to XMLHttpRequest blocked
```
**Solution:** Check backend CORS configuration

### Module Not Found
```
Cannot find module '@angular/...'
```
**Solution:** Run `npm install`

### Build Errors
```
NG6001: This declaration is not an Angular component
```
**Solution:** Ensure component is declared in module or marked standalone

### Port Already in Use
```
Port 4200 is already in use
```
**Solution:**
```bash
ng serve --port 4201
```

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)
- Mobile browsers

## Environment Variables

Create `.env` file (if using dotenv):

```env
NG_APP_API_URL=http://localhost:5000
NG_APP_NAME=Kargo
NG_APP_VERSION=1.0.0
```

Access in code:
```typescript
const apiUrl = process.env['NG_APP_API_URL'];
```

## Contributing

1. Fork repository
2. Create feature branch (`git checkout -b feature/amazing-feature`)
3. Commit changes (`git commit -m 'Add feature'`)
4. Push branch (`git push origin feature/amazing-feature`)
5. Open Pull Request

### Code Style

- Follow Angular style guide
- Use meaningful variable names
- Add comments for complex logic
- Write unit tests for components
- Ensure linting passes

## License

This project is licensed under the MIT License.

---

**For questions and support, please open an issue on GitHub!**
