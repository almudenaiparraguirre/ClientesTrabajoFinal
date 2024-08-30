using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SimuladorServicio
{
    public class AccessMonitoringData
    {
        public string Nombre { get; set; }

        public string Apellido { get; set; }

        public DateTime FechaNacimiento { get; set; }

        public string Empleo { get; set; }

        public int PaisId { get; set; }

        public string Pais { get; set; }

        public string Email { get; set; }

        public string Usuario { get; set; }

        public string TipoAcceso { get; set; }


    }
}
