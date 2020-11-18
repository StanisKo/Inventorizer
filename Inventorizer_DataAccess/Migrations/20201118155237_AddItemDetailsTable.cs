using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Inventorizer_DataAccess.Migrations
{
    public partial class AddItemDetailsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ItemDetail_Id",
                table: "Items",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ItemDetails",
                columns: table => new
                {
                    ItemDetail_Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Item_Id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDetails", x => x.ItemDetail_Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Items_ItemDetails_ItemDetail_Id",
                table: "Items");

            migrationBuilder.DropTable(
                name: "ItemDetails");

            migrationBuilder.DropIndex(
                name: "IX_Items_ItemDetail_Id",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "ItemDetail_Id",
                table: "Items");
        }
    }
}
