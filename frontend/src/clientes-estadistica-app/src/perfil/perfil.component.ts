import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../app/servicios/auth.service'; // Asegúrate de la ruta correcta
import { UserService } from '../app/servicios/user.service'; // Asegúrate de la ruta correcta
import { PerfilService } from 'src/app/servicios/perfil-service';

@Component({
  selector: 'app-perfil',
  templateUrl: './perfil.component.html',
  styleUrls: ['./perfil.component.css']
})
export class PerfilComponent implements OnInit {
  usuario: any;
  isEditing: boolean = false;  // Controlador para edición
  originalData: any;  // Almacenar los datos originales antes de editar

  constructor(private router: Router,
              public authService: AuthService,
              private userService: UserService,
              private perfilService: PerfilService) { }

  ngOnInit(): void {
    if (this.authService.isLoggedIn()) {
      const userEmail = this.authService.getUserEmail();
      let token = this.authService.getToken()
      this.userService.obtenerDatosCompletosUsuarioPorToken(token).subscribe({
        next: (userData) => {
          this.usuario = userData;
          this.originalData = { ...userData };  // Guardar los datos originales
          if (this.usuario.dateOfBirth instanceof Date) {
            this.usuario.dateOfBirthString = this.formatDate(this.usuario.dateOfBirth);
          } else {
            // Si es una cadena, puedes convertirla a un objeto Date primero
            this.usuario.dateOfBirthString = this.formatDate(new Date(this.usuario.dateOfBirth));
          }
        },
        error: (error) => {
          console.log(error);
        }
        
      });
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

  redirectToHome() {
    this.router.navigate(['/home']);
  }

  redirectToUsersInfo() {
    this.router.navigate(['/users-info']);
  }

  // Habilitar edición
  enableEditing() {
    this.isEditing = true;
  }

    formatDate(date: Date): string {
      // Formatea la fecha en YYYY-MM-DD
      const year = date.getFullYear();
      const month = String(date.getMonth() + 1).padStart(2, '0'); // Meses de 0-11
      const day = String(date.getDate()).padStart(2, '0'); // Día del mes
      return `${year}-${month}-${day}`;
    }
  // Guardar cambios
  saveChangesClient() {
    // Implementa la lógica para guardar los cambios, por ejemplo, llamando a un servicio
    this.perfilService.actualizarCliente(this.usuario).subscribe({});

    // Deshabilitar edición
    this.isEditing = false;
  }

  saveChanges() {
    // Implementa la lógica para guardar los cambios, por ejemplo, llamando a un servicio
    this.perfilService.actualizarUsuario(this.usuario).subscribe({});

    // Deshabilitar edición
    this.isEditing = false;
  }

  // Cancelar edición
  cancelEditing() {
    this.usuario = { ...this.originalData };  // Restaurar datos originales
    this.isEditing = false;
  }
}
