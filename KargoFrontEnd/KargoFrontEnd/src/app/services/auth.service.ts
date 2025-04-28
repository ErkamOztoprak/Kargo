import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  forgotPassword(email: any): Observable<{token?:string}> {
    throw new Error('Method not implemented.');
  }
  storeToken(token: string) {
    throw new Error('Method not implemented.');
  }

  private baseUrl:string="https://localhost:44318/api/User/";
  constructor(private http: HttpClient) { }

  signup(userObj:any){
    return this.http.post<any>(`${this.baseUrl}register`,userObj)
  }

  login(userObj: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}authenticate`, userObj);
  }
  saveToken(token:string){
    if (token) {
      localStorage.setItem('token', token);
    }
  }
  getToken(): string | null{
    return localStorage.getItem('token');
  
  }
  logout(): void{
    localStorage.removeItem('token');
    
  }
  isAuthenticated(): boolean{
    return !!this.getToken();
  }

}
