using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazingNotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHiddenUntilToNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "HiddenUntil",
                table: "Notes",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HiddenUntil",
                table: "Notes");
        }
    }
}
