using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImagenstesteUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Utilizadores_AspNetUsers_IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.DropIndex(
                name: "IX_Utilizadores_IdentityUserId",
                table: "Utilizadores");

            migrationBuilder.RenameColumn(
                name: "IdentityUserId",
                table: "Utilizadores",
                newName: "IdentityUserID");

            migrationBuilder.AddColumn<string>(
                name: "IdentityUser",
                table: "Utilizadores",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IdentityUser",
                table: "Utilizadores");

            migrationBuilder.RenameColumn(
                name: "IdentityUserID",
                table: "Utilizadores",
                newName: "IdentityUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Utilizadores_IdentityUserId",
                table: "Utilizadores",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Utilizadores_AspNetUsers_IdentityUserId",
                table: "Utilizadores",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
