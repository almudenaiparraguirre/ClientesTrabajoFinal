import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MonitoringData } from '../interfaces/monitorData.interface';
import { ClienteMonitor } from '../interfaces/clienteMonitor.interface';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MonitorDataService {

  private readonly URL = environment.apiUrl + "/api/";

  constructor(private http: HttpClient) { }

  getTransferencias(): Observable<MonitoringData[]> {
    return this.http.get<MonitoringData[]>(`${this.URL}Monitoring`);
  }

  getLastTransferencia(): Observable<MonitoringData> {
    return this.http.get<MonitoringData>(`${this.URL}Monitoring/last`);
  }

  getClientes(): Observable<ClienteMonitor[]> {
    return this.http.get<ClienteMonitor[]>(`${this.URL}ClienteMonitoring`);
  }

  getLastCliente(): Observable<ClienteMonitor> {
    return this.http.get<ClienteMonitor>(`${this.URL}ClienteMonitoring/last`);
  }
}

