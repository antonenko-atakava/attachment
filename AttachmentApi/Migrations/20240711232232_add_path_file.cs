using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttachmentApi.Migrations
{
    /// <inheritdoc />
    public partial class add_path_file : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "file_path",
                table: "attachment",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "file_path",
                table: "attachment");
        }
    }
}
