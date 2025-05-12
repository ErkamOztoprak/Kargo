import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, NgForm } from '@angular/forms';

@Component({
  selector: 'app-cargo-request-create',
  templateUrl: './cargo-request-create.component.html',
  styleUrl: './cargo-request-create.component.css',
  standalone: true,
  imports: [CommonModule, FormsModule]
})
// ...
export class CargoRequestCreateComponent {
  requestData: any = {
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
      proposedFee: null 
    }
  };

 

  submitCargoRequest(form: NgForm) {
    if (form.invalid) {
      alert('Lütfen formdaki tüm zorunlu alanları doğru bir şekilde doldurun.');
      Object.values(form.controls).forEach(control => {
        control.markAsTouched();
      });
      return;
    }
    console.log('Kargo Teslimat Talep Verileri:', this.requestData);
    alert('Teslimat talebi oluşturuldu! (Konsolu kontrol edin)');
    
  }
}
