import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { MonitorDataService } from '../servicios/monitor-data.service';
import { MonitoringData } from '../interfaces/monitorData.interface';
import { Chart } from 'chart.js/auto';
import { environment } from 'src/environments/environment';
import { Router } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  transferencias: MonitoringData[] = [];
  displayedTransferencias: MonitoringData[] = [];
  currentPage: number = 1;
  pageSize: number = 10;
  totalPages: number = 0;

  private pollingSubscription: Subscription;
  private readonly POLLING_INTERVAL = 1000; // Intervalo de sondeo en milisegundos
  private readonly API_URL = environment.apiUrl + '/api/monitoring/subscribe';
  private isReceiving: boolean = true; // Controla si la recepción está activa

  // Gráficos
  private financialChart: Chart<'line', any, any>;
  private countriesComparisonChart: Chart<'bar', any, any>;

  totalMoneyTransferred: number = 0;
  totalTransfersCompleted: number = 0;
  averageMoneyTransferred: number = 0;
  transferenciasHoy: number = 0;

  constructor(private http: HttpClient, private monitorDataService: MonitorDataService, private router: Router) { }

  ngOnInit(): void {
    this.loadInitialData();
    this.startLongPolling();
    this.initFinancialChart();
    this.initCountriesComparisonChart();
  }

  ngOnDestroy(): void {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
    if (this.financialChart) {
      this.financialChart.destroy();
    }
    if (this.countriesComparisonChart) {
      this.countriesComparisonChart.destroy();
    }
  }

  // Método para redirigir al dashboard
  redirectToDashboard() {
    this.router.navigate(['/dashboard']); // Asegúrate de que la ruta esté configurada correctamente en tu routing module
  }

  // Método para redirigir al dashboard de clientes
  redirectToDashboardClientes() {
    this.router.navigate(['/dashboardclientes']); // Asegúrate de que esta ruta también esté configurada en tu routing module
  }
  private loadInitialData(): void {
    this.monitorDataService.getTransferencias().subscribe(
      data => {
        if (data) {
          this.transferencias = [...data];
          this.updatePagination();
          this.updateFinancialChart();
          this.updateCountriesComparisonChart();
          this.calculateTotals();
          this.calculateAverageMoneyTransferred();
          this.updateTransferTable(); // Asegúrate de que la tabla esté actualizada
        }
      },
      error => {
        console.error('Error loading initial data', error);
      }
    );
  }

  private startLongPolling(): void {
    const poll = () => {
        if (this.isReceiving) {
            this.pollingSubscription = this.http.get<MonitoringData>(this.API_URL).subscribe(
                data => {
                    if (data && this.isReceiving) { // Si hay un objeto recibido
                        // Añadir el nuevo registro a las transferencias
                        this.transferencias.push(data);
                        // Actualizar la paginación, tabla y gráficos
                        this.updatePagination();
                        this.updateTransferTable(); // Actualizar la tabla con el nuevo registro
                        this.updateFinancialChart();
                        this.updateCountriesComparisonChart();
                        this.calculateTotals();
                        this.calculateAverageMoneyTransferred();
                    }
                    setTimeout(poll, this.POLLING_INTERVAL);
                },
                error => {
                    console.error('Error fetching updates', error);
                    setTimeout(poll, this.POLLING_INTERVAL);
                }
            );
        }
    };

    poll();
}

  private updatePagination(): void {
    this.totalPages = Math.ceil(this.transferencias.length / this.pageSize);
    const start = (this.currentPage - 1) * this.pageSize;
    const end = start + this.pageSize;
    
    // Asegúrate de que el orden de `transferencias` se mantenga para la paginación
    this.displayedTransferencias = this.transferencias.slice().reverse().slice(start, end);
}


  private updateFinancialChart() {
    const labels = this.transferencias.map(t => new Date(t.timestamp).toLocaleTimeString());
    const data = this.transferencias.map(t => t.valorDestino);

    this.financialChart.data.labels = labels;
    this.financialChart.data.datasets[0].data = data;
    this.financialChart.update();
  }

  private updateCountriesComparisonChart() {
    const receivingCountryMap = this.transferencias.reduce((acc, t) => {
      acc[t.paisDestino] = (acc[t.paisDestino] || 0) + 1;
      return acc;
    }, {} as { [key: string]: number });

    const sendingCountryMap = this.transferencias.reduce((acc, t) => {
      acc[t.paisOrigen] = (acc[t.paisOrigen] || 0) + 1;
      return acc;
    }, {} as { [key: string]: number });

    const allCountries = new Set([...Object.keys(receivingCountryMap), ...Object.keys(sendingCountryMap)]);
    const labels = Array.from(allCountries);
    const receivingCountryData = labels.map(country => receivingCountryMap[country] || 0);
    const sendingCountryData = labels.map(country => sendingCountryMap[country] || 0);

    this.countriesComparisonChart.data.labels = labels;
    this.countriesComparisonChart.data.datasets[0].data = receivingCountryData;
    this.countriesComparisonChart.data.datasets[1].data = sendingCountryData;
    this.countriesComparisonChart.update();
  }

  private calculateTotals() {
    this.totalMoneyTransferred = this.transferencias.reduce((sum, t) => sum + t.valorDestino, 0);
    this.totalTransfersCompleted = this.transferencias.length;
    this.transferenciasHoy = this.getTransferenciasHoy();
  }

  private calculateAverageMoneyTransferred() {
    if (this.totalTransfersCompleted > 0) {
      this.averageMoneyTransferred = this.totalMoneyTransferred / this.totalTransfersCompleted;
    } else {
      this.averageMoneyTransferred = 0;
    }
  }

  private updateTransferTable(): void {
    const tableBody = document.getElementById('transferTableBody');
    if (tableBody) {
        tableBody.innerHTML = ''; // Limpiar el contenido actual de la tabla
        this.displayedTransferencias.forEach((transferencia) => {
            const row = document.createElement('tr');
            row.className = 'border-b';
            row.innerHTML = `
                <td class="px-4 py-2">${transferencia.name}</td>
                <td class="px-4 py-2">${transferencia.paisOrigen}</td>
                <td class="px-4 py-2">${transferencia.paisDestino}</td>
                <td class="px-4 py-2">${transferencia.clienteOrigen}</td>
                <td class="px-4 py-2">${transferencia.clienteDestino}</td>
                <td class="px-4 py-2">$${transferencia.valorOrigen.toFixed(2)}</td>
                <td class="px-4 py-2">$${transferencia.valorDestino.toFixed(2)}</td>
                <td class="px-4 py-2">${new Date(transferencia.timestamp).toLocaleString()}</td>
            `;
            tableBody.appendChild(row); // Insertar como el primer hijo
        });
    }
}


  pauseReceiving(): void {
    this.isReceiving = false;
  }

  resumeReceiving(): void {
    this.isReceiving = true;
    this.startLongPolling(); // Reiniciar el sondeo al reanudar la recepción
  }

  clearTransfers(): void {
    this.transferencias = [];
    this.displayedTransferencias = [];
    this.clearFinancialChart();
    this.clearCountriesComparisonChart();
    this.updateTransferTable(); // Limpiar la tabla
    this.calculateTotals();
  }

  private clearFinancialChart(): void {
    if (this.financialChart) {
      this.financialChart.data.labels = [];
      this.financialChart.data.datasets[0].data = [];
      this.financialChart.update();
    }
  }

  private clearCountriesComparisonChart(): void {
    if (this.countriesComparisonChart) {
      this.countriesComparisonChart.data.labels = [];
      this.countriesComparisonChart.data.datasets[0].data = [];
      this.countriesComparisonChart.data.datasets[1].data = [];
      this.countriesComparisonChart.update();
    }
  }

  private getTransferenciasHoy(): number {
    const todayStart = new Date();
    todayStart.setHours(0, 0, 0, 0); // Inicio del día
  
    const todayEnd = new Date();
    todayEnd.setHours(23, 59, 59, 999); // Fin del día
  
    return this.transferencias.filter(t =>
      new Date(t.timestamp) >= todayStart && new Date(t.timestamp) <= todayEnd
    ).length;
  }

  onPrevPage(): void {
    if (this.currentPage > 1) {
        this.currentPage--;
        this.updatePagination();
        this.updateTransferTable(); // Refrescar la tabla para la nueva página
    }
}

onNextPage(): void {
  if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagination();
      this.updateTransferTable(); // Refrescar la tabla para la nueva página
  }
}

  private initFinancialChart() {
    this.financialChart = new Chart('financialChart', {
      type: 'line',
      data: {
        labels: [],
        datasets: [{
          label: 'Valor Transferido',
          data: [],
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          fill: true
        }]
      }
    });
  }

  private initCountriesComparisonChart() {
    this.countriesComparisonChart = new Chart('countriesComparisonChart', {
      type: 'bar',
      data: {
        labels: [],
        datasets: [{
          label: 'Países Destino',
          data: [],
          backgroundColor: 'rgba(153, 102, 255, 0.2)',
          borderColor: 'rgba(153, 102, 255, 1)',
          borderWidth: 1
        }, {
          label: 'Países Origen',
          data: [],
          backgroundColor: 'rgba(255, 99, 132, 0.2)',
          borderColor: 'rgba(255, 99, 132, 1)',
          borderWidth: 1
        }]
      }
    });
  }
}
