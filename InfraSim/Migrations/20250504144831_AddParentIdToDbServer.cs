using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfraSim.Migrations
{
    /// <inheritdoc />
    public partial class AddParentIdToDbServer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentId",
                table: "DbServers",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "DbServers");
        }
    }
}
