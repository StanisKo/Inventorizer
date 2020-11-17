using Microsoft.EntityFrameworkCore.Migrations;

namespace Inventorizer_DataAccess.Migrations
{
    public partial class AddOneToManyBetweenCategoryAndItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Category_Id",
                table: "Items",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Items_Category_Id",
                table: "Items",
                column: "Category_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_Categories_Category_Id",
                table: "Items",
                column: "Category_Id",
                principalTable: "Categories",
                principalColumn: "Category_Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_Categories_Category_Id",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_Category_Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Category_Id",
                table: "Items");
        }
    }
}
