using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiBasesDeDatosProyecto.Migrations
{
    public partial class AgregarProcedimientoAlmacenado : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP PROCEDURE IF EXISTS ObtenerClientesPorPais");
        }
    }
}
