import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AlertController } from '@ionic/angular';
import { LoginDto } from 'src/app/Models/DTOs/login-dto';
import { AuthService } from 'src/app/services/auth-service';

@Component({
  selector: 'app-login',
  templateUrl: './login.page.html',
  styleUrls: ['./login.page.scss'],
  standalone: false,
})
export class LoginPage implements OnInit {

  username: string = '';
  password: string = '';
  errorMessage: string = '';

  ngOnInit() {
  }

  constructor(
    private authService: AuthService, 
    private router: Router, 
    private alertController: AlertController
  ) {}

  /**
   * Handles form submission for login.
   * Validates the form, constructs the login DTO, and calls the AuthService.
   * Navigates to the stock page on successful login.
   * Displays an error alert with descriptive messages if login fails.
   */
  onSubmit(form: any) {
    if (!form.valid) return;

    const dto: LoginDto = { username: this.username, password: this.password };

    this.authService.login(dto).subscribe({
      next: () => this.router.navigate(['/tabs/stock']),
      error: (err) => {
        let msg = 'Login failed. Please check your credentials.';
        if (err?.status === 0) {
          msg = 'Cannot connect to server. Please try again later.';
        } else if (err?.status === 401) {
          msg = 'Invalid username or password.';
        } else if (err?.error?.message) {
          msg = err.error.message; // API-provided error
        }
        this.showErrorAlert(msg);
      }
    });
  }

  /**
   * Shows an Ionic alert with a given error message.
   * @param message - The message to display in the alert
   */
  async showErrorAlert(message: string) {
    const alert = await this.alertController.create({
      header: 'Login Error ⚠️',
      message: message,
      buttons: ['OK']
    });
    await alert.present();
  }
}
