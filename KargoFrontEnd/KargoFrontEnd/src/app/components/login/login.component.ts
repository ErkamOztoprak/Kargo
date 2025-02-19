import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup,Validators,ReactiveFormsModule, FormControl } from '@angular/forms';
import ValidateForm from '../../helpers/validateForm';

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

  constructor(private fb: FormBuilder){ }
    
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

  onSubmit()
  {
    if(this.loginForm.valid)
    {
      console.log(this.loginForm.value)
    }
    else
    {
      console.log("form is not valid");
      ValidateForm.validateAllFormFields(this.loginForm);
      alert("your form is invalid");
    }
  }

  
}
