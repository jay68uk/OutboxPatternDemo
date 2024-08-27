using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutBoxPattern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOutboxMessageErrorColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error",
                table: "outbox_messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error",
                table: "outbox_messages");
        }
    }
}
