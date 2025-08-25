using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealershipBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class DeleteBehaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAccessories_StockItems_StockItemId",
                table: "StockAccessories");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images",
                column: "StockItemId",
                principalTable: "StockItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAccessories_StockItems_StockItemId",
                table: "StockAccessories",
                column: "StockItemId",
                principalTable: "StockItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_StockAccessories_StockItems_StockItemId",
                table: "StockAccessories");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images",
                column: "StockItemId",
                principalTable: "StockItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StockAccessories_StockItems_StockItemId",
                table: "StockAccessories",
                column: "StockItemId",
                principalTable: "StockItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
