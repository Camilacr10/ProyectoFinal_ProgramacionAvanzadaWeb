using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalDAL.Migrations
{
    /// <inheritdoc />
    public partial class SyncManualChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Apellido",
                table: "Clientes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Apellido",
                table: "Clientes");
        }
    }
}
