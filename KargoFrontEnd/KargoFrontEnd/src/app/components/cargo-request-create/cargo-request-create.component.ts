import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';
import { AuthService, UserProfile } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CargoRequestService } from '../../services/cargo-request.service';
import { HttpClient } from '@angular/common/http';


interface SenderInfo{
  name: string;
  contactInfo: string;
}
interface ReceiverInfo{
  name: string;
  contactInfo: string;
}
interface CargoInfo{
  description: string;
  pickupLocation: string;
  deliveryLocation: string;
  specialInstructions: string;
}
interface PaymentInfo{
  proposedFee?: number;
}
interface ParcelCreateRequest{
  sender: SenderInfo;
  receiver: ReceiverInfo;
  cargo: CargoInfo;
  payment: PaymentInfo;
  // submittedByUsername?: string;
 
}

@Component({
  selector: 'app-cargo-request-create',
  templateUrl: './cargo-request-create.component.html',
  styleUrl: './cargo-request-create.component.css',
  standalone: true,
  
  imports: [CommonModule, FormsModule ] 
})
// ...
export class CargoRequestCreateComponent {
  private baseUrl = 'https://localhost:44318/api/Parcels';
  requestData: ParcelCreateRequest = {
    sender: {
      name: '', 
      contactInfo: '' 
    },
    receiver: {
      name: '',
      contactInfo: '' 
    },
    cargo: {
      description: '', 
      pickupLocation: '', 
      deliveryLocation: '', 
      specialInstructions: '' 
    },
    payment: {
      proposedFee: undefined 
    }
  };

  isLoading: boolean = false;
  loggedInUserName: string | null = null;
  message: string | null = null;
  messageType: 'success' | 'error' = 'success';

  constructor(
    private authService: AuthService,
    private router: Router,
    private cargoRequestService: CargoRequestService,
    private http: HttpClient,
  ) { }

  
  

  ngOnInit(): void{
    if(!this.authService.isAuthenticated()){
      this.router.navigate(['/login']);
      return;
    }
    this.initializeSenderName();
  }

  private initializeSenderName():void {
    const currentUser = this.authService.getCurrentUser();
    this.requestData.sender.name=this.formatSenderName(currentUser);
  }

  private formatSenderName(user: UserProfile|null): string {
    if(!user) return '';
    let nameParts: string[] = [];
    if(user.firstName){
      nameParts.push(user.firstName);
    }
    if(user.lastName){
      nameParts.push(user.lastName);
    }
    if(nameParts.length > 0){
      return nameParts.join(' ');
    } 
    return user.username || '';
  }

  private resetRequestData():void{
    const currentUser = this.authService.getCurrentUser();  
    this.requestData={
      sender: { name: this.formatSenderName(currentUser), contactInfo: '' },
      receiver: { name: '', contactInfo: '' },
      cargo: { description: '', pickupLocation: '', deliveryLocation: '', specialInstructions: '' },
      payment: { proposedFee: undefined }
    }
  }
  

  submitCargoRequest(form: NgForm) {
    this.message = null;

    if (form.invalid) {
      this.messageType = 'error';
      this.message = 'Lutfen formdaki tüm zorunlu alanları doğru bir şekilde doldurun.';
      
      Object.values(form.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }
    if(!this.authService.isAuthenticated()){
      this.messageType = 'error';
      this.message = 'Öncelikle giriş yapmalısınız.';
      this.router.navigate(['/login']);
      return;
    }
    
    const currentUser = this.authService.getCurrentUser();
    if (currentUser && !this.requestData.sender.name) { 
      this.requestData.sender.name = `${currentUser.firstName} ${currentUser.lastName}`;
    }

    this.isLoading = true;

    const payload: ParcelCreateRequest = { ...this.requestData };

    this.cargoRequestService.createCargoRequest(payload).subscribe({
      next: (response: { message?: string }) => {
      this.isLoading = false;
      this.messageType = 'success';
      this.message = response.message || 'Kargo talebi basarıyla oluşturuldu.';
      console.log('Kargo talebi oluşturuldu:', response);
      console.log('Gönderilen Kargo Talep Verileri (Başarılı):', payload);
      form.resetForm();
      this.resetRequestData(); 

      
      },
      error: (errorResponse: any) => {
        this.isLoading = false;
        this.messageType = 'error';
        if (errorResponse?.error?.errors) { 
            const validationErrors = errorResponse.error.errors;
            const errorMessages = Object.values(validationErrors).flat();
            this.message = errorMessages.join('\n');
        } else if (errorResponse?.error?.message) {
            this.message = errorResponse.error.message;
        } else if (errorResponse?.message) {
            this.message = errorResponse.message;
        } else if (typeof errorResponse?.error === 'string') {
            this.message = errorResponse.error;
        }
         else {
          this.message = 'Kargo talebi oluşturulurken bir hata oluştu. Lütfen tekrar deneyin.';
        }
        console.error('Kargo talebi oluşturulurken hata oluştu:', errorResponse);
        console.log('Gönderilen Kargo Talep Verileri (Hatalı):', payload);
      },
    });
    console.log('Kargo Teslimat Talep Verileri:', this.requestData);
    alert('Teslimat talebi oluşturuldu! (Konsolu kontrol edin)'); 
    
  }
}
