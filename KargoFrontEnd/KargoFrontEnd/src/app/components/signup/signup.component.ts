import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators,ReactiveFormsModule, FormControl } from '@angular/forms';
import ValidateForm from '../../helpers/validateForm';

@Component({
  selector: 'app-signup',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css',
  standalone:true
})
export class SignupComponent {

  type: string = "password";
  isText: boolean = false;
  eyeIcon: String ="fa-eye-slash";
  signUpForm!:FormGroup;
  constructor(private fb: FormBuilder){}

  ngOnInit(): void {
    this.signUpForm = this.fb.group(
    {
      firstName:['',Validators.required],
      lastName:['',Validators.required],
      userName:['',Validators.required],
      email:['',Validators.required],
      password:['',Validators.required],
    });
  }
  hideShowPass(){
    this.isText = !this.isText;
    this.isText ? this.eyeIcon = "fa-eye" : this.eyeIcon = "fa-eye-slash";
    this.isText ? this.type = "text" : this.type = "password";
  }
  
  onSignUp()
  {
    if(this.signUpForm.valid)
    {
      console.log(this.signUpForm.value)
    }
    else
    {
      ValidateForm.validateAllFormFields(this.signUpForm)
    }
  }
  
}
