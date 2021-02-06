using Microsoft.EntityFrameworkCore.Migrations;

namespace SalesManagementApp.Migrations
{
    public partial class RequiredRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_SalesPeople_SalesPersonFullName",
                table: "Sales");

            migrationBuilder.AlterColumn<string>(
                name: "SalesPersonFullName",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_SalesPeople_SalesPersonFullName",
                table: "Sales",
                column: "SalesPersonFullName",
                principalTable: "SalesPeople",
                principalColumn: "FullName",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_SalesPeople_SalesPersonFullName",
                table: "Sales");

            migrationBuilder.AlterColumn<string>(
                name: "SalesPersonFullName",
                table: "Sales",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_SalesPeople_SalesPersonFullName",
                table: "Sales",
                column: "SalesPersonFullName",
                principalTable: "SalesPeople",
                principalColumn: "FullName",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
