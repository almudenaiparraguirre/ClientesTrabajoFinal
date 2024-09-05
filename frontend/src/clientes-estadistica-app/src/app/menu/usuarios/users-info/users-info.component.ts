import { Component, OnInit, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';
import { UserService } from 'src/app/servicios/user.service'; 
import { ClienteService } from 'src/app/servicios/cliente.service';
import { Usuario } from 'src/app/clases/usuario'; 
import { Cliente } from 'src/app/clases/cliente'; 
import { HttpHeaders } from '@angular/common/http';
import { AuthService } from 'src/app/servicios/auth.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-users-info',
  templateUrl: './users-info.component.html',
})
export class UsersInfoComponent implements OnInit, OnDestroy {
  isAddModalOpen = false;
  // Variables para almacenar el usuario o cliente seleccionado y el estado del modal
  selectedCliente: Cliente | null = null;
  selectedUsuario: Usuario | null = null;
  isModalOpen = false;
  editMode: 'user' | 'client' = 'user';
  addMode: 'user' | 'client' = 'user';
  selectedItem: Usuario | Cliente;

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

  constructor(private userService: UserService, private clienteService: ClienteService,private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    this.loadUsuarios();
    this.loadClientes();
  }

  openAddModal(mode: 'user' | 'client') {
    this.isAddModalOpen = true;
    this.addMode = mode; 
    this.selectedItem = mode === 'user' ? new Usuario() : new Cliente();
  
    if (mode === 'user') {
      this.selectedUsuario = new Usuario();
      //this.selectedUsuario.dateOfBirth = '';  // Inicializar como cadena vacía
      this.selectedCliente = null;
    } else if (mode === 'client') {
      this.selectedCliente = new Cliente();
      //this.selectedCliente.fechaNacimiento = '';  // Inicializar como cadena vacía
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
          // Procesar cada usuario para separar nombre y apellido
          this.usuarios = data.map(usuario => {
            //const [nombre, ...resto] = usuario.fullName.split(' ');
            //const apellido = resto.join(' ');
            //console.log(nombre + apellido);
  
            return {
              ...usuario,
              //nombre: nombre,
              //apellido: apellido
              userName: usuario.userName || '-',
              fullName: usuario.fullName || '-',
              email: usuario.email || '-'
            };
          });
  
          this.filteredUsuarios = this.usuarios;
          this.totalUsuarios = this.usuarios.length;

        },
        error => {
          console.error('Error fetching users', error);
        }
      )
    );
  }  

  loadClientes(): void {
    const token = this.authService.getToken();
    //console.log('Token utilizado para la solicitud:', token);
  
    const headers = new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  
    this.clienteService.getClientes().subscribe( // Cambiado para usar getClientes sin parámetros
      (clientes) => {
        console.log('Clientes cargados', clientes);
        this.clientes = clientes;
        this.filteredClientes = this.clientes; // Asegúrate de tener una lista filtrada también
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
    if (this.activeTab === 'table1') {
      this.filteredUsuarios = this.usuarios.filter(usuario => 
        !usuario.isDeleted && 
        (usuario.userName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
         usuario.fullName.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
         usuario.email.toLowerCase().includes(this.searchTerm.toLowerCase()))
      );
    } else if (this.activeTab === 'table2') {
      this.filteredClientes = this.clientes.filter(cliente =>
        (cliente.nombre.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
         cliente.apellido.toLowerCase().includes(this.searchTerm.toLowerCase()) ||
         cliente.email.toLowerCase().includes(this.searchTerm.toLowerCase()))
      );
    }
  }

  // Redirige a la página de perfil
  redirectToProfile() {
    this.router.navigate(['/perfil']); // Asegúrate de que la ruta '/perfil' esté configurada correctamente en tu routing module
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

  deleteUsuario(usuario: Usuario){
    this.selectedUsuario = { ...usuario };
    const confirmacion = confirm(`¿Estás seguro de que deseas eliminar al usuario ${usuario.nombre} ${usuario.apellido}?`);
    if (confirmacion) {
        this.userService.eliminarUsuario(usuario.email).subscribe(
            response => {
                console.log('Usuario eliminado correctamente');
                this.loadUsuarios();
            },
            error => {
                console.error('Error al eliminar el usuario', error);
            }
        );
    }
}

  deleteCliente(cliente: Cliente){
    /*this.selectedCliente = { ...cliente };
    const confirmacion = confirm(`¿Estás seguro de que deseas eliminar al usuario ${cliente.nombre} ${cliente.apellido}?`);
    if (confirmacion) {
        this.clienteService.eliminarCliente(cliente.email).subscribe(
            () => {
                console.log('Usuario eliminado correctamente');
                this.loadClientes();
            },
            error => {
                console.error('Error al eliminar el usuario', error);
            }
        );
    }*/
  }

  closeModal(): void {
    this.isModalOpen = false;
    this.selectedUsuario = null;
    this.selectedCliente = null;
  }

  onSave(): void {
    if (this.editMode === 'user' && this.selectedUsuario) {
      console.log('Usuario guardado:', this.selectedUsuario);
      this.userService.editarUsuario(this.selectedUsuario).subscribe(
        response => {
          console.log('Usuario actualizado correctamente', response);
          this.loadUsuarios();  // Recargar la lista de usuarios
        },
        error => {
          console.error('Error al actualizar el usuario', error);
        }
      );
    } else if (this.editMode === 'client' && this.selectedCliente) {
      console.log('Cliente guardado:', this.selectedCliente);
      this.userService.editarCliente(this.selectedCliente).subscribe(
        response => {
          console.log('Cliente actualizado correctamente', response);
          this.loadClientes();  // Recargar la lista de clientes
        },
        error => {
          console.error('Error al actualizar el cliente', error);
        }
      );
    }
    this.closeModal();
  }

  onAdd() {
    if (this.addMode === 'user' && this.selectedUsuario) {
      // Verificar si dateOfBirth tiene un valor válido antes de convertir
     /* if (this.selectedUsuario.dateOfBirth) {
        this.selectedUsuario.dateOfBirth = new Date(this.selectedUsuario.dateOfBirth).toISOString();
      } else {
        // Si no hay una fecha, puedes optar por no enviar este campo o manejarlo según tu lógica
        this.selectedUsuario.dateOfBirth = ''; // O manejar un valor por defecto
      }
  */
      this.userService.registrarUsuario(this.selectedUsuario).subscribe(
        response => {
          console.log('Usuario añadido correctamente', response);
          this.loadUsuarios();  // Recargar la lista de usuarios
          this.closeAddModal();
        },
        error => {
          console.error('Error al añadir el usuario', error);
        }
      );
    } 
    else if (this.addMode === 'client' && this.selectedCliente){
      this.clienteService.registrarCliente(this.selectedCliente).subscribe(
        response => {
          console.log('Cliente añadido correctamente', response);
          this.loadClientes();  // Recargar la lista de clientes
          this.closeAddModal();
        },
        error => {
          console.error('Error al añadir el cliente', error);
        }
      );
    }
    else {
      this.closeAddModal();
    }
  }  

toTimestamp(dateString: string): number {
  return new Date(dateString).getTime();
}

// Convertir timestamp a fecha ISO para mostrarla en el campo
toDateString(timestamp: number): string {
  return new Date(timestamp).toISOString().split('T')[0];
}

  

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
