import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class ProfileComponent implements OnInit {
  userName: string = 'Yükleniyor...';
  email: string = '';
  firstName: string = '';
  lastName: string = '';
  phoneNumber: string = '';
  bio: string = '';
  profilePicture: string = 'assets/images/default-profile.png';
  rating: number = 0;
  isVerified: boolean = false;
  currentPassword: string = '';
  newPassword: string = '';
  confirmPassword: string = '';

  activeTab: string = 'activity';

  constructor(private authService: AuthService, private router: Router) { }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }

  ngOnInit(): void {
    if (!this.authService.isAuthenticated()) {
      this.router.navigate(['/login']);
      return;
    }

    const storedUser = localStorage.getItem('user');
    if (storedUser) {
      try {
        const userObject = JSON.parse(storedUser);
        if (userObject && userObject.userName) {
          this.userName = userObject.userName;
        }
      } catch (e) {
        // Hata durumunda sessiz kalabilir veya minimal bir log bırakılabilir
      }
    }
    
    this.fetchUserProfile(); 
    
    const fragment = window.location.hash.replace('#', '');
    if (fragment && ['activity', 'reviews', 'settings'].includes(fragment)) {
      this.activeTab = fragment;
    }
  }

  fetchUserProfile(): void { 
    this.authService.getUserProfile().subscribe({
      next: (profileData: { 
        userName?: string; 
        email?: string; 
        firstName?: string; 
        lastName?: string; 
        phoneNumber?: string; 
        profilePictureUrl?: string; 
        profilePicture?: string;    
        rating?: number; 
        isVerified?: boolean 
      }) => {
        this.userName = profileData.userName || 'Kullanıcı Adı';
        this.email = profileData.email || '';
        this.firstName = profileData.firstName || '';
        this.lastName = profileData.lastName || '';
        this.phoneNumber = profileData.phoneNumber || '';
        this.profilePicture = profileData.profilePictureUrl || profileData.profilePicture || 'assets/images/default-profile.png';
        this.rating = profileData.rating || 0;

        if (profileData.hasOwnProperty('isVerified')) { 
          this.isVerified = !!profileData.isVerified;
        } else {
          this.isVerified = false;
        }
      },
      error: (err: any) => {
        console.error('Error fetching profile data from backend:', err);
        this.userName = 'Profil bilgileri alınamadı.';
        this.isVerified = false; 
        if (err.status === 401 || err.status === 403) {
          alert('Lütfen tekrar giriş yapınız.');
          this.authService.logout(); 
          this.router.navigate(['/login']);
        }
      }
    });
  }
  
  setActiveTab(tabName: string): void {
    this.activeTab = tabName;
    window.location.hash = tabName;
  }
    saveProfileSettings(): void {
    const profileDataToUpdate = {
      firstName: this.firstName,
      lastName: this.lastName,
      email: this.email, 
      phoneNumber: this.phoneNumber,
    };

    this.authService.updateUserProfile(profileDataToUpdate).subscribe({
      next: (response) => {
        console.log('Profil güncelleme yanıtı:', response);
        alert(response?.message || 'Kişisel bilgileriniz başarıyla güncellendi!');
        // İsteğe bağlı: Güncellenmiş veriyi tekrar çekebilir veya component özelliklerini yanıttan güncelleyebilirsiniz.
        // this.fetchUserProfile(); // En güncel veriyi almak için
      },
      error: (err) => {
        console.error('Kişisel bilgiler güncellenirken hata oluştu:', err);
        let errorMessage = 'Kişisel bilgileriniz güncellenirken bir hata oluştu.';
        if (err.error && typeof err.error === 'string') {
            errorMessage = err.error;
        } else if (err.error && err.error.message && typeof err.error.message === 'string') {
            errorMessage = err.error.message;
        } else if (err.error && err.error.Message && typeof err.error.Message === 'string') { 
            errorMessage = err.error.Message;
        } else if (err.status === 400 && err.error?.errors) { 
            const firstErrorKey = Object.keys(err.error.errors)[0];
            if (firstErrorKey && err.error.errors[firstErrorKey] && err.error.errors[firstErrorKey].length > 0) {
                errorMessage = err.error.errors[firstErrorKey][0];
            }
        }
        alert(errorMessage);
      }
    });
  }
}
