import { Component, OnInit } from '@angular/core';
import { UserService } from '../servicios/user.service';
import { Usuario } from '../clases/usuario';
import { Router } from '@angular/router';
import { catchError, throwError } from 'rxjs';
import * as jwt_decode from 'jwt-decode';
import { AuthService } from '../servicios/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {
  errorMessage: string;
  isErrorVisible = false;
  usuarios: Usuario[];

  email: string = '';
  contrase: string = '';

  constructor(private usuarioService: UserService, private route: Router,
    private authService: AuthService) { }

  ngOnInit() {
    this.getUsuarios();
  }

  getUsuarios() {
    this.usuarioService.getUsuarios().subscribe(datos => {
      this.usuarios = datos;
      console.log(datos);
    });
  }

  authentication() {
    this.usuarioService.autenticarUsuario(this.email, this.contrase, true)
      .pipe(
        catchError(error => {
          if (error.status === 401) {
            this.showError('Credenciales incorrectas');
          } else {
            this.showError('Ocurrió un error al intentar autenticarse. Por favor, intente de nuevo.');
          }
          return throwError(error);
        })
      )
      .subscribe(response => {
        console.log('Respuesta del servidor:', response); // Imprimir toda la respuesta

        if (response && response.token) {
          localStorage.setItem('token', response.token.result);
          const token = response.token.result;
          
          // Decodifica el token para obtener el rol
          const decodedToken: any = jwt_decode.jwtDecode(token);
          console.log("token decodificado", decodedToken);
          //const decodedToken: any = jwt_decode(token);
          const userRole = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
          console.log(userRole)

          // Redirige según el rol del usuario
          if (userRole === 'Client') {
            this.route.navigate(['/perfil']);
          } else {
            this.route.navigate(['/users-info']);
          }
        }
      });
  }

  redirectToRegisterPage() {
    this.route.navigate(['/registro']);
  }

  showError(message: string) {
    this.errorMessage = message;
    this.isErrorVisible = true;
    setTimeout(() => this.isErrorVisible = false, 3000); // Ocultar el mensaje después de 5 segundos
  }
}
