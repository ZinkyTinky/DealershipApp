import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth-service';
import { RegisterDto } from 'src/app/Models/DTOs/register-dto';
import { AlertController } from '@ionic/angular';

@Component({
  selector: 'app-register',
  templateUrl: './register.page.html',
  styleUrls: ['./register.page.scss'],
  standalone: false,
})
export class RegisterPage implements OnInit {
  registerForm: FormGroup;
  errorMessage: string = '';
  successMessage: string = '';
  isSubmitting: boolean = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private fb: FormBuilder,
    private alertController: AlertController
  ) {
    // Initialize the registration form with validators
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
    });
  }

  ngOnInit() {
  }

  /**
   * Displays an Ionic alert with a specified header and message.
   * @param header - The title of the alert
   * @param message - The content of the alert
   */
  async showAlert(header: string, message: string) {
    const alert = await this.alertController.create({
      header: header,
      message: message,
      buttons: ['OK'],
    });
    await alert.present();
  }

  /**
   * Handles form submission for user registration.
   * Validates the form, constructs the RegisterDto, and calls AuthService.register().
   * Displays success or error messages and redirects on successful registration.
   */
  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isSubmitting = true;
    const dto: RegisterDto = this.registerForm.value;

    this.authService.register(dto).subscribe({
      next: (res) => {
        this.isSubmitting = false;
        this.showAlert('Success ✅', 'Registration successful!');
        this.router.navigate(['/tabs/stock']); // Redirect after successful register
      },
      error: (err) => {
        this.isSubmitting = false;
        let msg = 'Registration failed. Please try again.';
        if (err?.status === 0) {
          msg = 'Cannot connect to server. Please try again later.';
        } else if (err?.error?.message) {
          msg = err.error.message;
        }
        this.showAlert('Registration Error ⚠️', msg);
      },
    });
  }
}
