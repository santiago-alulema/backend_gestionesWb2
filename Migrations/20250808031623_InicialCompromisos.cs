using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class InicialCompromisos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Conversión manual del tipo string a boolean
            migrationBuilder.Sql(@"
        ALTER TABLE ""CompromisosPagos""
        ALTER COLUMN ""Estado"" DROP DEFAULT;
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE ""CompromisosPagos""
        ALTER COLUMN ""Estado"" TYPE boolean
        USING CASE
            WHEN ""Estado"" = 'pendiente' THEN FALSE
            WHEN ""Estado"" = 'pagado' THEN TRUE
            ELSE NULL
        END;
    ");

            // Agrega nueva columna como estaba previsto
            migrationBuilder.AddColumn<bool>(
                name: "IncumplioCompromisoPago",
                table: "CompromisosPagos",
                type: "boolean",
                nullable: true);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IncumplioCompromisoPago",
                table: "CompromisosPagos");

            // Reconvertir de boolean a texto
            migrationBuilder.Sql(@"
        ALTER TABLE ""CompromisosPagos""
        ALTER COLUMN ""Estado"" TYPE character varying(50)
        USING CASE
            WHEN ""Estado"" = TRUE THEN 'pagado'
            WHEN ""Estado"" = FALSE THEN 'pendiente'
            ELSE NULL
        END;
    ");

            migrationBuilder.Sql(@"
        ALTER TABLE ""CompromisosPagos""
        ALTER COLUMN ""Estado"" SET DEFAULT 'pendiente';
    ");
        }

    }
}
