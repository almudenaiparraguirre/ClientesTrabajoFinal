import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { catchError, map, Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  
  private readonly URL = "https://localhost:7107/api/Account/";
  public currentUserEmail: string; // Ajusta a tu endpoint real

  constructor(private http: HttpClient) {}

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token'); // Comprueba si hay un token en el almacenamiento local
  }
}
