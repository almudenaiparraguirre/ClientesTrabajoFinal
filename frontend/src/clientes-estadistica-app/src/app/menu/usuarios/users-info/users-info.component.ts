import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/servicios/user.service'; 
import { ClienteService } from 'src/app/servicios/cliente.service';
import { Usuario } from 'src/app/clases/usuario'; 
import { Cliente } from 'src/app/clases/cliente'; 
import { HttpHeaders } from '@angular/common/http';
import { AuthService } from 'src/app/servicios/auth.service';
import { Router } from '@angular/router';
import { PerfilService } from 'src/app/servicios/perfil-service';

@Component({
  selector: 'app-users-info',
  templateUrl: './users-info.component.html',
})
export class UsersInfoComponent implements OnInit, OnDestroy {
  isAddModalOpen = false;
  selectedCliente: Cliente | null = null;
  selectedUsuario: Usuario | null = null;
  isModalOpen = false;
  editMode: 'user' | 'client' = 'user';
  addMode: 'user' | 'client' = 'user';
  randomUserImageUrl: string = '';

  usuarios: Usuario[] = [];
  filteredUsuarios: Usuario[] = [];
  clientes: Cliente[] = [];
  filteredClientes: Cliente[] = [];
  
  // Variables para la paginación
  page: number = 1;
  pageSize: number = 5;
  totalUsuarios: number = 0;
  totalClientes: number = 0;

  // Variable para el filtrado
  searchTerm: string = '';

  // Variables para la tabulación
  activeTab: string = 'table1';

  // Variable para almacenar la suscripción
  private subscription: Subscription = new Subscription();
  perfilService: PerfilService;

  constructor(
    private userService: UserService, 
    private clienteService: ClienteService,
    private authService: AuthService, 
    private router: Router,
    perfilService: PerfilService,  // Inyección de PerfilService
    

  ) {    this.perfilService = perfilService;  // Asignación de perfilService
    this.generateRandomImageUrl();
  }

  ngOnInit(): void {
    this.loadUsuarios();
    this.loadClientes();
  }

  openAddModal(mode: 'user' | 'client') {
    this.isAddModalOpen = true;
    this.editMode = mode; // Cambia a `editMode` en lugar de `addMode`
    if (mode === 'user') {
      this.selectedUsuario = new Usuario();
      this.selectedCliente = null;
    } else {
      this.selectedCliente = new Cliente();
      this.selectedUsuario = null;
    }
  }
  

  closeAddModal() {
    this.isAddModalOpen = false;
  }

  loadUsuarios(): void {
    this.subscription.add(
      this.userService.getUsuarios().subscribe(
        data => {
          this.usuarios = data.map(usuario => ({
            ...usuario,
            userName: usuario.userName || '-',
            fullName: usuario.fullName || '-',
            email: usuario.email || '-',
            dateOfBirth: usuario.dateOfBirth ? new Date(usuario.dateOfBirth) : null
          }));
          this.filteredUsuarios = this.usuarios;
          this.totalUsuarios = this.usuarios.length;
          console.log('Usuarios cargados', this.usuarios);
        },
        error => {
          console.error('Error fetching users', error);
        }
      )
    );
  }
  
  loadClientes(): void {
    const token = this.authService.getToken();
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  
    this.clienteService.getClientes().subscribe(
      (clientes) => {
        console.log('Clientes cargados', clientes);
        this.clientes = clientes;
        this.filteredClientes = this.clientes;
        this.totalClientes = this.clientes.length;
      },
      (error) => {
        console.error('Error fetching clients', error);
        if (error.status === 403) {
          alert('No tienes permiso para acceder a esta ruta.');
        } else if (error.status === 401) {
          alert('No estás autorizado. Por favor, inicia sesión.');
        } else {
          alert('Ocurrió un error inesperado.');
        }
      }
    );
  }

