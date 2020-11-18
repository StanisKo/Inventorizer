using Microsoft.EntityFrameworkCore.Migrations;

namespace Inventorizer_DataAccess.Migrations
{
    public partial class AddAnotherMigrationToCreateTableOnly : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemDetails_ItemDetail_Id",
                table: "Items");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemDetail_Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemDetail_Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Item_Id",
                table: "ItemDetails");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemDetail_Id",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Item_Id",
                table: "ItemDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemDetail_Id",
                table: "Items",
                column: "ItemDetail_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Items_ItemDetails_ItemDetail_Id",
                table: "Items",
                column: "ItemDetail_Id",
                principalTable: "ItemDetails",
                principalColumn: "ItemDetail_Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
