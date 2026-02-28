using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestionStock.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTypeCateogorieNullDefault : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Categorie",
                table: "Produits",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "Sport",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Categorie",
                table: "Produits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true,
                oldDefaultValue: "Sport");
        }
    }
}
