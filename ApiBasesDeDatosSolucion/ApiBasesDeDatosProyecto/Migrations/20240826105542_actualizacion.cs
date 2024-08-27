using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBasesDeDatosProyecto.Migrations
{
    /// <inheritdoc />
    public partial class actualizacion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                    CREATE VIEW VistaClientesPaises AS
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
                        Paises p ON c.PaisId = p.Id;
                ");


            migrationBuilder.Sql(@"
                CREATE PROCEDURE ObtenerClientesPorPais
                    @PaisId INT
                AS
                BEGIN
                    SELECT 
                        Id,
                        Nombre,
                        Apellido,
                        Email,
                        FechaNacimiento,
                        Empleo
                    FROM 
                        Clientes
                    WHERE 
                        PaisId = @PaisId;
                END;
                GO
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS ObtenerClientesPorPais");
            migrationBuilder.Sql("DROP VIEW IF EXISTS VistaClientesPaises;");
        }
    }
}
