
using System.ComponentModel.DataAnnotations;
namespace ApiBasesDeDatosProyecto.Utilidades
{
    public static class ValidationExtensions
    {
        public static IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> ValidateEmail(this string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("El email es requerido.");
            }

            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new Regex(pattern);
            if (!regex.IsMatch(email))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("El formato del email no es válido.");
            }
        }

        public static IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePassword(this string password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("La contraseña es requerida.");
            }

            if (password.Length < 6)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("La contraseña debe tener al menos 6 caracteres.");
            }

            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("La contraseña debe tener al menos una letra mayúscula.");
            }

            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("La contraseña debe tener al menos una letra minúscula.");
            }

            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("La contraseña debe tener al menos un número.");
            }
        }

        public static IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> ValidateNombre(this string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("El nombre es requerido.");
            }

            if (!Regex.IsMatch(nombre, @"^[a-zA-Z\s]+$"))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("El nombre solo debe contener letras y espacios.");
            }
        }

        public static IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> ValidateFechaNacimiento(this long fechaNacimiento)
        {
            var dateTime = DateTimeOffset.FromUnixTimeMilliseconds(fechaNacimiento).DateTime;

            if (dateTime > DateTime.Now)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("La fecha de nacimiento no puede ser en el futuro.");
            }
        }

        public static IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> ValidatePaisNombre(this string paisNombre)
        {
            if (string.IsNullOrWhiteSpace(paisNombre))
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("El país es requerido.");
            }

            if (paisNombre.Length < 2)
            {
                yield return new System.ComponentModel.DataAnnotations.ValidationResult("El nombre del país debe tener al menos 2 caracteres.");
            }
        }
    }
}
