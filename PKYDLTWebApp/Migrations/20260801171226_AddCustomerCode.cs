using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PKYDLTWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CustomerCode",
                table: "Customers",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CustomerCode",
                table: "Customers");
        }
    }
}
