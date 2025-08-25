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

  private apiUrl = environment.apiUrl;
  private loggedIn = new BehaviorSubject<boolean>(this.hasToken());
  public isLoggedIn$ = this.loggedIn.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  /**
   * Checks if a JWT token exists in localStorage
   * @returns boolean - true if token exists, false otherwise
   */
  private hasToken(): boolean {
    return !!localStorage.getItem('token');
  }

  /**
   * Logs in a user by sending credentials to the API
   * Stores the returned JWT token in localStorage and updates login state
   * @param dto - Object containing username and password
   * @param token - Optional token to use in Authorization header
   * @returns Observable<AuthResponse>
   */
  login(dto: LoginDto, token?: string): Observable<AuthResponse> {
    let headers = new HttpHeaders({ 'Content-Type': 'application/json' });

    if (token) {
      headers = headers.set('Authorization', `Bearer ${token}`);
    }

    return this.http.post<AuthResponse>(`${this.apiUrl}/UserAuth/login`, dto, { headers })
      .pipe(
        tap(res => {
          if (res && res.token) {
          localStorage.setItem('token', res.token);
          this.loggedIn.next(true);
        } else {
          this.loggedIn.next(false);
        }
        })
      );
  }

  /**
   * Registers a new user by sending registration data to the API
   * Stores the returned JWT token in localStorage and updates login state
   * @param dto - Object containing registration information
   * @returns Observable<AuthResponse>
   */
  register(dto: RegisterDto): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/UserAuth/register`, dto)
      .pipe(
        tap(res => {
          localStorage.setItem('token', res.token);
          this.loggedIn.next(true);
        })
      );
  }

  /**
   * Logs out the user by removing the token from localStorage
   * Updates login state and navigates to the login page
   */
  logout(): void {
    localStorage.removeItem('token');
    this.loggedIn.next(false);
    this.router.navigate(['/tabs/login']);
  }

  /**
   * Retrieves the stored JWT token from localStorage
   * @returns string | null - the token if it exists, otherwise null
   */
  getToken(): string | null {
    return localStorage.getItem('token');
  }
}
