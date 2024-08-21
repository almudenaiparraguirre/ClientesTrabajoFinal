import { Component, AfterViewInit } from '@angular/core';
import { Chart } from 'chart.js/auto';
import { MonitorDataService, MonitoringData } from 'src/app/servicios/monitor-data.service'; 
import { UserService } from 'src/app/servicios/user.service'; 
import { ClienteService } from 'src/app/servicios/cliente.service';
import { Subscription } from 'rxjs';
import { Usuario } from '../clases/usuario';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements AfterViewInit {


  usuarios: Usuario[] = [];

  transferencias: MonitoringData[] = [];
  private subscription: Subscription = new Subscription();

  constructor(private userService: UserService, private monitorDataService: MonitorDataService) { }

  ngOnInit(): void {
    this.loadUsuarios();
    this.loadTransferencias();
  }

  
  ngAfterViewInit(): void {
    // Financial Chart
    const financialCtx = document.getElementById('financialChart') as HTMLCanvasElement;
    new Chart(financialCtx, {
      type: 'line',
      data: {
        labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct'],
        datasets: [{
          label: 'Earnings',
          data: [1200, 1500, 1000, 2000, 1500, 1700, 1400, 1800, 2200, 2000],
          borderColor: 'rgba(75, 192, 192, 1)',
          backgroundColor: 'rgba(75, 192, 192, 0.2)',
          fill: true
        }]
      }
    });

    

    // Subscriptions Chart
    const subscriptionsCtx = document.getElementById('subscriptionsChart') as HTMLCanvasElement;
    new Chart(subscriptionsCtx, {
      type: 'line',
      data: {
        labels: ['12pm', '3pm', '6pm', '9pm', '12am', '3am', '6am', '9am'],
        datasets: [{
          label: 'Subscriptions',
          data: [169, 180, 172, 181, 190, 178, 169, 162],
          borderColor: 'rgba(255, 159, 64, 1)',
          backgroundColor: 'rgba(255, 159, 64, 0.2)',
          fill: true
        }]
      }
    }); 
  }

  loadTransferencias(): void {
    this.subscription.add(
      this.monitorDataService.getTransferencias().subscribe(
        data => {
          this.transferencias = data;
          this.updateTransferList();
        },
        error => {
          console.error('Error fetching transferencias', error);
        }
      )
    );
  }

  updateTransferList(): void {
    const transferList = document.getElementById('transferList');
    if (transferList) {
      transferList.innerHTML = '';  // Limpia la lista actual
      this.transferencias.forEach((transferencia) => {
        const li = document.createElement('li');
        li.className = 'py-2 flex justify-between';
        li.innerHTML = `
          <span>${transferencia.name}</span>
          <span>${transferencia.paisOrigen} -> ${transferencia.paisDestino}</span>
          <span>${transferencia.clienteOrigen} -> ${transferencia.clienteDestino}</span>
          <span>$${transferencia.value.toFixed(2)}</span>
          <span>${new Date(transferencia.timestamp).toLocaleString()}</span>
        `;
        transferList.appendChild(li);
      });
    }
  }


  loadUsuarios(): void {
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
