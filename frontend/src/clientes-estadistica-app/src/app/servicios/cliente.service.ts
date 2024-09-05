import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Cliente } from '../clases/cliente'; // Asegúrate de tener una clase Cliente
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {

  //private readonly URL = "https://localhost:44339/api/Cliente";
  private readonly URL = environment.apiUrl + "/api/Cliente";

  constructor(private http: HttpClient, private authService: AuthService) { }

  // Método para obtener clientes con token en los encabezados
  getClientes(): Observable<Cliente[]> {
    const token = this.authService.getToken(); // Obtener el token
    console.log('Token utilizado para obtener clientes:', token);
    
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Agregar el token al encabezado
    });

    return this.http.get<Cliente[]>(this.URL, { headers }); // Pasar los encabezados
  }

  registrarCliente(cliente: Cliente): Observable<any> {
    console.log(cliente);
    return this.http.post<any>(`${this.URL}/Account/register`, cliente); // Corregido
  }

  eliminarCliente(email: string): Observable<any> { 
    return this.http.delete(`${this.URL}/${email}`);
  }
}
