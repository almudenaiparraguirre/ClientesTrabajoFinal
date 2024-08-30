import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { MonitoringData } from '../interfaces/monitorData.interface';

@Injectable({
  providedIn: 'root'
})
export class MonitorDataService {

  private apiUrl = 'https://localhost:44339/api/Monitoring';  // Cambia la URL según tu configuración

  constructor(private http: HttpClient) { }

  getTransferencias(): Observable<MonitoringData[]> {
    return this.http.get<MonitoringData[]>(this.apiUrl);
  }

  getLastTransferencia(): Observable<MonitoringData> {
    return this.http.get<MonitoringData>(`${this.apiUrl}/last`);
  }
}
export { MonitoringData };
