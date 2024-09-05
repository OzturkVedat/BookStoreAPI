using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookStoreBackend.Migrations
{
    /// <inheritdoc />
    public partial class sec : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullName",
                schema: "dbo",
                table: "Authors");

            migrationBuilder.AddColumn<string>(
                name: "Biography",
                schema: "dbo",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                schema: "dbo",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                schema: "dbo",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                schema: "dbo",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biography",
                schema: "dbo",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "FirstName",
                schema: "dbo",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "LastName",
                schema: "dbo",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Nationality",
                schema: "dbo",
                table: "Authors");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                schema: "dbo",
                table: "Authors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
