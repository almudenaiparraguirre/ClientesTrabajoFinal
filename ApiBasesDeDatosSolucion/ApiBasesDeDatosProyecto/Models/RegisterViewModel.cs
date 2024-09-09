using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ApiBasesDeDatosProyecto.Utilidades;
using NuGet.Packaging;

public class RegisterViewModel : IValidatableObject
{
    [Required(ErrorMessage = "El email es requerido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida.")]
    public string Password { get; set; }

    [Required(ErrorMessage = "La confirmación de la contraseña es requerida.")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    public string ConfirmPassword { get; set; }

    public string? Empleo { get; set; }

    [Required(ErrorMessage = "El nombre es requerido.")]
    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es requerida.")]
    public DateTime DateOfBirth { get; set; }
    
    public string? PaisNombre { get; set; }


    // Método de validación

    IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        var errors = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

        // Llamada a los métodos de extensión para realizar las validaciones
        errors.AddRange(Email.ValidateEmail());
        errors.AddRange(Password.ValidatePassword());

        if (Password != ConfirmPassword)
        {
            errors.Add(new System.ComponentModel.DataAnnotations.ValidationResult("Las contraseñas no coinciden.", new[] { nameof(ConfirmPassword) }));
        }

        if (!string.IsNullOrEmpty(Nombre))
        {
            errors.AddRange(Nombre.ValidateNombre());
        }



        return errors;
    }
}
