import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../app/servicios/auth.service'; // Asegúrate de la ruta correcta
import { Usuario } from '../app/clases/usuario'; // Asegúrate de la ruta correcta
import { UserService } from '../app/servicios/user.service'; // Asegúrate de la ruta correcta

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent implements OnInit {
  totalMoneyTransferred: number = 0;
  totalTransfersCompleted: number = 0;
  usuario: Usuario;
  userService: UserService;

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.loadProfileData();
  }

  canActivate(): boolean {
    if (this.authService.isLoggedIn()) {
      return true;
    } else {
      this.router.navigate(['/login']);
      return false;
    }
  }

  loadProfileData(): void {
    this.totalMoneyTransferred = 1234.56;
    this.totalTransfersCompleted = 42;
  }
}
