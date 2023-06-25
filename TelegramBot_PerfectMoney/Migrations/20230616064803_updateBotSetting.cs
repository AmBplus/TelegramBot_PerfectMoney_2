using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot_PerfectMoney.Migrations
{
    public partial class updateBotSetting : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "botSettings",
                keyColumn: "id",
                keyValue: 1L);

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "RoleModel",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "RuleText",
                table: "botSettings",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 16, 11, 18, 2, 822, DateTimeKind.Local).AddTicks(4296));

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 16, 11, 18, 2, 822, DateTimeKind.Local).AddTicks(4332));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 16, 11, 18, 2, 822, DateTimeKind.Local).AddTicks(5357));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RuleText",
                table: "botSettings");

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "Role",
                keyValue: null,
                column: "Role",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Role",
                table: "RoleModel",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 15, 18, 56, 53, 840, DateTimeKind.Local).AddTicks(1982));

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 15, 18, 56, 53, 840, DateTimeKind.Local).AddTicks(2014));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 15, 18, 56, 53, 840, DateTimeKind.Local).AddTicks(3083));

            migrationBuilder.InsertData(
                table: "botSettings",
                columns: new[] { "id", "StopSelling" },
                values: new object[] { 1L, false });
        }
    }
}
