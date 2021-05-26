using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AuctionAppIep.Migrations
{
    public partial class AuctionInitial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            

            migrationBuilder.CreateTable(
                name: "auctions",
                columns: table => new
                {
                    id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(nullable: false),
                    name = table.Column<string>(nullable: false),
                    description = table.Column<string>(nullable: false),
                    imageData = table.Column<byte[]>(nullable: false),
                    startingPrice = table.Column<int>(nullable: false),
                    bidPrice = table.Column<int>(nullable: false),
                    dateCreated = table.Column<DateTime>(nullable: false),
                    dateStart = table.Column<DateTime>(nullable: false),
                    dateEnd = table.Column<DateTime>(nullable: false),
                    status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_auctions", x => x.id);
                    table.ForeignKey(
                        name: "FK_auctions_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            
            migrationBuilder.CreateIndex(
                name: "IX_auctions_userId",
                table: "auctions",
                column: "userId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "auctions");
        }
    }
}
