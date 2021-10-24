using Microsoft.EntityFrameworkCore.Migrations;

namespace EncryptionTest.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:pgcrypto", ",,");

            migrationBuilder.CreateTable(
                name: "Testings",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    encrypted = table.Column<string>(nullable: true),
                    normal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Testings", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Testings");
        }
    }
}
