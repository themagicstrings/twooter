using Microsoft.EntityFrameworkCore.Migrations;

namespace Entities.Migrations
{
    public partial class followers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_users_user_id1",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_user_id1",
                table: "users");

            migrationBuilder.DropColumn(
                name: "user_id1",
                table: "users");

            migrationBuilder.CreateTable(
                name: "followers",
                columns: table => new
                {
                    who_id = table.Column<int>(type: "int", nullable: false),
                    whom_id = table.Column<int>(type: "int", nullable: false),
                    whouser_id = table.Column<int>(type: "int", nullable: true),
                    whomuser_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_followers", x => new { x.who_id, x.whom_id });
                    table.ForeignKey(
                        name: "FK_followers_users_whomuser_id",
                        column: x => x.whomuser_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_followers_users_whouser_id",
                        column: x => x.whouser_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_followers_whomuser_id",
                table: "followers",
                column: "whomuser_id");

            migrationBuilder.CreateIndex(
                name: "IX_followers_whouser_id",
                table: "followers",
                column: "whouser_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "followers");

            migrationBuilder.AddColumn<int>(
                name: "user_id1",
                table: "users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_users_user_id1",
                table: "users",
                column: "user_id1");

            migrationBuilder.AddForeignKey(
                name: "FK_users_users_user_id1",
                table: "users",
                column: "user_id1",
                principalTable: "users",
                principalColumn: "user_id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
