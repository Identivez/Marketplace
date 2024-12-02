using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GEJ_Lab.Migrations
{
    /// <inheritdoc />
    public partial class updates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShippingDetailsId",
                table: "PaymentData",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ShippingDetailsId",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingDetailsId",
                table: "PaymentData");

            migrationBuilder.DropColumn(
                name: "ShippingDetailsId",
                table: "Orders");
        }
    }
}
