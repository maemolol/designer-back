using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class DbChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_paintings_height_id"";");

            migrationBuilder.Sql(@"DROP INDEX IF EXISTS ""IX_paintings_width_id"";");

            migrationBuilder.Sql("""
            ALTER TABLE paintings 
            ADD COLUMN IF NOT EXISTS price real;
            """);

            migrationBuilder.Sql("""
            ALTER TABLE paintings
            ADD COLUMN IF NOT EXISTS sold boolean NOT NULL DEFAULT FALSE;
            """);

            migrationBuilder.CreateIndex(
                name: "IX_paintings_height_id",
                table: "paintings",
                column: "height_id");

            migrationBuilder.CreateIndex(
                name: "IX_paintings_width_id",
                table: "paintings",
                column: "width_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_paintings_height_id",
                table: "paintings");

            migrationBuilder.DropIndex(
                name: "IX_paintings_width_id",
                table: "paintings");

            migrationBuilder.DropColumn(
                name: "price",
                table: "paintings");

            migrationBuilder.DropColumn(
                name: "sold",
                table: "paintings");

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
    }
}
