using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot_PerfectMoney.Migrations
{
    public partial class UpdateBotSettingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 16, 11, 19, 34, 58, DateTimeKind.Local).AddTicks(2375));

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 16, 11, 19, 34, 58, DateTimeKind.Local).AddTicks(2407));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 16, 11, 19, 34, 58, DateTimeKind.Local).AddTicks(3787));

            migrationBuilder.InsertData(
                table: "botSettings",
                columns: new[] { "id", "RuleText", "StopSelling" },
                values: new object[] { 1L, "متنی وجود ندارد", false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "botSettings",
                keyColumn: "id",
                keyValue: 1L);

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
    }
}
