using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PR3MVC.Migrations
{
    /// <inheritdoc />
    public partial class DropSkuFromProductos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Productos_SKU",
                table: "Productos");

            migrationBuilder.DropColumn(
                name: "SKU",
                table: "Productos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SKU",
                table: "Productos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Productos_SKU",
                table: "Productos",
                column: "SKU",
                unique: true,
                filter: "[SKU] IS NOT NULL");
        }
    }
}
