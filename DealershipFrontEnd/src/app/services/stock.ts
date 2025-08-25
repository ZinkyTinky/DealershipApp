import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { StockItem } from '../Models/stock-item';
import { AuthService } from './auth-service';

@Injectable({
  providedIn: 'root',
})
export class StockService {
  private apiUrl = `${environment.apiUrl}/Stock`;

  constructor(private http: HttpClient, private authService: AuthService) {}

  // ------------------------
  // Public calls (no token)
  // ------------------------
  getStock(
    pageNumber: number = 1,
    pageSize: number = 10,
    searchTerm?: string,
    sortBy?: string,
    sortDesc: boolean = false
  ): Observable<any> {
    let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString())
      .set('sortDesc', sortDesc.toString());

    if (searchTerm) params = params.set('search', searchTerm);
    if (sortBy) params = params.set('sortBy', sortBy);

    // Use /search endpoint if searching or sorting
    const url = searchTerm || sortBy ? `${this.apiUrl}/search` : `${this.apiUrl}/all`;

    return this.http.get<any>(url, { params });
}


  getStockById(id: string): Observable<StockItem> {
    return this.http.get<StockItem>(`${this.apiUrl}/${id}`);
  }

  // ------------------------
  // Authenticated calls
  // ------------------------
  addStock(item: StockItem): Observable<StockItem> {
    const token = this.authService.getToken();
    if (!token) throw new Error('User is not authenticated');

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const formData = this.buildFormData(item);
    return this.http.post<StockItem>(`${this.apiUrl}`, formData, { headers });
  }

  updateStock(id: number, form: FormData): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put(`${this.apiUrl}/${id}`, form, { headers });
  }

  deleteStock(id: number): Observable<any> {
    const token = this.authService.getToken();
    if (!token) throw new Error('User is not authenticated');

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.apiUrl}/${id}`, { headers });
  }

  // ------------------------
  // Helper
  // ------------------------
  private buildFormData(
    item: StockItem,
    removeAccessoryIds: number[] = [],
    removeImageIds: number[] = []
  ): FormData {
    const formData = new FormData();

    formData.append('regNo', item.regNo);
    formData.append('make', item.make);
    formData.append('model', item.model);
    formData.append('modelYear', item.modelYear.toString());
    formData.append('kms', item.kms.toString());
    formData.append('colour', item.colour);
    formData.append('vin', item.vin);
    formData.append('retailPrice', item.retailPrice.toString());
    formData.append('costPrice', item.costPrice.toString());

    //Existing Accessories
    const existingAccessories = item.accessories
      .filter((acc) => acc.id)
      .map((acc) => ({
        id: acc.id,
        name: acc.name,
        description: acc.description ?? '',
      }));
    formData.append('accessories', JSON.stringify(existingAccessories));

    //new Accessories
    const newAccessories = item.accessories
      .filter(acc => !acc.id && acc.name && acc.name.trim())
      .map(acc => ({
        id: 0,
        name: acc.name,
        description: acc.description ?? ''
      }));

    if (newAccessories.length > 0) {
      formData.append('newAccessories', JSON.stringify(newAccessories));
    }


    // --- Removed Accessories ---
    formData.append('removeAccessoryIds', JSON.stringify(removeAccessoryIds));

    // --- Existing Images ---
    const existingImages = item.images
      .filter((img) => img.id)
      .map((img) => ({ id: img.id, name: img.name }));
    formData.append('images', JSON.stringify(existingImages));

    // --- New Images ---
    item.images
      .filter((img) => img.imageBinary && !img.id)
      .forEach((img) =>
        formData.append('newImages', img.imageBinary as File, img.name)
      );

    // --- Removed Images ---
    formData.append('removeImageIds', JSON.stringify(removeImageIds));

    return formData;
  }
}
