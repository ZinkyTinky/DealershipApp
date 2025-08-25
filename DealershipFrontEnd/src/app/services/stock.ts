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

  /**
   * Retrieves a paginated list of stock items.
   * Can optionally search and sort.
   * @param pageNumber - page number to retrieve
   * @param pageSize - number of items per page
   * @param searchTerm - optional search term
   * @param sortBy - optional column to sort by
   * @param sortDesc - whether to sort descending
   * @returns Observable<any> containing stock items and total count
   */
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

    const url = searchTerm || sortBy ? `${this.apiUrl}/search` : `${this.apiUrl}/all`;

    return this.http.get<any>(url, { params });
  }

  /**
   * Retrieves a single stock item by its ID
   * @param id - ID of the stock item
   * @returns Observable<StockItem>
   */
  getStockById(id: string): Observable<StockItem> {
    return this.http.get<StockItem>(`${this.apiUrl}/${id}`);
  }

  /**
   * Adds a new stock item. Requires authentication.
   * @param item - StockItem to add
   * @returns Observable<StockItem>
   */
  addStock(item: StockItem): Observable<StockItem> {
    const token = this.authService.getToken();
    if (!token) throw new Error('User is not authenticated');

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    const formData = this.buildFormData(item);
    return this.http.post<StockItem>(`${this.apiUrl}`, formData, { headers });
  }

  /**
   * Updates an existing stock item. Requires authentication.
   * @param id - ID of the stock item
   * @param form - FormData containing updated item information
   * @returns Observable<any>
   */
  updateStock(id: number, form: FormData): Observable<any> {
    const token = this.authService.getToken();
    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);

    return this.http.put(`${this.apiUrl}/${id}`, form, { headers });
  }

  /**
   * Deletes a stock item by its ID. Requires authentication.
   * @param id - ID of the stock item
   * @returns Observable<any>
   */
  deleteStock(id: number): Observable<any> {
    const token = this.authService.getToken();
    if (!token) throw new Error('User is not authenticated');

    const headers = new HttpHeaders().set('Authorization', `Bearer ${token}`);
    return this.http.delete(`${this.apiUrl}/${id}`, { headers });
  }

  /**
   * Helper method to convert a StockItem object into FormData
   * including existing/new/removed images and accessories.
   * @param item - StockItem to convert
   * @param removeAccessoryIds - optional list of accessory IDs to remove
   * @param removeImageIds - optional list of image IDs to remove
   * @returns FormData
   */
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

    const existingAccessories = item.accessories
      .filter((acc) => acc.id)
      .map((acc) => ({
        id: acc.id,
        name: acc.name,
        description: acc.description ?? '',
      }));
    formData.append('accessories', JSON.stringify(existingAccessories));

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

    formData.append('removeAccessoryIds', JSON.stringify(removeAccessoryIds));

    const existingImages = item.images
      .filter((img) => img.id)
      .map((img) => ({ id: img.id, name: img.name }));
    formData.append('images', JSON.stringify(existingImages));

    item.images
      .filter((img) => img.imageBinary && !img.id)
      .forEach((img) =>
        formData.append('newImages', img.imageBinary as File, img.name)
      );

    formData.append('removeImageIds', JSON.stringify(removeImageIds));

    return formData;
  }
}
