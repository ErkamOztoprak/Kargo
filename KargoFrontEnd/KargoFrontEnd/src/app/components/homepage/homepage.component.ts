import { Component,OnInit } from '@angular/core';
import {Router, RouterModule} from '@angular/router';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrl: './homepage.component.css',
  imports: [CommonModule,RouterModule]
})
export class HomepageComponent implements OnInit {
  userName: string | null= null;
  logoPath = 'assets/images/logo.png';

  constructor(private router: Router) {}

  ngOnInit():void{
    const user =JSON.parse(localStorage.getItem('user')|| '{}');
    this.userName=user.userName || null;
  }
  logout():void{
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.userName=null;
    this.router.navigate(['/login']);
  }
}
