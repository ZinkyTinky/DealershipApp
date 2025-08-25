import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
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
    // Any initialization logic can go here
  }

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit(form: any) {
    if (!form.valid) return;

    const dto: LoginDto = { username: this.username, password: this.password };

    this.authService.login(dto).subscribe({
      next: () => this.router.navigate(['/tabs/stock']),
      error: (err) => this.errorMessage = 'Login failed. Please check your credentials.'
    });
  }
}
