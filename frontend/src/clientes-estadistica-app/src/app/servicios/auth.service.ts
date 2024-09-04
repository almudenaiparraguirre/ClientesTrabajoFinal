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

  getUserEmail(): string {
    return localStorage.getItem('email'); 
  }

  // Método para obtener el token
  getToken(): string | null {
    const token = localStorage.getItem('token');
    //console.log('Token recuperado:', token); // Esto debería mostrar el token real
    return token;
  }

  // Método para establecer el token
  setToken(token: string): void {
    localStorage.setItem('token', token); // Almacena el token en el localStorage
  }

  // Método para eliminar el token
  clearToken(): void {
    localStorage.removeItem('token'); // Elimina el token del localStorage
  }
}
