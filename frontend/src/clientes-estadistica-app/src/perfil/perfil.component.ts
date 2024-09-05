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
  usuario: any;
  userData: any;
  email: string;

  constructor(private router: Router,
    public authService: AuthService,
    private userService: UserService) { }

    ngOnInit(): void {
      if (this.authService.isLoggedIn()) {
        console.log(this.authService.getUserEmail());
        this.userService.obtenerUsuarioPorEmail(this.authService.getUserEmail()).subscribe({
          next: (response) => {
            this.email = response.email;
            this.userService.obtenerDatosCompletosUsuarioPorEmail(this.email).subscribe({
              next: (userData) => {
                this.usuario = userData;
                console.log(this.usuario); // Ahora tienes todos los datos del usuario
              },
              error: (error) => {
                console.log(error);
              }
            });
            this.usuario = response;
            console.log(response);
        },
        error: (error) => {
          console.log(error)
        }});
      } else {
        this.router.navigate(['/login']);
      }
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
  
  // Redirige a la página de inicio
  redirectToHome() {
    this.router.navigate(['/home']); // Asegúrate de que esta ruta esté configurada correctamente en el módulo de enrutamiento
  }

  // Redirige a la página de información de usuarios
  redirectToUsersInfo() {
    this.router.navigate(['/users-info']); // Asegúrate de que esta ruta esté configurada correctamente en el módulo de enrutamiento
  }
}
