using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalDAL.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
