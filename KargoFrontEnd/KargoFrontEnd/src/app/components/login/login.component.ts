import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup,Validators,ReactiveFormsModule, FormControl } from '@angular/forms';
import ValidateForm from '../../helpers/validateForm';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrl: './login.component.css',
  standalone: true,
  imports: [ReactiveFormsModule,CommonModule]
 
})
export class LoginComponent {

  type: string = "password";
  isText: boolean = false;
  eyeIcon: String ="fa-eye-slash";
  loginForm!:FormGroup;

  constructor(private fb: FormBuilder, private auth: AuthService){ }
    
  ngOnInit():void{
    this.loginForm=this.fb.group({
      username: ['',Validators.required],
      password: ['',Validators.required]
    })
  }
  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }

  onLogin()
  {
    if(this.loginForm.valid)
    {
      console.log(this.loginForm.value)
      this.auth.login(this.loginForm.value).subscribe({
        next:(res)=>{
          alert(res.message)
        },
        error:(err)=>{
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

  
}
