import { Injectable } from '@angular/core';
import {HttpClient,HttpHeaders} from "@angular/common/http";
import { Observable } from 'rxjs';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  forgotPassword(email: any): Observable<{token?:string}> {
    throw new Error('Method not implemented.');
  }
  
  getUserProfile(): Observable<any> {
    return this.http.get<any>(`${this.baseUrl}profile`);
  }
  updateUserProfile(profileData: any): Observable<any>{
    return this.http.put<any>(`${this.baseUrl}profile/update`, profileData);
  }

  private baseUrl:string="https://localhost:44318/api/User/";
  constructor(private http: HttpClient) { }

  signup(userObj:any){
    return this.http.post<any>(`${this.baseUrl}register`,userObj)
  }

  login(userObj: any): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}authenticate`, userObj);
  }
  storeToken(token:string){
    if (token) {
      localStorage.setItem('token', token);
    }
  }
  getToken(): string | null {
    return localStorage.getItem('token'); // Ensure 'token' is the correct key
  }
  logout(): void{
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    
  }
  isAuthenticated(): boolean {
    const token = localStorage.getItem('token'); // Doğrudan localStorage.getItem kullanın
    console.log('AuthService.isAuthenticated() called.');
    console.log('Raw token from localStorage:', token);
    console.log('Type of token:', typeof token); // Token'ın tipini kontrol edin
    
    // Token'ın gerçekten dolu bir string olup olmadığını daha dikkatli kontrol edin
    const isAuthenticatedResult = (typeof token === 'string' && token.trim() !== ''); 
    
    console.log('IsAuthenticated result (detailed check):', isAuthenticatedResult);
    return isAuthenticatedResult;
  }

}
