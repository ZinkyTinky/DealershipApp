import { Component } from '@angular/core';
import { IonicModule, ModalController } from '@ionic/angular';
import { StockService } from 'src/app/services/stock';
import { StockItem } from 'src/app/Models/stock-item';
import { StockAccessory } from 'src/app/Models/stock-accessory';
import { IonHeader } from "@ionic/angular/standalone";
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

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
  token: string = ''; // set JWT token here or pass via modalProps

  constructor(
    private modalCtrl: ModalController,
    private stockService: StockService
  ) {}

  addAccessory() {
    if (this.newAccessory.name.trim() === '') return;
    this.stockItem.accessories.push({ ...this.newAccessory });
    this.newAccessory = { id: 0, name: '', description: '' };
  }

  removeAccessory(index: number) {
    this.stockItem.accessories.splice(index, 1);
  }

  onFileChange(event: any) {
    const files: FileList = event.target.files;
    for (let i = 0; i < files.length && i < 3; i++) {
      const file = files[i];
      this.stockItem.images.push({
        id: 0,
        name: `Image ${i + 1}`, // default name, can be edited by user
        imageBinary: file
      });
    }
  }
  removeImage(index: number) {
    this.stockItem.images.splice(index, 1);
  }

  async save() {
    this.stockService.addStock(this.stockItem).subscribe({
      next: (res) => this.modalCtrl.dismiss(res),
      error: (err) => console.error('Failed to add stock', err)
    });
  }

  cancel() {
    this.modalCtrl.dismiss();
  }
}
