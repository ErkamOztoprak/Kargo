// filepath: d:\Kargo\Kargo\KargoFrontEnd\KargoFrontEnd\src\app\guards\auth.guard.ts
import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { AuthService } from '../services/auth.service'; // AuthService'i import edin

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot): Observable<boolean | UrlTree> | Promise<boolean | UrlTree> | boolean | UrlTree {

    console.log('--- AUTH GUARD CAN ACTIVATE BAŞLADI! ---');
    
    console.log('AuthGuard.canActivate() called for route:', state.url);
    const isAuthenticated = this.authService.isAuthenticated(); // Bu, AuthService'teki logları tetikleyecektir
    console.log('AuthGuard: isAuthenticated result from service:', isAuthenticated);
    if (isAuthenticated) {
      return true;
    } else {
      console.log('AuthGuard: User not authenticated, redirecting to /login');
      this.router.navigate(['/login'], { queryParams: { returnUrl: state.url }});
      return false;
    }
  }
}