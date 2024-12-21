using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ChangeProductItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderProductItem");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductItems",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "ProductItems");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "ProductItems",
                newName: "OrderId");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "ProductItems",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductItems",
                table: "ProductItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductItems_OrderId",
                table: "ProductItems",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductItems_Orders_OrderId",
                table: "ProductItems",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductItems_Orders_OrderId",
                table: "ProductItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductItems",
                table: "ProductItems");

            migrationBuilder.DropIndex(
                name: "IX_ProductItems_OrderId",
                table: "ProductItems");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductItems");

            migrationBuilder.RenameColumn(
                name: "OrderId",
                table: "ProductItems",
                newName: "Id");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ProductItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "ProductItems",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductItems",
                table: "ProductItems",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "OrderProductItem",
                columns: table => new
                {
                    OrdersId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductItemsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProductItem", x => new { x.OrdersId, x.ProductItemsId });
                    table.ForeignKey(
                        name: "FK_OrderProductItem_Orders_OrdersId",
                        column: x => x.OrdersId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderProductItem_ProductItems_ProductItemsId",
                        column: x => x.ProductItemsId,
                        principalTable: "ProductItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderProductItem_ProductItemsId",
                table: "OrderProductItem",
                column: "ProductItemsId");
        }
    }
}
