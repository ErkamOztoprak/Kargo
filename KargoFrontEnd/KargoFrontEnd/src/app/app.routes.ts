import { Routes } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { SignupComponent } from './components/signup/signup.component';
import {ForgotPasswordComponent} from './components/forgot-password/forgot-password.component';
import {HomepageComponent} from './components/homepage/homepage.component';
import { ProfileComponent } from './components/profile/profile.component';


export const routes: Routes = [
    {path:'login',component: LoginComponent},
    {path:'signup',component: SignupComponent},
    {path:'forgot-password',component: ForgotPasswordComponent},   
    {path:'homepage',component: HomepageComponent}, 
    {path:'profile',component: ProfileComponent},
    {path:'',redirectTo:'/login',pathMatch:'full'},
    {path: '**', redirectTo: '/login'}
    
];
