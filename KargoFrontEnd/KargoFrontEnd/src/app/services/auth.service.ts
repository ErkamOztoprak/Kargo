import { Injectable } from '@angular/core';
import {HttpClient,HttpHeaders} from "@angular/common/http";
import { first, Observable } from 'rxjs';
import { jwtDecode } from 'jwt-decode';

export interface UserProfile {
  id: string | null;
  username: string | null;
  firstName: string | null;
  lastName: string | null;
  email: string | null;
}

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

  getUserName(): string | null {
    const currentUser =this.getCurrentUser();
    return currentUser ? currentUser.username : null;
  }
  getCurrentUser(): UserProfile | null {
    const token = this.getToken();
    if(token){
      try{
        const decodedToken:any =jwtDecode(token);
        return{
          id: decodedToken.sub|| null,
          username: decodedToken.name || decodedToken.unique_name || null,
          firstName: decodedToken.given_name || null,
          lastName: decodedToken.family_name || null,
          email: decodedToken.email || null,
        }
      }catch(error){
        console.error('Token decoding failed:', error);
        return null;
      }
    }
    return null;
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
    return localStorage.getItem('token'); 
  }
  logout(): void{
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    
  }
  isAuthenticated(): boolean {
    const token = localStorage.getItem('token'); 
    console.log('AuthService.isAuthenticated() called.');
    console.log('Raw token from localStorage:', token);
    console.log('Type of token:', typeof token); 
    
    const isAuthenticatedResult = (typeof token === 'string' && token.trim() !== ''); 
    
    console.log('IsAuthenticated result (detailed check):', isAuthenticatedResult);
    return isAuthenticatedResult;
  }

}
