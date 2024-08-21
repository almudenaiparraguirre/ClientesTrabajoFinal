import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map, Observable } from 'rxjs';
import { Usuario } from '../clases/usuario';
import { environment } from '../../environments/environment';
import { Cliente } from '../clases/cliente';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  // IMPORTANTE:
  //    1 URL ESTADISTICA Y OTRA PARA CLIENTES
  private readonly url_estadistica = environment.apiUrl;

  //URL AMIN
  private readonly URL = "https://localhost:44339/api/";

  constructor(private http: HttpClient) { }

  getUsuarios(): Observable<Usuario[]> {
    return this.http.get<Usuario[]>(`${this.URL}Account/users`);
  }

  autenticarUsuario(email: string, password: string, remember: boolean): Observable<any> {
    remember = false;
    return this.http.post<any>(`${this.URL}Account/login`, { email, password, remember });
  }

  registrarUsuario(usuario: any): Observable<any> {
    console.log(usuario); // Asegúrate de que los campos estén presentes y correctos DEBUG
    return this.http.post<any>(`${this.URL}Account/register`, usuario);
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

  crearCliente(cliente: Cliente): Observable<void> {
    const url = `${this.URL}Account/register`;
    return this.http.post<void>(url, cliente);
  }

  crearUsuario(usuario: Usuario): Observable<void> {
    const url = `${this.URL}Account/register`;
    return this.http.post<void>(url, usuario);
  }

  editarCliente(cliente: Cliente): Observable<void> {
    // Construir la URL con el parámetro de consulta
    const url = `${this.URL}updateUser?email=${encodeURIComponent(cliente.email)}`;
    // Convertir fecha de nacimiento a timestamp (milisegundos)
    cliente.fechaNacimiento = new Date(cliente.fechaNacimiento).getTime();
    // Enviar la solicitud PUT
    return this.http.put<void>(url, cliente);
  }

  editarUsuario(usuario: Usuario): Observable<any>{
    const url = `${this.URL}Account/update/${usuario.email}`;
    usuario.dateOfBirth = new Date(usuario.dateOfBirth).getTime();
    return this.http.put(url, usuario);
  }

  private formatDateToBackend(date: Date): string {
    return date.toISOString();
  }

  getEnvios(): Observable<any[]> {
    return this.http.get<any[]>(`${this.url_estadistica}/getEnvios`).pipe(
      map(envios => envios.map(envio => ({
        ...envio,
        fecha: this.formatearFecha(envio.fecha),
        cantidad: this.formatearCantidad(envio.cantidad)
      })))
    );
  }

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
