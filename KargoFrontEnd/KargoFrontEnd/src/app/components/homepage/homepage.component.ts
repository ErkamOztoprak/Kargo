import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { CargoRequestService } from '../../services/cargo-request.service';
import { AuthService } from '../../services/auth.service';


@Component({
  selector: 'app-homepage',
  templateUrl: './homepage.component.html',
  styleUrls: ['./homepage.component.css'], 
  imports: [CommonModule, RouterModule]
})
export class HomepageComponent implements OnInit {
  userName: string | null = null;
  logoPath = 'assets/images/logo.png';
  openCargoRequests: any[] = [];
  isLoadingCargoRequests: boolean = false;
  cargoRequestsError: string | null = null;

  selectedCargoId: number | null = null; 
  currentUserId: number | null = null;  
   

  constructor(
    private router: Router, 
    private cargoRequestService: CargoRequestService,
    private authService: AuthService

  ) {}

  ngOnInit(): void {
    const user = this.authService.getCurrentUser();
    if (user) {
      this.userName = user.username;
     
      if (user.id !== null && user.id !== undefined) { 
        const parsedId = parseInt(user.id, 10);
        if (!isNaN(parsedId)) { 
          this.currentUserId = parsedId;
        } else { 
          console.error('Kullanıcı ID\'si geçerli bir sayıya çevrilemedi:', user.id);
          this.currentUserId = null;
        }
      } else { 
        this.currentUserId = null;
      }
    } else {
      
      this.userName = null;
      this.currentUserId = null;
    }
    
    this.loadOpenCargoRequests();  
  }
  scrollToAktifListelemeler(): void {
    const element = document.getElementById('aktif-listelemeler');
    if (element) {
      element.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }

  loadOpenCargoRequests(): void {
    this.isLoadingCargoRequests = true; 
    this.cargoRequestsError = null;
    this.openCargoRequests = []; 

    this.cargoRequestService.getCargoRequests().subscribe({
      next: (data) => {
        this.openCargoRequests = data;
        this.isLoadingCargoRequests = false; 
        console.log('Açık kargo istekleri yüklendi:', this.openCargoRequests);
      },
      error: (err) => {
        this.cargoRequestsError = err.message || 'Kargo istekleri yüklenirken bir hata oluştu.';
        this.isLoadingCargoRequests = false; 
        console.error('Kargo istekleri yükleme hatası:', err);
      }
    });
  }
  toggleDetails(requestId: number): void {
    if (this.selectedCargoId === requestId) {
      this.selectedCargoId = null; // Aynı karta tekrar tıklanırsa detayları kapat
    } else {
      this.selectedCargoId = requestId; // Farklı bir kartın detaylarını aç
    }
  }

  acceptCargoRequest(cargoId: number): void {
    if (!this.currentUserId) {
      alert('Kargoyu alabilmek için giriş yapmış olmalısınız.');
      // İsteğe bağlı: this.router.navigate(['/login']);
      return;
    }

    

    console.log(`Kargo ID ${cargoId} için "Kargoyu Al" isteği gönderiliyor...`);
    // Servis metodunu çağırıyoruz
    this.cargoRequestService.acceptCargo(cargoId).subscribe({
      next: (response) => {
        console.log('Kargo alma başarılı:', response);
        alert(response?.message || 'Kargo başarıyla alındı!'); // Backend'den gelen mesajı kullanabiliriz
        this.loadOpenCargoRequests(); // Kargo listesini yenile (alınan kargo listeden kalkmalı veya durumu değişmeli)
        this.selectedCargoId = null;  // Açık olan detayları kapat
      },
      error: (err) => {
        console.error('Kargo alma hatası:', err);
        alert(`Hata: ${err.message || 'Kargo alınamadı. Lütfen tekrar deneyin.'}`);
      }
    });
  }

  logout(): void {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.userName = null;
    this.router.navigate(['/login']);
  }
}
