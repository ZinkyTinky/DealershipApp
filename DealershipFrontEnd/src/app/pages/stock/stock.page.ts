import { Component, OnInit } from '@angular/core';
import { StockItem } from 'src/app/Models/stock-item';
import { StockService } from 'src/app/services/stock';
import { ModalController } from '@ionic/angular';
import { AddStockModalComponent } from 'src/app/components/add-stock-modal/add-stock-modal.component';
import { environment } from 'src/environments/environment';
import { EditStockModalComponent } from 'src/app/components/edit-stock-modal/edit-stock-modal.component';

@Component({
  selector: 'app-stock',
  templateUrl: './stock.page.html',
  styleUrls: ['./stock.page.scss'],
  standalone: false,
})
export class StockPage implements OnInit {
  stock: StockItem[] = [];
  loading = true;
  error: string | null = null;
  apiUrl = environment.apiUrl;

  pageNumber = 1;
  pageSize = 10;
  totalCount = 0;
  totalPages = 0;

  searchTerm: string = '';
  sortBy: string | undefined = undefined;
  sortDesc: boolean = false;

  constructor(
    private stockService: StockService,
    private modalCtrl: ModalController
  ) {}

  ngOnInit() {
    this.loadStock();
  }

  onSearch() {
    this.pageNumber = 1; // reset to first page on search
    this.loadStock();
  }

  sort(column: string) {
    if (this.sortBy === column) {
      this.sortDesc = !this.sortDesc; // toggle
    } else {
      this.sortBy = column;
      this.sortDesc = false;
    }
    this.loadStock();
  }

  loadStock(pageNumber: number = 1, pageSize: number = 10) {
    this.loading = true;

    this.stockService
      .getStock(
        pageNumber,
        pageSize,
        this.searchTerm,
        this.sortBy,
        this.sortDesc
      )
      .subscribe({
        next: (data: any) => {
          this.stock = data.items;
          this.totalCount = data.totalCount;
          this.totalPages = Math.ceil(this.totalCount / this.pageSize);
          this.pageNumber = pageNumber;
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Failed to load stock';
          console.error(err);
          this.loading = false;
        },
      });
  }

  nextPage() {
    if (this.pageNumber < this.totalPages) this.loadStock(this.pageNumber + 1);
  }

  prevPage() {
    if (this.pageNumber > 1) this.loadStock(this.pageNumber - 1);
  }

  deleteStockItem(id: number) {
    this.stockService.deleteStock(id).subscribe({
      next: () => this.loadStock(),
      error: (err) => console.error('Failed to delete stock item', err),
    });
  }

  async openAddModal() {
    const modal = await this.modalCtrl.create({
      component: AddStockModalComponent
    });
    modal.onDidDismiss().then((res) => {
      if (res.data) this.loadStock(); // reload list after adding
    });
    return await modal.present();
  }

  async openEditModal(stockItem: StockItem) {
    const modal = await this.modalCtrl.create({
      component: EditStockModalComponent,
      componentProps: { stockItem: { ...stockItem } },
      // pass a shallow copy
    });
    modal.onDidDismiss().then((res) => {
      if (res.data) this.loadStock(); // refresh after editing
    });
    return await modal.present();
  }

  togglePriceSort() {
  if (this.sortBy === 'retailPrice') {
    this.sortDesc = !this.sortDesc; // toggle Asc/Desc
  } else {
    this.sortBy = 'retailPrice';
    this.sortDesc = false; // start ascending
  }
  this.loadStock();
}

}
