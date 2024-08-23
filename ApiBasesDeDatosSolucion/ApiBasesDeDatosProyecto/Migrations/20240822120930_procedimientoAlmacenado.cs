using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBasesDeDatosProyecto.Migrations
{
    /// <inheritdoc />
    public partial class procedimientoAlmacenado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE PROCEDURE GetClientesPorPais
                    @PaisId INT
                AS
                BEGIN
                    SELECT 
                        c.Id AS ClienteId,
                        c.Nombre AS ClienteNombre,
                        c.Apellido AS ClienteApellido,
                        c.FechaNacimiento,
                        c.Empleo,
                        c.Email,
                        p.Id AS PaisId,
                        p.Nombre AS PaisNombre,
                        p.Divisa,
                        p.Iso3
                    FROM 
                        Clientes c
                    INNER JOIN 
                        Paises p ON c.PaisId = p.Id
                    WHERE 
                        p.Id = @PaisId;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS ObtenerClientesPorPais");
        }
    }
}
