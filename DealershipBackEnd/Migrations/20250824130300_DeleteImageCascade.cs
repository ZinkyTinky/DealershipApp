using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealershipBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class DeleteImageCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images",
                column: "StockItemId",
                principalTable: "StockItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_StockItems_StockItemId",
                table: "Images",
                column: "StockItemId",
                principalTable: "StockItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
