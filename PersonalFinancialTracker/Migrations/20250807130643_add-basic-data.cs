using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinancialTracker.Migrations
{
    public partial class addbasicdata : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EntryCreatedDate",
                table: "Transactions",
                newName: "UpdatedTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Transactions",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Categories",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Categories",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Accounts",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Accounts",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "UpdatedTime",
                table: "Transactions",
                newName: "EntryCreatedDate");
        }
    }
}
