import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup,Validators,ReactiveFormsModule, FormControl } from '@angular/forms';
import ValidateForm from '../../helpers/validateForm';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router'; 
import {HttpClient,HttpHeaders} from "@angular/common/http";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule]
})

export class LoginComponent {

  type: string = "password";
  isText: boolean = false;
  eyeIcon: String ="fa-eye-slash";
  loginForm!:FormGroup;
  logoPath = 'assets/images/logo.png'

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router){ }
    
  ngOnInit():void{    this.loginForm=this.fb.group({
      username: ['',Validators.required],
      password: ['',Validators.required]
    })
  }
  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onLogin() {
    if(this.loginForm.valid) {
      console.log(this.loginForm.value)
      this.auth.login(this.loginForm.value).subscribe({
        next:(res)=>{
          console.log('Response:',res);

          if(res && res.token){
            console.log('Attempting to save token:', res.token);
            this.auth.storeToken(res.token);
            console.log('Token should be saved now. Current token from service:', this.auth.getToken()); // Kayıttan sonra token'ı kontrol et

            
            if(res.userName&&res.email){
              const user = {
                userName: res.userName,
                email: res.email,
                
              };
              localStorage.setItem('user', JSON.stringify(user));
            }
            this.router.navigate(['/homepage']);
          }
          else{
            console.error('User information is missing in the response');
            alert('Login successful, but user information is missing.');
          }
          
        },
        error:(err)=>{
          console.error('Error Response:', err);
          alert(err.error.message)
        }
      })
    }
    else
    {
      console.log("form is not valid");
      ValidateForm.validateAllFormFields(this.loginForm);
      alert("your form is invalid");
    }
  }
  onForgetPassword(){
    console.log("forgot passowrd page is called");
    this.router.navigate(['/forgot-password']);
  }
  
}
