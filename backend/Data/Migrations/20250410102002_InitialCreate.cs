using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace main.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "height",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    height = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_height", h => h.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "width",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    width = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_width", w => w.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "category",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_category", c => c.id);
                }
            );

            migrationBuilder.CreateTable(
                name: "paintings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    height_id = table.Column<Guid>(type: "uuid", nullable: false),
                    width_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_paintings", p => p.id);

                    table.ForeignKey(
                        name: "FK_paintings_height_height_id",
                        column: x => x.height_id,
                        principalTable: "height",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );

                    table.ForeignKey(
                        name: "FK_paintings_width_width_id",
                        column: x => x.width_id,
                        principalTable: "width",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );

                    table.ForeignKey(
                        name: "FK_paintings_category_shelter_id",
                        column: x => x.category_id,
                        principalTable: "category",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_paintings_height_id",
                table: "paintings",
                column: "height_id");

            migrationBuilder.CreateIndex(
                name: "IX_paintings_width_id",
                table: "paintings",
                column: "width_id");

            migrationBuilder.CreateIndex(
                name: "IX_paintings_category_id",
                table: "paintings",
                column: "category_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Breeds_breed_id",
                table: "paintings");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Genders_gender_id",
                table: "paintings");

            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Shelters_shelter_id",
                table: "paintings");

            migrationBuilder.DropIndex(
                name: "IX_Shelters_shelter_owner_id",
                table: "paintings");

            migrationBuilder.DropIndex(
                name: "IX_Pets_breed_id",
                table: "paintings");

            migrationBuilder.DropIndex(
                name: "IX_Pets_gender_id",
                table: "paintings");

            migrationBuilder.DropTable(
                name: "paintings");

            migrationBuilder.DropTable(
                name: "height");

            migrationBuilder.DropTable(
                name: "width");

            migrationBuilder.DropTable(
                name: "category");
        }
    }
}
