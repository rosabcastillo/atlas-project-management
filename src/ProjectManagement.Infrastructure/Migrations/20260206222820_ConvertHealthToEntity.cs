using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjectManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ConvertHealthToEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Health",
                table: "Projects",
                newName: "HealthId");

            migrationBuilder.CreateTable(
                name: "ProjectHealths",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayOrder = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectHealths", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_HealthId",
                table: "Projects",
                column: "HealthId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectHealths_Name",
                table: "ProjectHealths",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectHealths_HealthId",
                table: "Projects",
                column: "HealthId",
                principalTable: "ProjectHealths",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectHealths_HealthId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "ProjectHealths");

            migrationBuilder.DropIndex(
                name: "IX_Projects_HealthId",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "HealthId",
                table: "Projects",
                newName: "Health");
        }
    }
}
