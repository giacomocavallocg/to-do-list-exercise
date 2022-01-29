using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ToDoDomain.Migrations
{
    public partial class toDoDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MD_ToDoLists",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DeleteDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MD_ToDoLists", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MD_ToDoEntries",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(64)", nullable: false),
                    ToDoListId = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: true),
                    IsDone = table.Column<bool>(nullable: false),
                    ExpirationDate = table.Column<DateTime>(nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    UpdateDate = table.Column<DateTime>(nullable: true),
                    DeleteDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MD_ToDoEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MD_ToDoEntries_MD_ToDoLists_ToDoListId",
                        column: x => x.ToDoListId,
                        principalTable: "MD_ToDoLists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MD_ToDoEntries_ToDoListId",
                table: "MD_ToDoEntries",
                column: "ToDoListId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MD_ToDoEntries");

            migrationBuilder.DropTable(
                name: "MD_ToDoLists");
        }
    }
}
