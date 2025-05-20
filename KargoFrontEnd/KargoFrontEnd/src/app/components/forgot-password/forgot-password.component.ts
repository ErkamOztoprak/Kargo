import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl } from '@angular/forms';
import { Router } from '@angular/router';
import { ReactiveFormsModule } from '@angular/forms'; // ReactiveFormsModule import edin
import { CommonModule } from '@angular/common';


@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css'],
  standalone: true, // Eğer standalone ise
  imports: [
    ReactiveFormsModule, // Standalone için buraya ekleyin
    CommonModule         // Standalone için buraya ekleyin
  ]
  
})
export class ForgotPasswordComponent implements OnInit {
  forgotPasswordForm!: FormGroup;
  messageSent: boolean = false;
  isLoading: boolean = false;
  submittedEmail: string = '';

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private commonModule: CommonModule,

  ) { }

  ngOnInit(): void {
    this.forgotPasswordForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  // E-posta form kontrolü için getter
  get emailControl(): AbstractControl | null {
    return this.forgotPasswordForm.get('email');
  }

  onSubmit(): void {
    if (this.forgotPasswordForm.invalid) {
      this.forgotPasswordForm.markAllAsTouched();
      return;
    }

    this.isLoading = true;
    this.submittedEmail = this.forgotPasswordForm.value.email;

    setTimeout(() => {
      this.messageSent = true;
      this.isLoading = false;
    }, 1500);
  }
  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }
}
