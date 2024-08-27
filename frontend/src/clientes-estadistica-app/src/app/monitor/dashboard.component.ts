import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { Chart } from 'chart.js/auto'; // Asegúrate de que 'chart.js/auto' esté instalado

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnDestroy {
  transferencias: any[] = [];
  private pollingSubscription: Subscription;
  private readonly POLLING_INTERVAL = 5000; // Intervalo de sondeo en milisegundos (5 segundos)
  private readonly API_URL = 'https://localhost:7107/api/monitoring/subscribe';

  // Gráficos
  private financialChart: Chart<'line', any, any>;
  private countriesComparisonChart: Chart<'bar', any, any>;
  private labels: string[] = [];
  private data: number[] = [];
  private receivingCountryLabels: string[] = [];
  private receivingCountryData: number[] = [];
  private sendingCountryLabels: string[] = [];
  private sendingCountryData: number[] = [];
  totalMoneyTransferred: number = 0;
  totalTransfersCompleted: number = 0;

  constructor(private http: HttpClient) { }

  ngOnInit(): void {
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

  private startLongPolling(): void {
    const poll = () => {
      this.http.get<any[]>(this.API_URL).subscribe(
        data => {
          if (data) {
            this.transferencias = data;
            this.updateTransferTable();
            this.updateFinancialChart();
            this.updateCountriesComparisonChart();
            this.calculateTotals();
            // Continue polling after receiving data
            poll();
          }
        },
        error => {
          console.error('Error fetching transferencias', error);
          // Retry after a delay in case of error
          setTimeout(poll, this.POLLING_INTERVAL);
        }
      );
    };

    // Start polling
    poll();
  }

  private updateTransferTable(): void {
    const tableBody = document.getElementById('transferTableBody');
    if (tableBody) {
      tableBody.innerHTML = ''; // Limpia la tabla actual
      this.transferencias.forEach((transferencia) => {
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
        tableBody.appendChild(row);
      });
    }
  }

  private initFinancialChart() {
    const financialCtx = document.getElementById('financialChart') as HTMLCanvasElement;

    this.financialChart = new Chart(financialCtx, {
      type: 'line',
      data: {
        labels: this.labels,
        datasets: [{
          label: 'Valor Transferencias',
          data: this.data,
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          fill: true
        }]
      },
      options: {
        scales: {
          x: {
            title: {
              display: true,
              text: 'Fecha'
            }
          },
          y: {
            title: {
              display: true,
              text: 'Valor ($)'
            }
          }
        }
      }
    });
  }

  private initCountriesComparisonChart() {
    const countriesCtx = document.getElementById('countriesComparisonChart') as HTMLCanvasElement;

    this.countriesComparisonChart = new Chart(countriesCtx, {
      type: 'bar',
      data: {
        labels: this.receivingCountryLabels, // Usamos las mismas etiquetas para ambos conjuntos de datos
        datasets: [
          {
            label: 'Transferencias Recibidas',
            data: this.receivingCountryData,
            backgroundColor: 'rgba(255, 159, 64, 0.2)',
            borderColor: 'rgba(255, 159, 64, 1)',
            borderWidth: 1,
            barPercentage: 0.4, // Ajusta el ancho de las barras
            categoryPercentage: 0.8
          },
          {
            label: 'Transferencias Enviadas',
            data: this.sendingCountryData,
            backgroundColor: 'rgba(75, 192, 192, 0.2)',
            borderColor: 'rgba(75, 192, 192, 1)',
            borderWidth: 1,
            barPercentage: 0.4, // Ajusta el ancho de las barras
            categoryPercentage: 0.8
          }
        ]
      },
      options: {
        scales: {
          x: {
            stacked: false,
            title: {
              display: true,
              text: 'País'
            }
          },
          y: {
            stacked: false,
            title: {
              display: true,
              text: 'Número de Transferencias'
            }
          }
        }
      }
    });
  }

  private updateFinancialChart() {
    this.labels = this.transferencias.map(t => new Date(t.timestamp).toLocaleTimeString());
    this.data = this.transferencias.map(t => t.valorDestino);

    this.financialChart.data.labels = this.labels;
    this.financialChart.data.datasets[0].data = this.data;
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
    this.receivingCountryLabels = Array.from(allCountries);
    this.receivingCountryData = this.receivingCountryLabels.map(country => receivingCountryMap[country] || 0);
    this.sendingCountryData = this.receivingCountryLabels.map(country => sendingCountryMap[country] || 0);

    this.countriesComparisonChart.data.labels = this.receivingCountryLabels;
    this.countriesComparisonChart.data.datasets[0].data = this.receivingCountryData;
    this.countriesComparisonChart.data.datasets[1].data = this.sendingCountryData;
    this.countriesComparisonChart.update();
  }

  private calculateTotals(): void {
    this.totalMoneyTransferred = this.transferencias.reduce((acc, t) => acc + t.valorDestino, 0);
    this.totalTransfersCompleted = this.transferencias.length;
  }
}
