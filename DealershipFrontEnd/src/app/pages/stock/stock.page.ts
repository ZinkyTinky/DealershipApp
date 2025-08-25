import { Component, OnInit } from '@angular/core';
import { StockItem } from 'src/app/Models/stock-item';
import { StockService } from 'src/app/services/stock';
import { AlertController, ModalController } from '@ionic/angular';
import { AddStockModalComponent } from 'src/app/components/add-stock-modal/add-stock-modal.component';
import { environment } from 'src/environments/environment';
import { EditStockModalComponent } from 'src/app/components/edit-stock-modal/edit-stock-modal.component';
import { Router } from '@angular/router';

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
    private modalCtrl: ModalController,
    private alertController: AlertController,
    private router: Router
  ) {}

  ngOnInit() {
    this.loadStock(); // Load initial stock list on component init
  }

  /**
   * Displays an Ionic alert with a specified header and message
   * @param header - The title of the alert
   * @param message - The content of the alert
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
   * Handles API errors and displays appropriate messages
   * Redirects to login if unauthorized
   * @param err - The error object from API
   * @param defaultMsg - Default error message to display
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
   * Trigger search functionality and reload stock list
   */
  onSearch() {
    this.pageNumber = 1; // reset to first page on search
    this.loadStock();
  }

  /**
   * Sorts stock list by specified column and toggles ascending/descending
   * @param column - Column name to sort by
   */
  sort(column: string) {
    if (this.sortBy === column) {
      this.sortDesc = !this.sortDesc; // toggle
    } else {
      this.sortBy = column;
      this.sortDesc = false;
    }
    this.loadStock();
  }

  /**
   * Loads stock items from the server with pagination, sorting, and search
   * @param pageNumber - Page number to load
   * @param pageSize - Number of items per page
   */
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
          this.loading = false;
          this.handleError(err, 'Failed to load stock');
        },
      });
  }

  /**
   * Navigate to next page of stock items
   */
  nextPage() {
    if (this.pageNumber < this.totalPages) this.loadStock(this.pageNumber + 1);
  }

  /**
   * Navigate to previous page of stock items
   */
  prevPage() {
    if (this.pageNumber > 1) this.loadStock(this.pageNumber - 1);
  }

  /**
   * Deletes a stock item by id and reloads the list
   * @param id - ID of the stock item to delete
   */
  deleteStockItem(id: number) {
    this.stockService.deleteStock(id).subscribe({
      next: async () => await this.showAlert('Deleted ✅', 'Stock item deleted successfully.'),
      error: (err) => this.handleError(err, 'Failed to delete stock item'),
    });
  }

  /**
   * Opens modal to add a new stock item
   * Reloads stock list after adding
   */
  async openAddModal() {
    const modal = await this.modalCtrl.create({
      component: AddStockModalComponent
    });
    modal.onDidDismiss().then((res) => {
      if (res.data) this.loadStock();
    });
    return await modal.present();
  }

  /**
   * Opens modal to edit an existing stock item
   * Reloads stock list after editing
   * @param stockItem - The stock item to edit
   */
  async openEditModal(stockItem: StockItem) {
    const modal = await this.modalCtrl.create({
      component: EditStockModalComponent,
      componentProps: { stockItem: { ...stockItem } }, // pass a shallow copy
    });
    modal.onDidDismiss().then((res) => {
      if (res.data) this.loadStock();
    });
    return await modal.present();
  }

  /**
   * Toggles sorting by retailPrice column
   */
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
