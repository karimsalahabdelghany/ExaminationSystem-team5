using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExaminationSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "UX_QuizAttempts_UserId_QuizId_InProgress",
                table: "QuizAttempts",
                columns: new[] { "UserId", "QuizId", "Status" },
                unique: true,
                filter: "[Status] = 1 AND [IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "UX_QuizAttempts_UserId_QuizId_InProgress",
                table: "QuizAttempts");
        }
    }
}
