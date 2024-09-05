namespace ApiBasesDeDatosProyecto.Models
{
    public class EditViewModel
    {
        [Required]
        public string Email { get; set; }

        public string? Empleo { get; set; }

        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public  DateTime FechaNacimiento { get; set; }
        public int PaisId { get; set; }
    }
}
