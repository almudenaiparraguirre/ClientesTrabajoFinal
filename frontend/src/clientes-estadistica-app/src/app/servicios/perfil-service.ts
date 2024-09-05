import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MonitoringData } from '../interfaces/monitorData.interface';
import { ClienteMonitor } from '../interfaces/clienteMonitor.interface';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class PerfilService {

  private readonly URL = environment.apiUrl + "/api/";

  constructor(private http: HttpClient) { }

  actualizarCliente(cliente: any): Observable<any> {
    return this.http.put<any>(`${this.URL}cliente/${cliente.email}`, cliente);
  }

  actualizarUsuario(usuario: any): Observable<any> {
    return this.http.put<any>(`${this.URL}Account/${usuario.email}`, usuario);
  }

}

