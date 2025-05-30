using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemocaoTopico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Topico",
                table: "Cartas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Topico",
                table: "Cartas",
                type: "TEXT",
                nullable: true);
        }
    }
}
