using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Migrations
{
    /// <inheritdoc />
    public partial class addedNewEntityCreatorAndExecutor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatorId",
                table: "MyTasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ExecutorId",
                table: "MyTasks",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_CreatorId",
                table: "MyTasks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_MyTasks_ExecutorId",
                table: "MyTasks",
                column: "ExecutorId");

            migrationBuilder.AddForeignKey(
                name: "FK_MyTasks_AspNetUsers_CreatorId",
                table: "MyTasks",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MyTasks_AspNetUsers_ExecutorId",
                table: "MyTasks",
                column: "ExecutorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyTasks_AspNetUsers_CreatorId",
                table: "MyTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_MyTasks_AspNetUsers_ExecutorId",
                table: "MyTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyTasks_CreatorId",
                table: "MyTasks");

            migrationBuilder.DropIndex(
                name: "IX_MyTasks_ExecutorId",
                table: "MyTasks");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "MyTasks");

            migrationBuilder.DropColumn(
                name: "ExecutorId",
                table: "MyTasks");
        }
    }
}
