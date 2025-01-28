using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExampleEf.Migrations
{
    /// <inheritdoc />
    public partial class add_indexesName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Adi",
                table: "Urunler",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Urunler_Adi",
                table: "Urunler",
                column: "Adi");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Urunler_Adi",
                table: "Urunler");

            migrationBuilder.AlterColumn<string>(
                name: "Adi",
                table: "Urunler",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
