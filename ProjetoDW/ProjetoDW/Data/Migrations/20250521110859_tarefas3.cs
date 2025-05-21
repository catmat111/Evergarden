using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class tarefas3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tarefa",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<string>(
                name: "UtilizadorId",
                table: "Tarefa",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tarefa",
                table: "Tarefa",
                column: "Id");

            

            migrationBuilder.CreateIndex(
                name: "IX_Tarefa_UtilizadorId",
                table: "Tarefa",
                column: "UtilizadorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tarefa_AspNetUsers_UtilizadorId",
                table: "Tarefa",
                column: "UtilizadorId",
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
                name: "FK_Tarefa_AspNetUsers_UtilizadorId",
                table: "Tarefa");

            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_AspNetUsers_IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tarefa",
                table: "Tarefa");

            migrationBuilder.DropIndex(
                name: "IX_Tarefa_UtilizadorId",
                table: "Tarefa");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "Tarefa");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Tarefa",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);
        }
    }
}
