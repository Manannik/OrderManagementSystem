using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Notification.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_data",
                columns: table => new
                {
                    OrderId = table.Column<Guid>(type: "uuid", nullable: false),
                    Stage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_data", x => x.OrderId);
                });

            migrationBuilder.CreateTable(
                name: "user_state",
                columns: table => new
                {
                    TelegramUserId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastUserMessage = table.Column<int>(type: "integer", nullable: true),
                    Pages = table.Column<string>(type: "text", nullable: false),
                    UserDataId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_state", x => x.TelegramUserId);
                    table.ForeignKey(
                        name: "FK_user_state_user_data_UserDataId",
                        column: x => x.UserDataId,
                        principalTable: "user_data",
                        principalColumn: "OrderId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_state_UserDataId",
                table: "user_state",
                column: "UserDataId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_state");

            migrationBuilder.DropTable(
                name: "user_data");
        }
    }
}
