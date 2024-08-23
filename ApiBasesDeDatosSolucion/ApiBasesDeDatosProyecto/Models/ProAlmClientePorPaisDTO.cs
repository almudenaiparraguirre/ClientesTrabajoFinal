namespace ApiBasesDeDatosProyecto.Models
{
    public class ProAlmClientePorPaisDto
    {
        [Key]
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteApellido { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public string Empleo { get; set; }
        public string Email { get; set; }
        public int PaisId { get; set; }
        public string PaisNombre { get; set; }
        public string Divisa { get; set; }
        public string Iso3 { get; set; }
    }

}
