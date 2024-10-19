using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Book_Store.Migrations
{
    /// <inheritdoc />
    public partial class cartAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CartID",
                table: "CartItems",
                newName: "CartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CartId",
                table: "CartItems",
                newName: "CartID");
        }
    }
}
