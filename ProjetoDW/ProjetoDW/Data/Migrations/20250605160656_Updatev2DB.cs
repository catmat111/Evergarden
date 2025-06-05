using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class Updatev2DB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Utilizadores_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "DataEnvio",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "Topico",
                table: "Cartas");

            

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Categorias",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorDestinatarioFk",
                table: "Cartas",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Cartas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Cartas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataEnvio",
                table: "Cartas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "TEXT");

            

            migrationBuilder.AddForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas",
                column: "UtilizadorDestinatarioFk",
                principalTable: "Utilizadores",
                principalColumn: "Id");

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
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadorCriadorId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_UtilizadorCriadorId",
                table: "Categorias");

            migrationBuilder.RenameColumn(
                name: "UtilizadorCriadorId",
                table: "Categorias",
                newName: "Topico");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Categorias",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

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

            migrationBuilder.AddColumn<int>(
                name: "UtilizadoresFk",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadorDestinatarioFk",
                table: "Cartas",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Titulo",
                table: "Cartas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Descricao",
                table: "Cartas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<DateTime>(
                name: "DataEnvio",
                table: "Cartas",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Topico",
                table: "Cartas",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_UtilizadoresFk",
                table: "Categorias",
                column: "UtilizadoresFk");

            migrationBuilder.AddForeignKey(
                name: "FK_Cartas_Utilizadores_UtilizadorDestinatarioFk",
                table: "Cartas",
                column: "UtilizadorDestinatarioFk",
                principalTable: "Utilizadores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
