import { Injectable } from "@angular/core";
import { HttpClient,HttpErrorResponse,HttpHeaders} from "@angular/common/http";
import { Observable, throwError } from "rxjs";
import { catchError } from "rxjs/operators";
import {AuthService} from "./auth.service";

@Injectable({
    providedIn: 'root'
})
export class CargoRequestService {
    private baseUrl:string="https://localhost:44318/api/Parcels"; 

    constructor(
        private http: HttpClient,
        private authService: AuthService
    ){}

    // @param requestData 
    // @returns

    createCargoRequest(requestData: any): Observable<any> {
        const url = this.baseUrl;

        const token = this.authService.getToken();
        let headers= new HttpHeaders({
            'Content-Type': 'application/json',
        });

        if (token) {
            headers = headers.append('Authorization', `Bearer ${token}`);
        }
        return this.http.post(url, requestData, { headers })
            .pipe(
                catchError(this.handleError)
            );
    } 
    getCargoRequests(): Observable<any> {
        const url = this.baseUrl;
        const token = this.authService.getToken();
        let headers= new HttpHeaders({
            'Content-Type': 'application/json',
        });

        if (token) {
            headers = headers.append('Authorization', `Bearer ${token}`);
        }
        return this.http.get(url, { headers })
            .pipe(
                catchError(this.handleError)
            );
    }

     /**
     * Gets the parcels accepted by the currently logged-in user.
     * These are typically in-progress deliveries for the carrier.
     * @returns An Observable of the HTTP response containing accepted parcels.
     */
    getMyAcceptedParcels(): Observable<any> { // Metot adı güncellendi
        const token = this.authService.getToken();
        if (!token) {
          return throwError(() => new Error('Kullanıcı girişi yapılmamış veya token bulunamadı.'));
        }

        const headers = new HttpHeaders({
          'Content-Type': 'application/json',
          'Authorization': `Bearer ${token}`
        });

        // Yeni backend endpoint'inizin URL'i
        const acceptedParcelsUrl = `${this.baseUrl}/my-accepted`; 

        return this.http.get(acceptedParcelsUrl, { headers: headers })
          .pipe(
            catchError(this.handleError)
          );
    }
    
    // ... (handleError metodu ve diğer metotlar) ...


    /**
     * Accepts a cargo request.
     * @param cargoId The ID of the cargo to accept.
     * @returns An Observable of the HTTP response.
     */
    acceptCargo(cargoId: number): Observable<any> {
        const token = this.authService.getToken();
        if (!token) {
          // Kullanıcı giriş yapmamışsa veya token yoksa hata döndür
          return throwError(() => new Error('Kullanıcı girişi yapılmamış veya token bulunamadı.'));
        }

        const headers = new HttpHeaders({
          'Content-Type': 'application/json', 
          'Authorization': `Bearer ${token}`
        });

        // /api/Parcels/{id}/accept
        const acceptUrl = `${this.baseUrl}/${cargoId}/accept`; 
        
        return this.http.put(acceptUrl, {}, { headers: headers }) 
          .pipe(
            catchError(this.handleError)
          );
        
        
    }

      /**
   * Gets the parcels completed by the currently logged-in user.
   * @returns An Observable of the HTTP response containing completed parcels.
   */
  getMyCompletedParcels(): Observable<any> { 
      const token = this.authService.getToken();
      if (!token) {
        return throwError(() => new Error('Kullanıcı girişi yapılmamış veya token bulunamadı.'));
      }
      const headers = new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${token}`
      });
      // Backend endpoint'inizin URL yapısına göre bu kısmı düzenleyin
      const completedParcelsUrl = `${this.baseUrl}/my-completed`; 

      return this.http.get<any[]>(completedParcelsUrl, { headers: headers }) // <any[]> tipini ekleyebilirsiniz
        .pipe(
          catchError(this.handleError)
        );
  }

 /**
   * Cancels a cargo request created by the current user.
   * Assumes a backend endpoint like PUT /api/Parcels/{parcelId}/cancel
   * @param parcelId The ID of the parcel to cancel.
   * @returns An Observable of the HTTP response.
   */
  cancelParcelRequest(parcelId: number): Observable<any> {
    const token = this.authService.getToken();
    if (!token) {
      return throwError(() => new Error('Kullanıcı girişi yapılmamış veya token bulunamadı.'));
    }
    const headers = new HttpHeaders({
      'Content-Type': 'application/json', 
      'Authorization': `Bearer ${token}`
    });
   
    const cancelUrl = `${this.baseUrl}/${parcelId}/cancel`; 

    
    return this.http.put<any>(cancelUrl, {}, { headers: headers })
    
      .pipe(
        catchError(this.handleError)
      );
  }

  getMyCreatedParcels(): Observable<any[]> {
    const token = this.authService.getToken();
    if (!token) {
      return throwError(() => new Error('Kullanıcı girişi yapılmamış veya token bulunamadı.'));
    }
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
    
    const url = `${this.baseUrl}/my-created`; 

    return this.http.get<any[]>(url, { headers })
      .pipe(
        catchError(this.handleError)
      );
  }

  /**
   * Marks a cargo as completed by the current user.
   * @param parcelId The ID of the parcel to mark as completed.
   * @returns An Observable of the HTTP response.
   */
  completeCargoDelivery(parcelId: number): Observable<any> { 
      const token = this.authService.getToken();
      if (!token) {
        return throwError(() => new Error('Kullanıcı girişi yapılmamış veya token bulunamadı.'));
      }
      const headers = new HttpHeaders({
        'Content-Type': 'application/json', // PUT isteği için body boş olsa bile Content-Type gerekebilir
        'Authorization': `Bearer ${token}`
      });
      // Backend endpoint'inizin URL yapısına göre bu kısmı düzenleyin
      // Örneğin: /api/Parcels/{parcelId}/complete
      const completeUrl = `${this.baseUrl}/${parcelId}/complete`; 

      return this.http.put<any>(completeUrl, {}, { headers: headers }) 
        .pipe(
          catchError(this.handleError)
        );
  }
    
    /**
     * @param error The HTTP error response.
     * @returns 
     */
    private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Bilinmeyen bir hata oluştu!';
    if (error.error instanceof ErrorEvent) {
      // İstemci tarafı veya ağ hatası.
      errorMessage = `Bir hata oluştu: ${error.error.message}`;
    } else {
      // Backend bir hata kodu döndürdü.
      // Yanıt gövdesi hatanın ipuçlarını içerebilir.
      if (error.status === 0) {
        errorMessage = 'Sunucuya ulaşılamıyor. Lütfen ağ bağlantınızı kontrol edin veya daha sonra tekrar deneyin.';
      } else if (error.error && error.error.message) {
        errorMessage = `Sunucu hatası (Kod: ${error.status}): ${error.error.message}`;
      } else if (error.error && typeof error.error === 'string') {
        errorMessage = `Sunucu hatası (Kod: ${error.status}): ${error.error}`;
      }
       else {
        errorMessage = `Sunucu hatası (Kod: ${error.status}): ${error.statusText}`;
      }
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage)); // Kullanıcıya gösterilecek hata mesajı olarak Error objesi döndür
  }


}
