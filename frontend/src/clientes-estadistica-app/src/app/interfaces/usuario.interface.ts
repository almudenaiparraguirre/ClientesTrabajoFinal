import { Timestamp } from "rxjs";

export interface Usuario {
    Email: string;
    Password: string;
    ConfirmPassword: string;
    Nombre: string;
    Apellido: string;
    PaisNombre: number;
    FechaNacimiento: string; // Asegúrate de que FechaNac sea de tipo Date
  }
  