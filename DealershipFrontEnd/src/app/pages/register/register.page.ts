import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from 'src/app/services/auth-service';
import { RegisterDto } from 'src/app/Models/DTOs/register-dto';

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
    private fb: FormBuilder
  ) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required]
    });
  }

  ngOnInit() {}

  onSubmit() {
    if (this.registerForm.invalid) return;

    this.isSubmitting = true;
    const dto: RegisterDto = this.registerForm.value;

    this.authService.register(dto).subscribe({
      next: (res) => {
        this.successMessage = 'Registration successful!';
        this.errorMessage = '';
        this.isSubmitting = false;
        this.router.navigate(['/tabs/stock']); // Redirect after successful register
      },
      error: (err) => {
        this.errorMessage = err?.error?.message || 'Registration failed.';
        this.successMessage = '';
        this.isSubmitting = false;
      }
    });
  }

}
