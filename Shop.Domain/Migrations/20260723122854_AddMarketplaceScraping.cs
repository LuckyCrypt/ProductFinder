using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddMarketplaceScraping : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Stores",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastError",
                table: "Offers",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastStatus",
                table: "Offers",
                type: "character varying(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Stores_Code",
                table: "Stores",
                column: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Stores_Code",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Stores");

            migrationBuilder.DropColumn(
                name: "LastError",
                table: "Offers");

            migrationBuilder.DropColumn(
                name: "LastStatus",
                table: "Offers");
        }
    }
}
