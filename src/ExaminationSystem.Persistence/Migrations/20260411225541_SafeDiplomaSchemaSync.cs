using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExaminationSystem.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SafeDiplomaSchemaSync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Diplomas]', N'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH(N'[Diplomas]', N'Name') IS NOT NULL AND COL_LENGTH(N'[Diplomas]', N'Title') IS NULL
                    BEGIN
                        EXEC sp_rename N'[Diplomas].[Name]', N'Title', 'COLUMN';
                    END;

                    IF COL_LENGTH(N'[Diplomas]', N'Description') IS NOT NULL
                    BEGIN
                        ALTER TABLE [Diplomas] ALTER COLUMN [Description] nvarchar(1000) NULL;
                    END;

                    IF COL_LENGTH(N'[Diplomas]', N'QuizCount') IS NULL
                    BEGIN
                        ALTER TABLE [Diplomas] ADD [QuizCount] int NOT NULL CONSTRAINT [DF_Diplomas_QuizCount] DEFAULT 0;
                    END;

                    IF COL_LENGTH(N'[Diplomas]', N'Status') IS NULL
                    BEGIN
                        ALTER TABLE [Diplomas] ADD [Status] int NOT NULL CONSTRAINT [DF_Diplomas_Status] DEFAULT 0;
                    END;

                    IF COL_LENGTH(N'[Diplomas]', N'QuizCount') IS NOT NULL AND COL_LENGTH(N'[Diplomas]', N'Status') IS NOT NULL
                    BEGIN
                        EXEC(N'
                            UPDATE [Diplomas]
                            SET [QuizCount] = 1, [Status] = 0
                            WHERE [Id] IN (''2d21ae7d-d8a0-4f19-9509-f39b5b339a7f'', ''8480d832-e7da-4f56-9a58-91d90a51e683'');
                        ');
                    END;
                END;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                IF OBJECT_ID(N'[Diplomas]', N'U') IS NOT NULL
                BEGIN
                    IF COL_LENGTH(N'[Diplomas]', N'QuizCount') IS NOT NULL
                    BEGIN
                        DECLARE @dfQuizCount nvarchar(128);
                        SELECT @dfQuizCount = dc.name
                        FROM sys.default_constraints dc
                        INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                        INNER JOIN sys.tables t ON t.object_id = c.object_id
                        WHERE t.name = N'Diplomas' AND c.name = N'QuizCount';

                        IF @dfQuizCount IS NOT NULL
                            EXEC(N'ALTER TABLE [Diplomas] DROP CONSTRAINT [' + @dfQuizCount + ']');

                        ALTER TABLE [Diplomas] DROP COLUMN [QuizCount];
                    END;

                    IF COL_LENGTH(N'[Diplomas]', N'Status') IS NOT NULL
                    BEGIN
                        DECLARE @dfStatus nvarchar(128);
                        SELECT @dfStatus = dc.name
                        FROM sys.default_constraints dc
                        INNER JOIN sys.columns c ON c.default_object_id = dc.object_id
                        INNER JOIN sys.tables t ON t.object_id = c.object_id
                        WHERE t.name = N'Diplomas' AND c.name = N'Status';

                        IF @dfStatus IS NOT NULL
                            EXEC(N'ALTER TABLE [Diplomas] DROP CONSTRAINT [' + @dfStatus + ']');

                        ALTER TABLE [Diplomas] DROP COLUMN [Status];
                    END;

                    IF COL_LENGTH(N'[Diplomas]', N'Title') IS NOT NULL AND COL_LENGTH(N'[Diplomas]', N'Name') IS NULL
                    BEGIN
                        EXEC sp_rename N'[Diplomas].[Title]', N'Name', 'COLUMN';
                    END;

                    IF COL_LENGTH(N'[Diplomas]', N'Description') IS NOT NULL
                    BEGIN
                        UPDATE [Diplomas] SET [Description] = N'' WHERE [Description] IS NULL;
                        ALTER TABLE [Diplomas] ALTER COLUMN [Description] nvarchar(2000) NOT NULL;
                    END;
                END;
                """);
        }
    }
}
