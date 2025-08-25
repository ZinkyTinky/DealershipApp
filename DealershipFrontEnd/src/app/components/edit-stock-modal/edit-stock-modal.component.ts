import { Component, Input } from '@angular/core';
import { AlertController, IonicModule, ModalController } from '@ionic/angular';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { StockItem } from 'src/app/Models/stock-item';
import { StockAccessory } from 'src/app/Models/stock-accessory';
import { StockService } from 'src/app/services/stock';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-edit-stock-modal',
  templateUrl: './edit-stock-modal.component.html',
  styleUrls: ['./edit-stock-modal.component.scss'],
  standalone: true,
  imports: [IonicModule, FormsModule, CommonModule],
})
export class EditStockModalComponent {
  @Input() stockItem!: StockItem; // Populated when opening modal
  newAccessory: StockAccessory = { id: 0, name: '', description: '' };
  removedImageIds: number[] = [];
  removedAccessoryIds: number[] = []; // Track removed accessory IDs
  apiUrl = environment.apiUrl;

  constructor(
    private modalCtrl: ModalController,
    private stockService: StockService,
    private alertController: AlertController,
    private router: Router
  ) {}

  /**
   * Displays an Ionic alert with a header and message
   * @param header - Title of the alert
   * @param message - Content of the alert
   */
  async showAlert(header: string, message: string) {
    const alert = await this.alertController.create({
      header,
      message,
      buttons: ['OK']
    });
    await alert.present();
  }

  /**
   * Handles API errors and shows appropriate alerts
   * Redirects to login if unauthorized
   * @param err - Error object from API call
   * @param defaultMsg - Default message to show if no API message exists
   */
  handleError(err: any, defaultMsg: string) {
    if (err?.status === 401) {
      this.showAlert('Unauthorized ⚠️', 'Your session has expired. Please login again.');
      this.router.navigate(['/login']);
    } else if (err?.error?.message) {
      this.showAlert('Error ⚠️', err.error.message);
    } else {
      this.showAlert('Error ⚠️', defaultMsg);
    }
  }

  /**
   * Adds a new accessory to the stock item
   * Resets the input fields after adding
   */
  addAccessory() {
    if (!this.newAccessory.name.trim()) return;
    this.stockItem.accessories.push({ ...this.newAccessory, id: 0 });
    this.newAccessory = { id: 0, name: '', description: '' };
  }

  /**
   * Removes an accessory from the stock item and tracks removed IDs
   * @param index - Index of the accessory to remove
   */
  removeAccessory(index: number) {
    const acc = this.stockItem.accessories[index];
    if (acc.id) {
      this.removedAccessoryIds.push(acc.id);
    }
    this.stockItem.accessories.splice(index, 1);
  }

  /**
   * Handles file input change and adds selected images to the stock item
   * Limits the number of images to 3
   * @param event - File input change event
   */
  onFileChange(event: any) {
    const files: FileList = event.target.files;
    const maxImages = 3;
    const remainingSlots = maxImages - this.stockItem.images.length;

    for (let i = 0; i < files.length && i < remainingSlots; i++) {
      const file = files[i];
      this.stockItem.images.push({
        id: 0,
        name: file.name,
        imageBinary: file,
        imageUrl: undefined,
        previewUrl: URL.createObjectURL(file)
      });
    }
  }

  /**
   * Removes an image from the stock item and tracks removed image IDs
   * @param index - Index of the image to remove
   */
  removeImage(index: number) {
    const removed = this.stockItem.images[index];
    if (removed.id) {
      this.removedImageIds.push(removed.id);
    }
    this.stockItem.images.splice(index, 1);
  }

  /**
   * Sends the updated stock item data to the server
   * Handles new, existing, and removed accessories/images
   */
  async save() {
    if (!this.stockItem.id) {
      await this.showAlert('Error ⚠️', 'Stock item is missing an ID');
      return;
    }

    const formData = new FormData();

    // Scalars
    formData.append('id', this.stockItem.id.toString());
    formData.append('regNo', this.stockItem.regNo);
    formData.append('make', this.stockItem.make);
    formData.append('model', this.stockItem.model);
    formData.append('modelYear', this.stockItem.modelYear.toString());
    formData.append('kms', this.stockItem.kms.toString());
    formData.append('colour', this.stockItem.colour);
    formData.append('vin', this.stockItem.vin);
    formData.append('retailPrice', this.stockItem.retailPrice.toString());
    formData.append('costPrice', this.stockItem.costPrice.toString());

    // Accessories
    const existingAccessories = this.stockItem.accessories
      .filter(acc => acc.id)
      .map(acc => ({
        id: acc.id,
        name: acc.name,
        description: acc.description ?? ''
      }));
    formData.append('accessories', JSON.stringify(existingAccessories));

    const newAccessories = this.stockItem.accessories
      .filter(acc => !acc.id && acc.name && acc.name.trim());
    if (newAccessories.length > 0) {
      formData.append('newAccessories', JSON.stringify(newAccessories));
    }

    // Removed Accessories
    formData.append('removeAccessoryIds', JSON.stringify(this.removedAccessoryIds));

    // Images
    const existingImages = this.stockItem.images
      .filter(img => img.id)
      .map(img => ({ id: img.id, name: img.name }));
    formData.append('images', JSON.stringify(existingImages));

    this.stockItem.images
      .filter(img => img.imageBinary && !img.id)
      .forEach(img => formData.append('newImages', img.imageBinary as File, img.name));

    formData.append('removeImageIds', JSON.stringify(this.removedImageIds));

    this.stockService.updateStock(this.stockItem.id, formData).subscribe({
      next: async (res) => {
        await this.showAlert('Success ✅', 'Stock updated successfully.');
        this.modalCtrl.dismiss(res);
      },
      error: (err) => this.handleError(err, 'Failed to update stock')
    });
  }

  /**
   * Generates a preview URL for a newly selected image
   * @param img - Image object to preview
   * @returns URL string or undefined
   */
  imgPreview(img: any): string | undefined {
    if (img.imageBinary instanceof File) {
      return URL.createObjectURL(img.imageBinary);
    }
    return undefined;
  }

  /**
   * Closes the modal without saving changes
   */
  cancel() {
    this.modalCtrl.dismiss();
  }
}
