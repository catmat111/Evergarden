using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class ListaDestinatarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RemetenteId",
                table: "Utilizadores",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_RemetenteId",
                table: "Utilizadores",
                column: "RemetenteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_Utilizadores_RemetenteId",
                table: "Utilizadores",
                column: "RemetenteId",
                principalTable: "Utilizadores",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Utilizadores_RemetenteId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_RemetenteId",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "RemetenteId",
                table: "Utilizadores");
        }
    }
}
