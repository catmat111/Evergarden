using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForcaMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
// Drop FK antiga do Destinatário
            migrationBuilder.DropForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas");

            // Drop FK antiga do Remetente
            migrationBuilder.DropForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorRemetenteFk",
                table: "Cartas");

            // Drop FK antiga do RemetenteId (self-referencing)
            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Utilizadores_RemetenteId",
                table: "Utilizadores");

            // Adiciona novamente com Cascade
            migrationBuilder.AddForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas",
                column: "UtilizadorDestinatarioFk",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorRemetenteFk",
                table: "Cartas",
                column: "UtilizadorRemetenteFk",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_Utilizadores_RemetenteId",
                table: "Utilizadores",
                column: "RemetenteId",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
