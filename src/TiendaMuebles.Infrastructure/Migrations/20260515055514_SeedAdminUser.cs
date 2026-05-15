using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TiendaMuebles.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAdminUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "PasswordHash", "Role" },
                values: new object[] { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2026, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), "admin@tiendamuebles.com", "$2a$12$sbwJNiELotlcqbpeF/gVgebq9SIRnIWdZlybAk2Sd6i8wYXqtl.9G", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"));
        }
    }
}
