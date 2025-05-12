import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import ValidateForm from '../../helpers/validateForm';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css',
  standalone: true
})
export class SignupComponent {
  message: string = '';
  isError: boolean = false;

  type: string = "password";
  isText: boolean = false;
  eyeIcon: String = "fa-eye-slash";
  signUpForm!: FormGroup;
  logoPath = 'assets/images/logo.png'
  constructor(private fb: FormBuilder, private auth: AuthService) { }

  ngOnInit(): void {
    this.signUpForm = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, {
      validator: this.passwordMatchValidator
    });
  }

  passwordMatchValidator(formGroup: FormGroup) {
    const password = formGroup.get('password')?.value;
    const confirmPassword = formGroup.get('confirmPassword')?.value;

    if (password !== confirmPassword) {
      formGroup.setErrors({ passwordMismatch: true });
      return { passwordMismatch: true };
    } else {
      formGroup.setErrors(null);
      return null;
    }
  }

  // Her iki şifre alanını da aynı anda değiştirmek için
  hideShowPass() {
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onSignUp() {
    if (this.signUpForm.valid) {
      console.log(this.signUpForm.value);

      const signupData = {
        firstName: this.signUpForm.value.firstName,
        lastName: this.signUpForm.value.lastName,
        userName: this.signUpForm.value.userName,
        email: this.signUpForm.value.email,
        phoneNumber: this.signUpForm.value.phoneNumber,
        password: this.signUpForm.value.password
      };

      // Burada API'ye kayıt isteğini gönder
      // this.authService.signup(signupData).subscribe({...})
    } else {
      this.validateAllFormFields(this.signUpForm);
    }
  }

  validateAllFormFields(formGroup: FormGroup) {
    Object.keys(formGroup.controls).forEach(field => {
      const control = formGroup.get(field);
      control?.markAsDirty();
      control?.updateValueAndValidity();
    });
  }
}
