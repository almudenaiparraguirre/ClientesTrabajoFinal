import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { UserService } from '../servicios/user.service';
import { Usuario } from '../interfaces/usuario.interface';
import { CambioRolModel } from '../interfaces/cambioRol.interface';

@Component({
  selector: 'app-registro',
  templateUrl: './registro.component.html',
  styleUrls: ['./registro.component.css']
})
export class RegistroComponent implements OnInit {

  registroForm: FormGroup;
  errorMessage: string;
  isErrorVisible = false;

  constructor(
    private fb: FormBuilder, 
    private miServicio: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.registroForm = this.fb.group({
      Nombre: ['', [Validators.required, Validators.minLength(3)]],
      Apellido: ['', [Validators.required, Validators.minLength(3)]],
      Correo: ['', [Validators.required, Validators.email]],
      Contraseña: ['', [Validators.required, Validators.minLength(6), this.passwordStrengthValidator]],
      Contraseña2: ['', [Validators.required]],
      Rol: ['Client', Validators.required],
      PaisNombre: ['', Validators.required],
      Empleo: [''],
      FechaNac: ['', Validators.required]
    }, {
      validator: this.passwordMatchValidator('Contraseña', 'Contraseña2')
    });
  }

  passwordMatchValidator(password: string, confirmPassword: string) {
    return (formGroup: FormGroup) => {
      const passwordControl = formGroup.get(password);
      const confirmPasswordControl = formGroup.get(confirmPassword);

      if (confirmPasswordControl?.errors && !confirmPasswordControl.errors['passwordMismatch']) {
        return;
      }

      if (passwordControl?.value !== confirmPasswordControl?.value) {
        confirmPasswordControl?.setErrors({ passwordMismatch: true });
      } else {
        confirmPasswordControl?.setErrors(null);
      }
    };
  }

  passwordStrengthValidator(control: any) {
    const value = control.value || '';
    if (!/[A-Z]/.test(value)) {
      return { passwordStrength: 'La contraseña debe tener al menos una letra mayúscula.' };
    }
    if (!/[a-z]/.test(value)) {
      return { passwordStrength: 'La contraseña debe tener al menos una letra minúscula.' };
    }
    if (!/[0-9]/.test(value)) {
      return { passwordStrength: 'La contraseña debe tener al menos un número.' };
    }
    return null;
  }

  onSubmit(): void {
    if (this.registroForm.invalid) {
      this.showValidationErrors();
      return;
    }

    const fechaNacTimestamp = new Date(this.registroForm.value.FechaNac).getTime();
  
    const usuario: Usuario = {
      Email: this.registroForm.value.Correo,
      Password: this.registroForm.value.Contraseña,
      ConfirmPassword: this.registroForm.value.Contraseña2,
      Nombre: this.registroForm.value.Nombre,
      Apellido: this.registroForm.value.Apellido,
      PaisNombre: this.registroForm.value.PaisNombre,
      FechaNacimiento: fechaNacTimestamp
    };

    // Registrar usuario
    this.miServicio.registrarUsuario(usuario).subscribe(
      response => {
        const datosCambioRol: CambioRolModel = {
          Email: this.registroForm.value.Correo,
          NuevoRol: "Client",
          Nombre: this.registroForm.value.Nombre,
          Apellido: this.registroForm.value.Apellido,
          Pais: this.registroForm.value.PaisNombre,
          Empleo: this.registroForm.value.Empleo,
          FechaNacimiento: fechaNacTimestamp
        };

        this.miServicio.añadirRolUsuario(datosCambioRol).subscribe(
          response => {
            this.router.navigate(['/login']);
          },
          error => {
            this.handleError(error, 'Error al añadir rol');
          }
        );
      },
      error => {
        this.handleError(error, 'Error al registrar usuario');
      }
    );
  }

  handleError(error: any, prefix: string) {
    if (error.status === 400) {
      this.showError(`${prefix}: ${error.error}`);
    } else if (error.status === 0) {
      this.showError('No se pudo conectar al servidor.');
    } else {
      this.showError(`${prefix}: ${error.message}`);
    }
  }

  showError(message: string) {
    this.errorMessage = message;
    this.isErrorVisible = true;
    setTimeout(() => this.isErrorVisible = false, 3000);
  }

  private showValidationErrors(): void {
    let errorMessage = 'Por favor corrige los siguientes errores:\n';
    const controls = this.registroForm.controls;

    for (const key in controls) {
      if (controls.hasOwnProperty(key)) {
        const control = controls[key];
        if (control.invalid) {
          if (control.errors) {
            if (control.errors['required']) {
              errorMessage += `- El campo ${key} es requerido.\n`;
            }
            if (control.errors['minlength']) {
              errorMessage += `- El campo ${key} debe tener al menos ${control.errors['minlength'].requiredLength} caracteres.\n`;
            }
            if (control.errors['email']) {
              errorMessage += `- El campo ${key} debe ser un email válido.\n`;
            }
            if (control.errors['passwordMismatch']) {
              errorMessage += `- Las contraseñas no coinciden.\n`;
            }
            if (control.errors['passwordStrength']) {
              errorMessage += `- ${control.errors['passwordStrength']}\n`;
            }
          }
        }
      }
    }

    this.showError(errorMessage);
  }
}
