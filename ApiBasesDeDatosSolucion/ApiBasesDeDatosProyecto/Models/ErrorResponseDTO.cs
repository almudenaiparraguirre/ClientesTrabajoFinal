namespace ApiBasesDeDatosProyecto.Models
{
    public class ErrorResponseDTO
    {
        public string Message { get; set; }
        public List<string> Details { get; set; }

        public ErrorResponseDTO(string message, List<string> details = null)
        {
            Message = message;
            Details = details ?? new List<string>();
        }
    }

}
