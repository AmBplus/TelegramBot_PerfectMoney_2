using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PefectMoney.Data.Migrations
{
    public partial class ACTIVE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreationDate",
                value: new DateTime(2023, 7, 7, 20, 35, 3, 804, DateTimeKind.Local).AddTicks(7598));

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreationDate",
                value: new DateTime(2023, 7, 7, 20, 35, 3, 804, DateTimeKind.Local).AddTicks(7628));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 7, 7, 20, 35, 3, 804, DateTimeKind.Local).AddTicks(9940));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                column: "CreationDate",
                value: new DateTime(2023, 7, 7, 10, 25, 33, 719, DateTimeKind.Local).AddTicks(5967));
        }
    }
}
