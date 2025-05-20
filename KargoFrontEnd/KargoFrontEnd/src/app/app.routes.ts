import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import { HomepageComponent } from './components/homepage/homepage.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ProfileComponent } from './components/profile/profile.component';
import { CargoRequestCreateComponent } from './components/cargo-request-create/cargo-request-create.component';
import { AuthGuard } from './services/auth.guard';

export const routes: Routes = [
    {path:'login',component: LoginComponent},
    {path:'signup',component: SignupComponent},
    {path:'forgot-password',component: ForgotPasswordComponent },   
    {path:'homepage',component: HomepageComponent,canActivate: [AuthGuard]}, // AuthGuard ile korunan rota
    {path:'',redirectTo:'/login',pathMatch:'full'},
    {path: 'profile', component: ProfileComponent, canActivate: [AuthGuard] }, // AuthGuard ile korunan rota
    {path: 'cargo-request-create', component: CargoRequestCreateComponent, canActivate: [AuthGuard] },// AuthGuard ile korunan rota
    {path: '**', redirectTo: '/login'},
    
];
