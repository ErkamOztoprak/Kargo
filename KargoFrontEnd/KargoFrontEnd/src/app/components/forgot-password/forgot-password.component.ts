import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import { CommonModule } from '@angular/common';
import {AuthService} from '../../services/auth.service';
import { Router } from '@angular/router'; 


@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.css',
  standalone: true,
  imports:[ReactiveFormsModule,CommonModule]
})
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordForm!: FormGroup;

  constructor(private fb: FormBuilder, private authService: AuthService, private router:Router){
    console.log("forgot password page is called");
  }
  
  ngOnInit(): void {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }
  onSubmit():void {
    if(this.forgotPasswordForm.valid){
      const email = this.forgotPasswordForm.value.email;
      console.log('password reset link sent to email:',email);

      this.authService.forgotPassword(email).subscribe({
        next:(response: { token?: string }) => {
          console.log('Password reset link sent to email successfully', response);

          if(response.token){
            this.authService.storeToken(response.token);
            this.router.navigate(['/dashboard']);
          }
        }
      })
    }
  }
 
}
