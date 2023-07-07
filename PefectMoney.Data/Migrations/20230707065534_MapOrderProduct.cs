using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PefectMoney.Data.Migrations
{
    public partial class MapOrderProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.CreateTable(
                name: "ProductEntity",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductEntity", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    trackId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalDollarPrice = table.Column<double>(type: "double", nullable: false),
                    TotalRialsPrice = table.Column<double>(type: "double", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    BotChatId = table.Column<long>(type: "bigint", nullable: false),
                    Rial = table.Column<int>(type: "int", nullable: false),
                    Dollar = table.Column<double>(type: "double", nullable: false),
                    VoicherCodeId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_ProductEntity_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "VoicherCodes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VoicherCode = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserBotChatId = table.Column<long>(type: "bigint", nullable: false),
                    OrderId = table.Column<long>(type: "bigint", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoicherCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoicherCodes_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "ProductEntity",
                columns: new[] { "Id", "CreationDate", "Name" },
                values: new object[] { 1L, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "VoicherCode" });

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2023, 7, 7, 10, 25, 33, 719, DateTimeKind.Local).AddTicks(3908));

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2023, 7, 7, 10, 25, 33, 719, DateTimeKind.Local).AddTicks(3925));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreationDate", "PhoneNumber" },
                values: new object[] { new DateTime(2023, 7, 7, 10, 25, 33, 719, DateTimeKind.Local).AddTicks(5967), "+989308505480" });

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductId",
                table: "Orders",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_VoicherCodes_OrderId",
                table: "VoicherCodes",
                column: "OrderId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoicherCodes");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "ProductEntity");

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(7217));

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(7234));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "CreationDate", "PhoneNumber" },
                values: new object[] { new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(9651), "+989394059810" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Active", "BotChatId", "CreationDate", "FirstName", "LastName", "PhoneNumber", "RoleId", "UserNameTelegram" },
                values: new object[] { 2L, true, 0L, new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(9662), null, null, "+989308505480", 1, null });
        }
    }
}
