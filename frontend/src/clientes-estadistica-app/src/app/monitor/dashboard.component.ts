import { Component, AfterViewInit, OnDestroy } from '@angular/core';
import { Chart } from 'chart.js/auto';
import { MonitorDataService, MonitoringData } from 'src/app/servicios/monitor-data.service';
import { UserService } from 'src/app/servicios/user.service';
import { Subscription } from 'rxjs';
import { Usuario } from '../clases/usuario';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements AfterViewInit, OnDestroy {
  usuarios: Usuario[] = [];
  transferencias: MonitoringData[] = [];
  private subscription: Subscription = new Subscription();
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

  constructor(private userService: UserService, private monitorDataService: MonitorDataService) { }

  ngOnInit(): void {
    this.loadUsuarios();
    this.loadTransferencias(); // Carga inicial de datos
  }

  ngAfterViewInit(): void {
    this.initFinancialChart(); // Inicializa el gráfico después de que la vista esté cargada
    this.initCountriesComparisonChart();
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
    if (this.financialChart) {
      this.financialChart.destroy();
    }
    if (this.countriesComparisonChart) {
      this.countriesComparisonChart.destroy();
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

  private loadTransferencias(): void {
    this.subscription.add(
      this.monitorDataService.getTransferencias().subscribe(
        data => {
          this.transferencias = data;
          this.updateTransferTable();
          this.updateFinancialChart();
          this.updateCountriesComparisonChart();
          this.calculateTotals();
        },
        error => {
          console.error('Error fetching transferencias', error);
        }
      )
    );
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
          <td class="px-4 py-2">$${transferencia.value.toFixed(2)}</td>
          <td class="px-4 py-2">${new Date(transferencia.timestamp).toLocaleString()}</td>
        `;
        tableBody.appendChild(row);
      });
    }
  }

  private updateFinancialChart() {
    this.labels = this.transferencias.map(t => new Date(t.timestamp).toLocaleTimeString());
    this.data = this.transferencias.map(t => t.value);

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
    this.totalMoneyTransferred = this.transferencias.reduce((acc, t) => acc + t.value, 0);
    this.totalTransfersCompleted = this.transferencias.length;
  }

  private loadUsuarios(): void {
    this.subscription.add(
      this.userService.getUsuarios().subscribe(
        data => {
          this.usuarios = data;
          document.getElementById('UsuariosLogueados').innerHTML = data.length.toString();
        },
        error => {
          console.error('Error fetching users', error);
        }
      )
    );
  }
}
