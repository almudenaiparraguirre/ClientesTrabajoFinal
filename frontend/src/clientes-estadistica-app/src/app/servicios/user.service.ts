import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Usuario } from '../clases/usuario';
import { environment } from '../../environments/environment';
import { Cliente } from '../clases/cliente';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  // Acordaros de usar variables de entorno para las URLs: environment.apiUrl
  //URL AMIN
  
  private readonly URL = environment.apiUrl + "/api/";

  constructor(private http: HttpClient,private authService: AuthService) { }

  getUsuarios(): Observable<Usuario[]> {
    // Obtener el token
    const token = this.authService.getToken();

    // Configurar los encabezados con el token
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Agregar el token al encabezado
    });

    // Realizar la solicitud GET con los encabezados
    return this.http.get<Usuario[]>(`${this.URL}Account/activeUsers`, { headers });
  }

  isLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }

  autenticarUsuario(email: string, password: string, remember: boolean): Observable<any> {
    remember = false;
    return this.http.post<any>(`${this.URL}Account/login`, { email, password, remember });
  }

// TO DO
  registrarUsuario(usuario: any): Observable<any> {
    console.log(usuario); // Asegúrate de que los campos estén presentes y correctos DEBUG
    return this.http.post<any>(`${this.URL}Account/register`, usuario);
  }
  registrarUsuarioAdmin(usuario: any): Observable<any> {
    console.log(usuario); // Asegúrate de que los campos estén presentes y correctos DEBUG
    return this.http.post<any>(`${this.URL}Account/registerAdmin`, usuario);
  }

  añadirRolUsuario(usuario: any): Observable<any> {
    console.log(usuario); // Asegúrate de que los campos estén presentes y correctos DEBUG
    return this.http.post<any>(`${this.URL}Account/cambiarRolUsuario`, usuario);
  }

  obtenerPaisIdPorNombre(nombre: string): Observable<{ id: number }> {
    // Construir la URL con el nombre del país
    const url = `${this.URL}Paises/nombre/${nombre}`;
    // Hacer la solicitud GET
    return this.http.get<{ id: number }>(url);
  }
  
  editarCliente(cliente: Cliente): Observable<void> {
    // Obtener el token JWT del almacenamiento local (localStorage o donde lo guardes)
    const token = localStorage.getItem('token'); // Asegúrate de que el token esté guardado aquí

    // Construir los encabezados incluyendo el token
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}` // Agregar el token a los encabezados
    });

    // Construir la URL con el parámetro de consulta
    const url = `${this.URL}Cliente/${cliente.email}`;

    return this.http.put<void>(url, cliente, { headers }); // Incluir los encabezados en la solicitud
  }

  editarUsuario(usuario: Usuario): Observable<any> {
    const url = `${this.URL}Account/updateUser?email=${usuario.email}`;
    
    // Si la fecha de nacimiento está definida, convertirla a formato ISO
    if (usuario.dateOfBirth) {
      usuario.dateOfBirth = new Date(usuario.dateOfBirth); // "1995-10-11"
      console.log(usuario)
      console.log("Fecha mandada al back (solo fecha)", usuario.dateOfBirth);
  }
  

    console.log("Fecha de nacimiento usuario", usuario.dateOfBirth); // Verificar el formato

    // Obtener el token del almacenamiento local
    const token = localStorage.getItem('token');
    console.log("Es el token", token);
    
    // Configurar las cabeceras
    const headers = new HttpHeaders({
        'Authorization': `Bearer ${token}`,
        'Content-Type': 'application/json'
    });
    
    return this.http.put(url, usuario, { headers });
}

  eliminarUsuario(email: string): Observable<any> {
    return this.http.delete(`${this.URL}Account/users/${email}`);
  }

  obtenerUsuarioPorEmail(email: string): Observable<any> {
    return this.http.get(`${this.URL}Account/users/getUser?email=${email}`, { responseType: 'text' }).pipe(
      map(response => {
        try {
          return JSON.parse(response);
        } catch {
          return { email: response }; // Fallback if it's not valid JSON
        }
      })
    );
  }

  obtenerDatosCompletosUsuarioPorToken(token: string): Observable<any> {
    return this.http.get(`${this.URL}Account/users/getCompleteUserInfo?token=${token}`);
  }  

  private formatDateToBackend(date: Date): string {
    return date.toISOString();
  }

  // getEnvios(): Observable<any[]> {
  //   return this.http.get<any[]>(`${this.url_estadistica}/getEnvios`).pipe(
  //     map(envios => envios.map(envio => ({
  //       ...envio,
  //       fecha: this.formatearFecha(envio.fecha),
  //       cantidad: this.formatearCantidad(envio.cantidad)
  //     })))
  //   );
  // }

  private formatearFecha(fecha: any): string {
    let fechaDate: Date;

    // Intenta crear un objeto Date a partir del valor proporcionado
    if (fecha instanceof Date) {
      fechaDate = fecha;
    } else if (typeof fecha === 'string') {
      fechaDate = new Date(fecha);
    } else {
      // Si la fecha no es una instancia de Date ni una cadena, devuelves un valor vacío o un error.
      return '';
    }

    // Verifica si la fecha es válida
    if (isNaN(fechaDate.getTime())) {
      return ''; // Retorna una cadena vacía o algún valor por defecto en caso de error
    }

    const day = fechaDate.getDate().toString().padStart(2, '0');
    const month = (fechaDate.getMonth() + 1).toString().padStart(2, '0');
    const year = fechaDate.getFullYear().toString().slice(-2);

    return `${day}/${month}/${year}`;
  }

  private formatearCantidad(cantidad: number): string {
    return cantidad.toFixed(2);
  }
  
}
