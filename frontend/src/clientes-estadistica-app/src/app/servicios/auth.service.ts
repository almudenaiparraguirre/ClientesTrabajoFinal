import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  constructor() {}

  // Método para verificar si el usuario está autenticado
  isLoggedIn(): boolean {
    return !!localStorage.getItem('token'); // Comprueba si hay un token en el almacenamiento local
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
