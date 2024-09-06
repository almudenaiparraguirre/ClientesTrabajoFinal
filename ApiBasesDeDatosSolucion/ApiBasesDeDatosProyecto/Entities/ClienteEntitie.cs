namespace ApiBasesDeDatosProyecto.Entities
{
    [Table("Clientes")]
    public class Cliente 
    {
        [Key]
        public int Id { get; set; }

        //[ForeignKey(nameof(Usuario))]
        //public int UserId { get; set; }

        [Required]
        [StringLength(25)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(30)]
        public string Apellido { get; set; }

        [Required]
        public DateTime dateOfBirth { get; set; }

        public string Empleo { get; set; }

        [Required]
        public int PaisId { get; set; }

        //[ForeignKey(nameof(PaisId))]
        public Pais Pais { get; set; }  // Navegación a la entidad Pais

        //public Usuario? Usuario { get; set; }  // Navegación a la entidad Usuario

        [Required]

        public string Email { get; set; }

        
    }
}
