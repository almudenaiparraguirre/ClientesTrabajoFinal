import { Component, OnInit, OnDestroy, Renderer2 } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { MonitorDataService } from '../servicios/monitor-data.service';
import { ClienteMonitor } from '../interfaces/clienteMonitor.interface';
import { Chart } from 'chart.js/auto';

@Component({
  selector: 'app-client-access-monitor',
  templateUrl: './client-access-monitor.component.html',
  styleUrls: ['./client-access-monitor.component.css']
})
export class DashboardClientesComponent implements OnInit, OnDestroy {
  clientes: ClienteMonitor[] = [];
  displayedClientes: ClienteMonitor[] = [];
  currentPage: number = 1;
  pageSize: number = 10;
  totalPages: number = 0;

  private pollingSubscription: Subscription;
  private readonly POLLING_INTERVAL = 1000; // Intervalo de sondeo en milisegundos
  private readonly API_URL = 'https://localhost:7107/api/monitoring/subscribe';
  private isReceiving: boolean = true;

  // Gráficos
  private clientesChart: Chart<'line', any, any>;
  private clientesPorPaisChart: Chart<'bar', any, any>;

  totalClientes: number = 0;
  nuevosClientes: number = 0;
  clientesActivos: number = 0;
  clientesInactivos: number = 0;

  constructor(
    private http: HttpClient,
    private monitorDataService: MonitorDataService,
    private renderer: Renderer2
  ) { }

  ngOnInit(): void {
    this.loadInitialData();
    this.startLongPolling();
    this.initClientesChart();
    this.initClientesPorPaisChart();
  }

  ngOnDestroy(): void {
    if (this.pollingSubscription) {
      this.pollingSubscription.unsubscribe();
    }
    if (this.clientesChart) {
      this.clientesChart.destroy();
    }
    if (this.clientesPorPaisChart) {
      this.clientesPorPaisChart.destroy();
    }
  }

  private loadInitialData(): void {
    this.monitorDataService.getClientes().subscribe(
      data => {
        if (data) {
          this.clientes = [...data];
          this.updatePagination();
          this.updateClientesChart();
          this.updateClientesPorPaisChart();
          this.updateCardMetrics();
          this.updateTable();
        }
      },
      error => {
        console.error('Error loading initial data:', error);
      }
    );
  }

  private startLongPolling(): void {
    const poll = () => {
      this.http.get<ClienteMonitor[]>(this.API_URL).subscribe(
        (newClientes) => {
          if (this.isReceiving && newClientes) {
            this.clientes.push(...newClientes);
            this.updatePagination();
            this.updateClientesChart();
            this.updateClientesPorPaisChart();
            this.updateCardMetrics();
            this.updateTable(); // Actualiza la tabla con los nuevos clientes
          }
          if (this.isReceiving) {
            setTimeout(poll, this.POLLING_INTERVAL);
          }
        },
        (error) => {
          console.error('Error en la solicitud:', error);
          setTimeout(poll, this.POLLING_INTERVAL);
        }
      );
    };
    poll();
  }

  pauseReceiving(): void {
    this.isReceiving = false;
  }

  resumeReceiving(): void {
    this.isReceiving = true;
    this.startLongPolling();
  }

  clearClientes(): void {
    this.clientes = [];
    this.displayedClientes = [];
    this.currentPage = 1;
    this.totalPages = 0;
    this.updateClientesChart();
    this.updateClientesPorPaisChart();
    this.updateCardMetrics();
    this.updateTable(); // Limpia la tabla
  }

  private updatePagination(): void {
    this.totalPages = Math.ceil(this.clientes.length / this.pageSize);
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.displayedClientes = this.clientes.slice(startIndex, endIndex);
  }

  onPrevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.updatePagination();
      this.updateTable(); // Actualiza la tabla al cambiar de página
    }
  }

  onNextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.updatePagination();
      this.updateTable(); // Actualiza la tabla al cambiar de página
    }
  }

  // Métodos para gráficos
  private initClientesChart(): void {
    try {
        const ctx = document.getElementById('clientesChart') as HTMLCanvasElement;
        this.clientesChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: [
                    {
                        label: 'Registros',
                        data: [],
                        borderColor: 'rgba(75, 192, 192, 1)',
                        backgroundColor: 'rgba(75, 192, 192, 0.2)',
                        fill: true
                    },
                    {
                        label: 'Logins',
                        data: [],
                        borderColor: 'rgba(255, 99, 132, 1)',
                        backgroundColor: 'rgba(255, 99, 132, 0.2)',
                        fill: true
                    }
                ]
            },
            options: {
                responsive: true,
                scales: {
                    x: {
                        beginAtZero: true
                    },
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    } catch (error) {
        console.error('Error initializing clientesChart:', error);
    }
}


  private initClientesPorPaisChart(): void {
    const ctx = document.getElementById('clientesPorPaisChart') as HTMLCanvasElement;
    this.clientesPorPaisChart = new Chart(ctx, {
      type: 'bar',
      data: {
        labels: [],
        datasets: [{
          label: 'Número de Clientes',
          data: [],
          backgroundColor: 'rgba(54, 162, 235, 0.2)',
          borderColor: 'rgba(54, 162, 235, 1)',
          borderWidth: 1
        }]
      }
    });
  }

  private updateClientesChart(): void {
    const registrosPorPeriodo: { [key: string]: number } = {};
    const loginsPorPeriodo: { [key: string]: number } = {};

    this.clientes.forEach((cliente) => {
        const tipoAcceso = cliente.tipoAcceso.toLowerCase(); // Normaliza el tipoAcceso a minúsculas
        const periodo = this.getPeriod(cliente);

        console.log('Cliente tipoAcceso:', tipoAcceso); // Verificación de tipoAcceso
        console.log('Periodo:', periodo); // Verificación de periodo

        if (tipoAcceso === 'registro') {
            registrosPorPeriodo[periodo] = (registrosPorPeriodo[periodo] || 0) + 1;
        } else if (tipoAcceso === 'login') {
            loginsPorPeriodo[periodo] = (loginsPorPeriodo[periodo] || 0) + 1;
        }
    });

    console.log('Registros por periodo:', registrosPorPeriodo);
    console.log('Logins por periodo:', loginsPorPeriodo);

    const periodos = Object.keys(registrosPorPeriodo).sort();
    const datosRegistros = periodos.map((periodo) => registrosPorPeriodo[periodo] || 0);
    const datosLogins = periodos.map((periodo) => loginsPorPeriodo[periodo] || 0);

    this.clientesChart.data.labels = periodos;
    this.clientesChart.data.datasets[0].data = datosRegistros;
    this.clientesChart.data.datasets[1].data = datosLogins;
    this.clientesChart.update();
}




private getPeriod(cliente: ClienteMonitor): string {
  // Suponiendo que el cliente tiene una fecha de acceso
  // Ahora queremos agrupar por día completo (YYYY-MM-DD)
  const fechaAcceso = new Date(cliente.fechaRecibido); // Ajusta según tu modelo de datos
  const año = fechaAcceso.getFullYear();
  const mes = fechaAcceso.getMonth() + 1; // Meses en JS van de 0-11
  const dia = fechaAcceso.getDate();
  return `${año}-${mes < 10 ? '0' : ''}${mes}-${dia < 10 ? '0' : ''}${dia}`; // Formato: YYYY-MM-DD
}



  private updateClientesPorPaisChart(): void {
    // Objeto para contar clientes por país
    const clientesPorPais: { [key: string]: number } = {};

    // Contar clientes por cada país
    this.clientes.forEach((cliente) => {
        const pais = cliente.pais;
        if (clientesPorPais[pais]) {
            clientesPorPais[pais]++;
        } else {
            clientesPorPais[pais] = 1;
        }
    });

    // Extraer las etiquetas (nombres de países) y los datos (conteo de clientes)
    const paises = Object.keys(clientesPorPais);
    const cuentaClientes = Object.values(clientesPorPais);

    // Actualizar los datos del gráfico
    this.clientesPorPaisChart.data.labels = paises;
    this.clientesPorPaisChart.data.datasets[0].data = cuentaClientes;
    this.clientesPorPaisChart.update();
}

  private updateCardMetrics(): void {
    this.totalClientes = this.clientes.length;
    this.nuevosClientes = this.clientes.filter(cliente => {
      // Lógica para determinar si el cliente es nuevo
    }).length;
    //this.clientesActivos = this.clientes.filter(cliente => cliente.estado === 'activo').length;
    //this.clientesInactivos = this.clientes.filter(cliente => cliente.estado === 'inactivo').length;
  }

  // Método para actualizar la tabla de clientes
  private updateTable(): void {
    const tableBody = document.getElementById('clientesTableBody');
    if (tableBody) {
      tableBody.innerHTML = ''; // Limpiar el contenido actual de la tabla

      this.displayedClientes.forEach((cliente) => {
        const row = document.createElement('tr');
        row.className = 'border-b'; // Asegura que las filas tengan un borde inferior

        row.innerHTML = `
                <td class="px-4 py-2">${cliente.nombre}</td>
                <td class="px-4 py-2">${cliente.apellido}</td>
                <td class="px-4 py-2">${cliente.pais}</td>
                <td class="px-4 py-2">${cliente.email}</td>
                <td class="px-4 py-2">${new Date(cliente.fechaNacimiento).toLocaleDateString()}</td>
                <td class="px-4 py-2">${cliente.tipoAcceso}</td>
            `;

        tableBody.appendChild(row); // Insertar la fila en el cuerpo de la tabla
      });
    }
  }

}
