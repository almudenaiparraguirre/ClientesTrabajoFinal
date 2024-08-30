import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Cliente } from '../clases/cliente'; // Asegúrate de tener una clase Cliente
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ClienteService {

  private readonly URL = "https://localhost:44339/api/Cliente";

  constructor(private http: HttpClient) { }

  getClientes(): Observable<Cliente[]> {
    return this.http.get<Cliente[]>(this.URL);
  }

  registrarCliente(cliente: Cliente): Observable<any>{
    console.log(cliente);
    return this.http.post<any>(`${this.URL}Account/register`, cliente);
  }

  eliminarCliente(email: string): Observable<any> {
    return this.http.delete(`${this.URL}/${email}`);
  }
}
