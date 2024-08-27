using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutBoxPattern.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOutboxMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_processed",
                table: "outbox_messages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_processed",
                table: "outbox_messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
