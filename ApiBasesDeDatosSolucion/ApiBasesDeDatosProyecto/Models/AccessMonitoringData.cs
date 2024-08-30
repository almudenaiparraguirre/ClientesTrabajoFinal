namespace ApiBasesDeDatosProyecto.Models
{
    public class AccessMonitoringData
    {
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public DateTime FechaNacimiento { get; set; }


        [Key]
        public string Empleo { get; set; }

        public int PaisId { get; set; }

        public string Pais { get; set; }

        public string Email { get; set; }

        public string Usuario { get; set; }

        public string TipoAcceso { get; set; }

        public DateTime FechaRecibido { get; set; }
    }
}