  filterResults() {
    this.page = 1;
    if (this.activeTab === 'table1') {
      this.filteredUsuarios = this.usuarios.filter(usuario => 
        usuario.userName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        usuario.fullName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        usuario.email.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    } else if (this.activeTab === 'table2') {
      this.filteredClientes = this.clientes.filter(cliente =>
        cliente.nombre.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        cliente.apellido.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
        cliente.email.toLowerCase().includes(this.searchTerm.toLowerCase())
      );
    }
  }

  redirectToProfile() {
    this.router.navigate(['/perfil']);
  }
  
  editUsuario(usuario: Usuario): void {
    this.selectedUsuario = { ...usuario };
    this.editMode = 'user';
    this.isModalOpen = true;
  }

  editCliente(cliente: Cliente): void {
    this.selectedCliente = { ...cliente };
    this.editMode = 'client';
    this.isModalOpen = true;
  }

  deleteUsuario(usuario: Usuario) {
    const confirmacion = confirm(`¿Estás seguro de que deseas eliminar al usuario ${usuario.nombre} ${usuario.apellido}?`);
    if (confirmacion) {
      this.userService.eliminarUsuario(usuario.email).subscribe(
        () => {
          console.log('Usuario eliminado correctamente');
          this.loadUsuarios();
        },
        error => {
          console.error('Error al eliminar el usuario', error);
        }
      );
    }
  }

  deleteCliente(cliente: Cliente) {
    const confirmacion = confirm(`¿Estás seguro de que deseas eliminar al cliente ${cliente.nombre} ${cliente.apellido}?`);
    if (confirmacion) {
      this.clienteService.eliminarCliente(cliente.email).subscribe(
        () => {
          console.log('Cliente eliminado correctamente');
          this.loadClientes();
        },
        error => {
          console.error('Error al eliminar el cliente', error);
        }
      );
    }
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedUsuario = null;
    this.selectedCliente = null;
  }

  onSave(): void {
    if (this.editMode === 'user' && this.selectedUsuario && this.selectedUsuario.email) {
      this.userService.editarUsuario(this.selectedUsuario).subscribe(
        () => {
          console.log('Usuario actualizado correctamente');
          this.loadUsuarios();
        },
        error => {
          console.error('Error al actualizar el usuario', error);
        }
      );
    } else if (this.editMode === 'client' && this.selectedCliente && this.selectedCliente.email) {
      this.perfilService.actualizarCliente(this.selectedCliente).subscribe(
        () => {
          console.log('Cliente actualizado correctamente');
          this.loadClientes();
        },
        error => {
          console.error('Error al actualizar el cliente', error);
        }
      );
    }
    this.closeModal();
  }

  onAdd(): void {
    if (this.editMode === 'user' && this.selectedUsuario) {
      if (this.selectedUsuario.dateOfBirth) {
        this.selectedUsuario.dateOfBirth = new Date(this.selectedUsuario.dateOfBirth);
      } else {
        this.selectedUsuario.dateOfBirth = null;
      }
  
      // Aquí utilizas registrarUsuarioAdmin en lugar de registrarUsuario
      this.userService.registrarUsuarioAdmin(this.selectedUsuario).subscribe(
        () => {
          console.log('Usuario añadido correctamente como Admin');
          this.loadUsuarios();
          this.closeAddModal();
        },
        error => {
          console.error('Error al añadir el usuario como Admin', error);
        }
      );
    } else if (this.editMode === 'client' && this.selectedCliente) {
      this.clienteService.registrarCliente(this.selectedCliente).subscribe(
        () => {
          console.log('Cliente añadido correctamente');
          this.loadClientes();
          this.closeAddModal();
        },
        error => {
          console.error('Error al añadir el cliente', error);
        }
      );
    } else {
      this.closeAddModal();
    }
  }
  
  toDateString(timestamp: number): string {
    return new Date(timestamp).toISOString().split('T')[0];
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  generateRandomImageUrl(): void {
    const randomNumber = Math.floor(Math.random() * 100) + 1;
    this.randomUserImageUrl = `https://randomuser.me/api/portraits/men/${randomNumber}.jpg`;
  }
}
