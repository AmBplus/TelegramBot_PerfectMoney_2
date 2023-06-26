using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TelegramBot_PerfectMoney.Migrations
{
    public partial class modifedBandCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ShabaNumber",
                table: "BankCart",
                newName: "CartNumber");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "BankCart",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 25, 22, 56, 55, 460, DateTimeKind.Local).AddTicks(6283));

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 25, 22, 56, 55, 460, DateTimeKind.Local).AddTicks(6305));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 25, 22, 56, 55, 460, DateTimeKind.Local).AddTicks(9193));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "BankCart");

            migrationBuilder.RenameColumn(
                name: "CartNumber",
                table: "BankCart",
                newName: "ShabaNumber");

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 25, 22, 55, 4, 481, DateTimeKind.Local).AddTicks(1320));

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 25, 22, 55, 4, 481, DateTimeKind.Local).AddTicks(1338));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 25, 22, 55, 4, 481, DateTimeKind.Local).AddTicks(3399));
        }
    }
}
