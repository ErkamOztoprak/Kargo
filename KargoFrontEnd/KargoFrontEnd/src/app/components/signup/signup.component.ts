import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators,ReactiveFormsModule, FormControl } from '@angular/forms';
import ValidateForm from '../../helpers/validateForm';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-signup',
  imports: [CommonModule,ReactiveFormsModule],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.css',
  standalone:true
})
export class SignupComponent {
  message: string = '';
  isError:boolean=false;

  type: string = "password";
  isText: boolean = false;
  eyeIcon: String ="fa-eye-slash";
  signUpForm!:FormGroup;
  constructor(private fb: FormBuilder,private auth: AuthService){}

  ngOnInit(): void {
    this.signUpForm = this.fb.group(
    {
      firstName:['',Validators.required],
      lastName:['',Validators.required],
      userName:['',Validators.required],
      email:['',[Validators.required,Validators.email]],
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
    if(this.signUpForm.valid){
      this.auth.signup(this.signUpForm.value).subscribe({
        next:(res=>{
          console.log("Response:",res);
          this.message=res.message;
          this.isError=false;
          alert(res.message)
        }),
        error:(err=>{
          console.log("Error:",err);
          this.message=err.error?.message || "Signup Failed";
          this.isError=true;
          alert(err.error.message)
        })
      })

    }
    else
    {
      ValidateForm.validateAllFormFields(this.signUpForm)
    }
  }
  
}
