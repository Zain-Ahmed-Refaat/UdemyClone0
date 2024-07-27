using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UdemyClone.Migrations
{
    /// <inheritdoc />
    public partial class up : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Questions_CorrectAnswerId",
                table: "Questions",
                column: "CorrectAnswerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Answers_CorrectAnswerId",
                table: "Questions",
                column: "CorrectAnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Answers_CorrectAnswerId",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_CorrectAnswerId",
                table: "Questions");
        }
    }
}
