using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimpleUrlShortener.Core.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ShortUrls",
                columns: table => new
                {
                    Path = table.Column<string>(type: "varchar(10)", nullable: false, collation: "utf8mb4_bin"),
                    Destination = table.Column<string>(type: "longtext", nullable: false, collation: "utf8mb4_bin"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShortUrls", x => x.Path);
                })
                .Annotation("Relational:Collation", "utf8mb4_bin");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ShortUrls");
        }
    }
}
