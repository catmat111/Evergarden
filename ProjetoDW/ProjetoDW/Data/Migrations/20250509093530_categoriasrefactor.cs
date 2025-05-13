using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class categoriasrefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Utilizadores_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "Utilizadores",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UtilizadoresFk",
                table: "Categorias",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_IdentityUserId",
                table: "Utilizadores",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadoresFk",
                table: "Categorias",
                column: "UtilizadoresFk",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_AspNetUsers_IdentityUserId",
                table: "Utilizadores",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_AspNetUsers_UtilizadoresFk",
                table: "Categorias");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_AspNetUsers_IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.AlterColumn<int>(
                name: "UtilizadoresFk",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

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
