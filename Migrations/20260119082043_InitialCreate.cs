using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cat = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "height",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cm = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_height", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "width",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    cm = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_width", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "paintings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    height_id = table.Column<int>(type: "integer", nullable: true),
                    width_id = table.Column<int>(type: "integer", nullable: true),
                    category_id = table.Column<int>(type: "integer", nullable: true),
                    name = table.Column<string>(type: "text", nullable: true),
                    image_link = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paintings", x => x.id);
                    table.ForeignKey(
                        name: "FK_paintings_category_category_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_paintings_height_height_id",
                        column: x => x.height_id,
                        principalTable: "height",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_paintings_width_width_id",
                        column: x => x.width_id,
                        principalTable: "width",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_category_id",
                table: "category",
                column: "id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_paintings_category_id",
                table: "paintings",
                column: "category_id");

            migrationBuilder.CreateIndex(
                name: "IX_paintings_height_id",
                table: "paintings",
                column: "height_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_paintings_width_id",
                table: "paintings",
                column: "width_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "paintings");

            migrationBuilder.DropTable(
                name: "category");

            migrationBuilder.DropTable(
                name: "height");

            migrationBuilder.DropTable(
                name: "width");
        }
    }
}
