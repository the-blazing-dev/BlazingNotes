using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazingNotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDateTimeColumnsToNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // manually reordered the statements in hope to have the same ordering in the database
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(2000,01,01, 0,0,0,DateTimeKind.Utc)); // configured manually

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedAt",
                table: "Notes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RelevantAt",
                table: "Notes",
                type: "TEXT",
                nullable: true);
            
            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedAt",
                table: "Notes",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Notes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArchivedAt",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "ModifiedAt",
                table: "Notes");

            migrationBuilder.DropColumn(
                name: "RelevantAt",
                table: "Notes");
        }
    }
}
