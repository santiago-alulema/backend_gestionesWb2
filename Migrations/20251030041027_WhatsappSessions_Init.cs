using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class WhatsappSessions_Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WhatsappSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    User = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Ready = table.Column<bool>(type: "boolean", nullable: false),
                    ExistsOnNode = table.Column<bool>(type: "boolean", nullable: false),
                    LastReason = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WhatsappSessions", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WhatsappSessions_User",
                table: "WhatsappSessions",
                column: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WhatsappSessions");
        }
    }
}
