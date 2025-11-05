using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace gestiones_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS audit;");
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS pgcrypto;"); // por si usas gen_random_uuid()

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "audit",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    EventDateUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "now()"),
                    UserName = table.Column<string>(type: "text", nullable: true),
                    Action = table.Column<string>(type: "text", nullable: false),
                    Table = table.Column<string>(type: "text", nullable: false),
                    KeyJson = table.Column<string>(type: "jsonb", nullable: true),
                    BeforeJson = table.Column<string>(type: "jsonb", nullable: true),
                    AfterJson = table.Column<string>(type: "jsonb", nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_audit_logs", x => x.Id); });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_EventDateUtc",
                schema: "audit",
                table: "audit_logs",
                column: "EventDateUtc");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_Table_Action",
                schema: "audit",
                table: "audit_logs",
                columns: new[] { "Table", "Action" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "audit_logs", schema: "audit");
        }

    }
}
