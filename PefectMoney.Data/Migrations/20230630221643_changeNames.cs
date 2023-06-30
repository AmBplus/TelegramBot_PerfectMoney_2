using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PefectMoney.Data.Migrations
{
    public partial class changeNames : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CodeId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "RoleModels");

            migrationBuilder.RenameColumn(
                name: "BotUserId",
                table: "Users",
                newName: "BotChatId");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "RoleModels",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreationDate", "Name" },
                values: new object[] { new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(7217), "Admin" });

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreationDate", "Name" },
                values: new object[] { new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(7234), "Customer" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(9651));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 7, 1, 1, 46, 43, 575, DateTimeKind.Local).AddTicks(9662));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "RoleModels");

            migrationBuilder.RenameColumn(
                name: "BotChatId",
                table: "Users",
                newName: "BotUserId");

            migrationBuilder.AddColumn<string>(
                name: "ChatId",
                table: "Users",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CodeId",
                table: "Users",
                type: "varchar(200)",
                maxLength: 200,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Role",
                table: "RoleModels",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreationDate", "Role" },
                values: new object[] { new DateTime(2023, 6, 30, 19, 23, 16, 732, DateTimeKind.Local).AddTicks(6824), "Admin" });

            migrationBuilder.UpdateData(
                table: "RoleModels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreationDate", "Role" },
                values: new object[] { new DateTime(2023, 6, 30, 19, 23, 16, 732, DateTimeKind.Local).AddTicks(6840), "Customer" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 19, 23, 16, 732, DateTimeKind.Local).AddTicks(8938));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2L,
                column: "CreationDate",
                value: new DateTime(2023, 6, 30, 19, 23, 16, 732, DateTimeKind.Local).AddTicks(8945));
        }
    }
}
