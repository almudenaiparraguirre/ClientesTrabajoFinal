import { Component, OnInit } from '@angular/core';
import { Usuario } from '../app/clases/usuario';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent implements OnInit {
  totalMoneyTransferred: number = 0; // Define la propiedad totalMoneyTransferred
  totalTransfersCompleted: number = 0; // Define la propiedad totalTransfersCompleted
  usuario: Usuario;

  constructor() { }

  ngOnInit(): void {
    // Aquí puedes inicializar o cargar datos para estas propiedades
    this.loadProfileData();
  }

  loadProfileData(): void {
    // Simula la carga de datos
    this.totalMoneyTransferred = 1234.56;
    this.totalTransfersCompleted = 42;
  }
}
