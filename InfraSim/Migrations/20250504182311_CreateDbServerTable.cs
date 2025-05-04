using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfraSim.Migrations
{
    /// <inheritdoc />
    public partial class CreateDbServerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DbServers_ParentId",
                table: "DbServers",
                column: "ParentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DbServers_ParentId",
                table: "DbServers");
        }
    }
}
