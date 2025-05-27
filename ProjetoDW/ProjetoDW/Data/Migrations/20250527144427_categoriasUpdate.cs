using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class categoriasUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Utilizadores_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "DataEnvio",
                table: "Categorias");

            

           

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadoresFk",
                table: "Categorias",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            
            

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias",
                column: "UtilizadorCriadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadoresFk",
                table: "Categorias",
                column: "UtilizadoresFk",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefa_AspNetUsers_UtilizadorId",
                table: "Tarefa",
                column: "UtilizadorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Tarefa_AspNetUsers_UtilizadorId",
                table: "Tarefa");

            migrationBuilder.DropIndex(
                name: "IX_Tarefa_UtilizadorId",
                table: "Tarefa");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UtilizadorCriadorId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "Tarefa");

            migrationBuilder.RenameColumn(
                name: "UtilizadorCriadorId",
                table: "Categorias",
                newName: "Topico");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadoresFk",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Categorias",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataEnvio",
                table: "Categorias",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Utilizadores_UtilizadoresFk",
                table: "Categorias",
                column: "UtilizadoresFk",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
