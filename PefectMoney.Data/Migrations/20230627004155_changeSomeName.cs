using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PefectMoney.Data.Migrations
{
    public partial class changeSomeName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 27, 4, 11, 55, 189, DateTimeKind.Local).AddTicks(669));

            migrationBuilder.UpdateData(
                table: "RoleModel",
                keyColumn: "id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 27, 4, 11, 55, 189, DateTimeKind.Local).AddTicks(682));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 27, 4, 11, 55, 189, DateTimeKind.Local).AddTicks(2288));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
