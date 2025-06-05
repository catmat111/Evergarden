using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class ForcaMig2 : Migration
    {
       protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Remover FKs antigas (caso já existam)
            migrationBuilder.DropForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas");

            migrationBuilder.DropForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorRemetenteFk",
                table: "Cartas");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Utilizadores_RemetenteId",
                table: "Utilizadores");

            // (Opcional) Se já adicionaste o campo UtilizadorCriadorId em Categorias
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias");

            // Adicionar novamente as FKs com onDelete: Cascade
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

            // (Opcional) FK para as categorias ligadas ao IdentityUser
            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias",
                column: "UtilizadorCriadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Reverter para sem cascata
            migrationBuilder.DropForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas");

            migrationBuilder.DropForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorRemetenteFk",
                table: "Cartas");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_Utilizadores_RemetenteId",
                table: "Utilizadores");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas",
                column: "UtilizadorDestinatarioFk",
                principalTable: "Utilizadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorRemetenteFk",
                table: "Cartas",
                column: "UtilizadorRemetenteFk",
                principalTable: "Utilizadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_Utilizadores_RemetenteId",
                table: "Utilizadores",
                column: "RemetenteId",
                principalTable: "Utilizadores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias",
                column: "UtilizadorCriadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
