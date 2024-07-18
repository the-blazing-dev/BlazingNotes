using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazingNotes.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIsArchivedToNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Notes");
        }
    }
}
