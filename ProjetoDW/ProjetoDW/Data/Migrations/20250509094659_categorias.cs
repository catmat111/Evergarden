using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class categorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UtilizadorCriadorId",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UtilizadorCriadorId",
                table: "Categorias",
                column: "UtilizadorCriadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias",
                column: "UtilizadorCriadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UtilizadorCriadorId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UtilizadorCriadorId",
                table: "Categorias");
        }
    }
}
