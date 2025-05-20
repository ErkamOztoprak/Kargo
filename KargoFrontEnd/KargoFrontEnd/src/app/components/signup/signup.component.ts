import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule, FormControl } from '@angular/forms';
import ValidateForm from '../../helpers/validateForm';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

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
  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) { }

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

      this.auth.signup(signupData).subscribe({
        next:(response)=>{
          console.log('Kayit Basarili',response);
          this.message = 'Kayit Basarili! Giris Yapabilirsiniz';
          this.isError = false;
          this.signUpForm.reset();
          
          this.router.navigate(['/login']);

        }
      })
    } else {
      this.validateAllFormFields(this.signUpForm);
      this.message = 'Lutfen Formu Doldurunuz';
      this.isError = true;
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
