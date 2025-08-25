import { Component, Input } from '@angular/core';
import { IonicModule, ModalController } from '@ionic/angular';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { StockItem } from 'src/app/Models/stock-item';
import { StockAccessory } from 'src/app/Models/stock-accessory';
import { StockService } from 'src/app/services/stock';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-edit-stock-modal',
  templateUrl: './edit-stock-modal.component.html',
  styleUrls: ['./edit-stock-modal.component.scss'],
  standalone: true,
  imports: [IonicModule, FormsModule, CommonModule],
})
export class EditStockModalComponent {
  @Input() stockItem!: StockItem; // populated when opening modal
  newAccessory: StockAccessory = { id: 0, name: '', description: '' };
  removedImageIds: number[] = [];
  removedAccessoryIds: number[] = []; // <-- track removed accessory IDs
  apiUrl = environment.apiUrl;


  constructor(
    private modalCtrl: ModalController,
    private stockService: StockService
  ) {}

  // Add a new accessory
  addAccessory() {
    if (!this.newAccessory.name.trim()) return;
    this.stockItem.accessories.push({ ...this.newAccessory, id: 0 }); // new items get id 0
    this.newAccessory = { id: 0, name: '', description: '' };
  }

  // Remove an accessory
  removeAccessory(index: number) {
    const acc = this.stockItem.accessories[index];
    if (acc.id) {
      this.removedAccessoryIds.push(acc.id); // track removed IDs
    }
    this.stockItem.accessories.splice(index, 1);
  }

  // Handle image file selection
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

  removeImage(index: number) {
    const removed = this.stockItem.images[index];
    if (removed.id) {
      this.removedImageIds.push(removed.id);
    }
    this.stockItem.images.splice(index, 1);
  }

  // Save updated stock item
  async save() {
    if (!this.stockItem.id) {
      console.error('Stock item is missing an id');
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

    // --- Accessories ---
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

    // --- Removed Accessories ---
    formData.append('removeAccessoryIds', JSON.stringify(this.removedAccessoryIds));

    // --- Images ---
    const existingImages = this.stockItem.images
      .filter(img => img.id)
      .map(img => ({ id: img.id, name: img.name }));
    formData.append('images', JSON.stringify(existingImages));

    this.stockItem.images
      .filter(img => img.imageBinary && !img.id)
      .forEach(img => formData.append('newImages', img.imageBinary as File, img.name));

    formData.append('removeImageIds', JSON.stringify(this.removedImageIds));

    this.stockService
      .updateStock(this.stockItem.id, formData)
      .subscribe({
        next: (res) => {
          console.log('Stock updated successfully', res);
          this.modalCtrl.dismiss(res);
        },
        error: (err) => console.error('Failed to update stock', err),
      });
  }

  imgPreview(img: any): string | undefined {
    if (img.imageBinary instanceof File) {
      return URL.createObjectURL(img.imageBinary);
    }
    return undefined;
  }


  cancel() {
    this.modalCtrl.dismiss();
  }
}
