using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace courseWork.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FixForeightKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderNumber1",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderNumber1",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "OrderNumber1",
                table: "OrderDetails");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderNumber",
                table: "OrderDetails",
                column: "OrderNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderNumber",
                table: "OrderDetails",
                column: "OrderNumber",
                principalTable: "Orders",
                principalColumn: "OrderNumber",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Orders_OrderNumber",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_OrderNumber",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "OrderNumber1",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderNumber1",
                table: "OrderDetails",
                column: "OrderNumber1");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Orders_OrderNumber1",
                table: "OrderDetails",
                column: "OrderNumber1",
                principalTable: "Orders",
                principalColumn: "OrderNumber",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
