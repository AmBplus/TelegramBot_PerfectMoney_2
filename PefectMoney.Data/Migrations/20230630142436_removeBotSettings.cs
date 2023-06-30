using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PefectMoney.Data.Migrations
{
    public partial class removeBotSettings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "botSettings",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.UpdateData(
                table: "botSettings",
                keyColumn: "RuleText",
                keyValue: null,
                column: "RuleText",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "RuleText",
                table: "botSettings",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 17, 54, 36, 591, DateTimeKind.Local).AddTicks(1671));

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 17, 54, 36, 591, DateTimeKind.Local).AddTicks(1687));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 17, 54, 36, 591, DateTimeKind.Local).AddTicks(3409));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 17, 54, 36, 591, DateTimeKind.Local).AddTicks(3418));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RuleText",
                table: "botSettings",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 5, 48, 8, 599, DateTimeKind.Local).AddTicks(5829));

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 5, 48, 8, 599, DateTimeKind.Local).AddTicks(5856));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 5, 48, 8, 599, DateTimeKind.Local).AddTicks(9497));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 5, 48, 8, 599, DateTimeKind.Local).AddTicks(9510));

            migrationBuilder.InsertData(
                table: "botSettings",
                columns: new[] { "id", "RuleText", "StopSelling" },
                values: new object[] { 1L, "متنی وجود ندارد", false });
        }
    }
}
