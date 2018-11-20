using Microsoft.EntityFrameworkCore.Migrations;

namespace DearDiaryLogs.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiaryLog",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EventName = table.Column<string>(nullable: true),
                    StoryUrl = table.Column<string>(nullable: true),
                    StartTime = table.Column<string>(nullable: true),
                    EndTime = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiaryImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    EntryId = table.Column<int>(nullable: false),
                    ImageURL = table.Column<string>(nullable: true),
                    Width = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiaryImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiaryImage_DiaryLog_EntryId",
                        column: x => x.EntryId,
                        principalTable: "DiaryLog",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiaryImage_EntryId",
                table: "DiaryImage",
                column: "EntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiaryImage");

            migrationBuilder.DropTable(
                name: "DiaryLog");
        }
    }
}
