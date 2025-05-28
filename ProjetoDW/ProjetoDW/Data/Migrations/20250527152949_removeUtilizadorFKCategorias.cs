using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class removeUtilizadorFKCategorias : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UtilizadoresFk",
                table: "Categorias");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UtilizadoresFk",
                table: "Categorias",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UtilizadoresFk",
                table: "Categorias",
                column: "UtilizadoresFk");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadoresFk",
                table: "Categorias",
                column: "UtilizadoresFk",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
