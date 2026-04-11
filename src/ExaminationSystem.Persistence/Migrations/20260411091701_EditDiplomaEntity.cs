using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExaminationSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EditDiplomaEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Diplomas",
                newName: "Title");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Diplomas",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<int>(
                name: "QuizCount",
                table: "Diplomas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Diplomas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Diplomas",
                keyColumn: "Id",
                keyValue: new Guid("2d21ae7d-d8a0-4f19-9509-f39b5b339a7f"),
                columns: new[] { "QuizCount", "Status" },
                values: new object[] { 1, 0 });

            migrationBuilder.UpdateData(
                table: "Diplomas",
                keyColumn: "Id",
                keyValue: new Guid("8480d832-e7da-4f56-9a58-91d90a51e683"),
                columns: new[] { "QuizCount", "Status" },
                values: new object[] { 1, 0 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuizCount",
                table: "Diplomas");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Diplomas");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Diplomas",
                newName: "Name");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Diplomas",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);
        }
    }
}
