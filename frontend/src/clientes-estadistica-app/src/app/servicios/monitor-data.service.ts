import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MonitoringData } from '../interfaces/monitorData.interface';
import { ClienteMonitor } from '../interfaces/clienteMonitor.interface';

@Injectable({
  providedIn: 'root'
})
export class MonitorDataService {

  private apiUrl = 'https://localhost:7107/api/';  // Cambia la URL según tu configuración

  constructor(private http: HttpClient) { }

  getTransferencias(): Observable<MonitoringData[]> {
    return this.http.get<MonitoringData[]>(`${this.apiUrl}Monitoring`);
  }

  getLastTransferencia(): Observable<MonitoringData> {
    return this.http.get<MonitoringData>(`${this.apiUrl}Monitoring/last`);
  }

  getClientes(): Observable<ClienteMonitor[]> {
    return this.http.get<ClienteMonitor[]>(`${this.apiUrl}ClienteMonitoring`);
  }

  getLastCliente(): Observable<ClienteMonitor> {
    return this.http.get<ClienteMonitor>(`${this.apiUrl}ClienteMonitoring/last`);
  }
}

