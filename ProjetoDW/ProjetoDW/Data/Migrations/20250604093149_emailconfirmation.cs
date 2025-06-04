using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class emailconfirmation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            

           

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
                name: "FK_Tarefa_AspNetUsers_UtilizadorId",
                table: "Tarefa");

            migrationBuilder.DropIndex(
                name: "IX_Tarefa_UtilizadorId",
                table: "Tarefa");

            migrationBuilder.DropColumn(
                name: "UtilizadorId",
                table: "Tarefa");
        }
    }
}
