// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { LoginDto } from '../Models/DTOs/login-dto';
import { RegisterDto } from '../Models/DTOs/register-dto';
import { HttpHeaders } from '@angular/common/http';

interface AuthResponse {
  token: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private apiUrl = environment.apiUrl; // <-- use environment variable
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  public isLoggedIn$ = this.loggedIn.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }

  login(dto: LoginDto, token?: string): Observable<AuthResponse> {
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    return this.http.post<AuthResponse>(`${this.apiUrl}/UserAuth/login`, dto, { headers })
      .pipe(
        tap(res => {
          localStorage.setItem('token', res.token);
          this.loggedIn.next(true);
        })
      );
  }


  register(dto: RegisterDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/UserAuth/register`, dto)
      .pipe(
        tap(res => {
          localStorage.setItem('token', res.token);
          this.loggedIn.next(true);
        })
      );
  }

  logout(): void {
    localStorage.removeItem('token');
    this.loggedIn.next(false);
    this.router.navigate(['/tabs/login']);
  }

  getToken(): string | null {
    return localStorage.getItem('token');
  }

}
