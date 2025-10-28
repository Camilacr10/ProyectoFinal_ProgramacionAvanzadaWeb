using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProyectoFinalDAL.Migrations
{
    /// <inheritdoc />
    public partial class InicialIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Solicitudes",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes");

            migrationBuilder.AlterColumn<string>(
                name: "Estado",
                table: "Solicitudes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddForeignKey(
                name: "FK_Solicitudes_Clientes_IdCliente",
                table: "Solicitudes",
                column: "IdCliente",
                principalTable: "Clientes",
                principalColumn: "IdCliente",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
