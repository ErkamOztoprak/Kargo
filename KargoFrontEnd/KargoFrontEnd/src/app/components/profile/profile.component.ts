import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CargoRequestService } from '../../services/cargo-request.service';
import { ChangeDetectorRef } from '@angular/core';

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

  myAcceptedParcels: any[] = [];
  isLoadingAcceptedParcels: boolean = false;
  acceptedParcelsError: string | null = null;

  myCompletedParcels: any[] = [];
  isLoadingCompletedParcels: boolean = false;
  completedParcelsError: string | null = null;

  mySentParcels: any[] = [];
  isLoadingSentParcels: boolean = false;
  sentParcelsError: string | null = null; 


  constructor(
    private authService: AuthService,
    private router: Router,
    private cargoRequestService:CargoRequestService,
    private cdr: ChangeDetectorRef
  ) { }

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
   
      }
    }

    this.fetchUserProfile(); 
    this.loadMyAcceptedParcels();
    this.loadMyCompletedParcels();
    this.loadMySentParcels();
    
    const fragment = window.location.hash.replace('#', '');
    if (fragment && ['activity', 'reviews', 'settings'].includes(fragment)) {
      this.activeTab = fragment;
    }
    
  }
  loadMySentParcels(): void {
  this.isLoadingSentParcels = true;
  this.sentParcelsError = null;
  this.mySentParcels = []; 

  this.cargoRequestService.getMyCreatedParcels().subscribe({
    next: (parcels) => {
      this.mySentParcels = parcels;
      this.isLoadingSentParcels = false;
      this.cdr.detectChanges();
    },
    error: (err) => {
      this.sentParcelsError = err.message || 'Gönderdiğiniz kargolar yüklenirken bir hata oluştu.';
      this.isLoadingSentParcels = false;
      console.error('Gönderdiğiniz kargolar yüklenirken hata:', err);
      this.cdr.detectChanges();
    }
  });
}
  getStatusDisplayName(status: string): string {
  switch (status) {
    case 'Pending': return 'Beklemede';
    case 'Accepted': return 'Taşıyıcı Atandı';
    case 'PendingCarrier': return 'Taşıyıcı Bekleniyor';
    case 'InTransit': return 'Yolda';
    case 'Delivered': return 'Teslim Edildi';
    case 'Cancelled': return 'İptal Edildi';
    default: return status;
  }
}

cancelMySentParcel(parcelId: number): void {
    if (!confirm('Bu kargo talebini iptal etmek istediğinizden emin misiniz?')) {
      return;
    }

    this.isLoadingSentParcels = true; 

    this.cargoRequestService.cancelParcelRequest(parcelId).subscribe({
      next: (response) => { 
        alert(response?.message || 'Kargo talebi başarıyla iptal edildi.');
        this.loadMySentParcels(); 
        this.cdr.detectChanges();
      },
      error: (err) => {
        this.isLoadingSentParcels = false; 
        alert(`İptal işlemi sırasında bir hata oluştu: ${err.error?.message || err.message || 'Bilinmeyen bir hata oluştu.'}`);
        console.error('Kargo iptal edilirken hata:', err);
        this.cdr.detectChanges();
      }
    });
  }

  loadMyCompletedParcels(): void {
    this.isLoadingCompletedParcels = true;
    this.completedParcelsError = null;
    this.cargoRequestService.getMyCompletedParcels().subscribe({
      next: (parcels) => {
        this.myCompletedParcels = parcels;
        this.isLoadingCompletedParcels = false;
      },
      error: (err) => {
        // Backend'den gelen spesifik hata mesajını göstermeye çalışalım
        this.completedParcelsError = err.error?.message || err.message || 'Tamamlanan kargolar yüklenirken bir hata oluştu.';
        this.isLoadingCompletedParcels = false;
        console.error('Tamamlanan kargolar yüklenirken hata:', err);
        if (err.status === 401 || err.status === 403) {
            // fetchUserProfile zaten genel bir uyarı ve yönlendirme yapıyor.
            // Bu nedenle burada ek bir alert veya yönlendirme genellikle gereksizdir.
        }
      }
    });
  }

  markAsDelivered(parcelId: number): void {
    if (!confirm('Bu kargoyu teslim ettiğinizi onaylıyor musunuz? Bu işlem geri alınamaz.')) {
      return;
    }
    // İsteğe bağlı: Butonu geçici olarak devre dışı bırakabilir veya bir yükleniyor göstergesi ekleyebilirsiniz.
    this.cargoRequestService.completeCargoDelivery(parcelId).subscribe({
      next: (response) => {
        alert(response?.message || 'Kargo başarıyla teslim edildi olarak işaretlendi!');
        // Listeleri yenilemek için hem kabul edilenleri hem de tamamlananları tekrar yükle
        this.loadMyAcceptedParcels(); 
        this.loadMyCompletedParcels();
      },
      error: (err) => {
        alert(`Hata: ${err.error?.message || err.message || 'Kargo teslim edildi olarak işaretlenemedi.'}`);
        console.error('Kargo teslim etme hatası:', err);
      }
    });
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

  loadMyAcceptedParcels(): void {
    this.isLoadingAcceptedParcels = true;
    this.acceptedParcelsError = null;
    this.cargoRequestService.getMyAcceptedParcels().subscribe({ 
      next: (parcels) => {
        this.myAcceptedParcels = parcels;
        this.isLoadingAcceptedParcels = false;
      },
      error: (err) => {
        this.acceptedParcelsError = err.message || 'Kabul edilen kargolar yüklenirken bir hata oluştu.';
        this.isLoadingAcceptedParcels = false;
        console.error('Kabul edilen kargolar yüklenirken hata:', err);
        
        if (err.status === 401 || err.status === 403) {
             alert('Kargolarınızı görmek için lütfen tekrar giriş yapın.'); // fetchUserProfile zaten benzer bir uyarı veriyor
            
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
        
        // this.fetchUserProfile(); 
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
