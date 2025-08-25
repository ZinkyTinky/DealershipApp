using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealershipBackEnd.Migrations
{
    /// <inheritdoc />
    public partial class AddImageBinaryToImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Data",
                table: "Images",
                newName: "ImageBinary");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageBinary",
                table: "Images",
                newName: "Data");
        }
    }
}
