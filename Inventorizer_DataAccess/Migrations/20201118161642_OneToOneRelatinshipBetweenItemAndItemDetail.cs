using Microsoft.EntityFrameworkCore.Migrations;

namespace Inventorizer_DataAccess.Migrations
{
    public partial class OneToOneRelatinshipBetweenItemAndItemDetail : Migration
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

            migrationBuilder.CreateIndex(
                name: "IX_ItemDetails_Item_Id",
                table: "ItemDetails",
                column: "Item_Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ItemDetails_Items_Item_Id",
                table: "ItemDetails",
                column: "Item_Id",
                principalTable: "Items",
                principalColumn: "Item_Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ItemDetails_Items_Item_Id",
                table: "ItemDetails");

            migrationBuilder.DropIndex(
                name: "IX_ItemDetails_Item_Id",
                table: "ItemDetails");

            migrationBuilder.AddColumn<int>(
                name: "ItemDetail_Id",
                table: "Items",
                type: "integer",
                nullable: true);

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
