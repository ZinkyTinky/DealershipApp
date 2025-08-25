import { Component } from '@angular/core';
import { AlertController, IonicModule, ModalController } from '@ionic/angular';
import { StockService } from 'src/app/services/stock';
import { StockItem } from 'src/app/Models/stock-item';
import { StockAccessory } from 'src/app/Models/stock-accessory';
import { IonHeader } from "@ionic/angular/standalone";
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

@Component({
  selector: 'app-add-stock-modal',
  templateUrl: './add-stock-modal.component.html',
  styleUrls: ['./add-stock-modal.component.scss'],
  standalone: true,
  imports: [IonicModule, FormsModule, CommonModule],
})
export class AddStockModalComponent {
  stockItem: StockItem = {
    id: 0,
    regNo: '',
    make: '',
    model: '',
    modelYear: new Date().getFullYear(),
    kms: 0,
    colour: '',
    vin: '',
    retailPrice: 0,
    costPrice: 0,
    accessories: [],
    images: [],
    dtCreated: new Date().toISOString(),
    dtUpdated: new Date().toISOString()
  };

  newAccessory: StockAccessory = { id: 0, name: '', description: '' };
  token: string = ''; 

  constructor(
    private modalCtrl: ModalController,
    private stockService: StockService,
    private alertController: AlertController,
    private router: Router
  ) {}

  /**
   * Displays an Ionic alert with specified header and message
   * @param header - Title of the alert
   * @param message - Content message of the alert
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
    if (this.newAccessory.name.trim() === '') return;
    this.stockItem.accessories.push({ ...this.newAccessory });
    this.newAccessory = { id: 0, name: '', description: '' };
  }

  /**
   * Removes an accessory from the stock item
   * @param index - Index of the accessory to remove
   */
  removeAccessory(index: number) {
    this.stockItem.accessories.splice(index, 1);
  }

  /**
   * Handles file input change and adds selected images to the stock item
   * Limits the number of images to 3
   * @param event - File input change event
   */
  onFileChange(event: any) {
    const files: FileList = event.target.files;
    for (let i = 0; i < files.length && i < 3; i++) {
      const file = files[i];
      this.stockItem.images.push({
        id: 0,
        name: `Image ${i + 1}`,
        imageBinary: file
      });
    }
  }

  /**
   * Removes an image from the stock item
   * @param index - Index of the image to remove
   */
  removeImage(index: number) {
    this.stockItem.images.splice(index, 1);
  }

  /**
   * Sends the stock item data to the server to save
   * Closes modal and shows success alert if successful
   */
  async save() {
    this.stockService.addStock(this.stockItem).subscribe({
      next: async (res) => {
        await this.modalCtrl.dismiss(res);
        await this.showAlert('Success ✅', 'Stock item added successfully.');
      },
      error: (err) => this.handleError(err, 'Failed to add stock')
    });
  }

  /**
   * Dismisses the modal without saving changes
   */
  cancel() {
    this.modalCtrl.dismiss();
  }
}
