import { Component } from '@angular/core';

@Component({
  selector: 'app-homepage',
  imports: [],
  templateUrl: './homepage.component.html',
  styleUrl: './homepage.component.css'
})
export class HomepageComponent {
  userName: string = '';

  ngOnInit():void{
    const user =JSON.parse(localStorage.getItem('user')|| '{}');
    this.userName=user.name || 'Misafir';
  }
}
