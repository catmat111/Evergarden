using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoDW.Data.Migrations
{
    /// <inheritdoc />
    public partial class CartasCategoriasRelacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CartaCategoria",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CartaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CategoriaId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CartaCategoria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CartaCategoria_Cartas_CartaId",
                        column: x => x.CartaId,
                        principalTable: "Cartas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CartaCategoria_Categorias_CategoriaId",
                        column: x => x.CategoriaId,
                        principalTable: "Categorias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CartaCategoria_CartaId",
                table: "CartaCategoria",
                column: "CartaId");

            migrationBuilder.CreateIndex(
                name: "IX_CartaCategoria_CategoriaId",
                table: "CartaCategoria",
                column: "CategoriaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CartaCategoria");
        }
    }
}
