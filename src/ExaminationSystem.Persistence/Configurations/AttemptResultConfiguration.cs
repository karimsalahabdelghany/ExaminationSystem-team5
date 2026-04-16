using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class AttemptResultConfiguration : IEntityTypeConfiguration<AttemptResult>
{
    public void Configure(EntityTypeBuilder<AttemptResult> builder)
    {
        builder.ToTable("AttemptResults");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(x => x.AttemptId).IsRequired();
        builder.Property(x => x.Score)
            .HasColumnType("decimal(9,2)")
            .IsRequired();
        builder.Property(x => x.Passed).IsRequired();
        builder.Property(x => x.TotalQuestions).IsRequired();
        builder.Property(x => x.CorrectCount).IsRequired();
        builder.Property(x => x.CalculatedAt)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.QuestionBreakdownJson)
            .HasColumnType("nvarchar(max)")
            .IsRequired();

        builder.HasIndex(x => x.AttemptId).IsUnique();

        builder.HasQueryFilter(x => !x.Attempt.IsDeleted);

        builder.HasData(
    new
    {
        Id = Guid.Parse("E1F2A3B4-C5D6-7891-EFA1-901234567891"),
        AttemptId = Guid.Parse("F6A7B8C9-D0E1-2345-FABC-456789012345"),
        Score = 66.67m,
        Passed = true,   // PassScore for Quiz 1 is 60
        TotalQuestions = 3,
        CorrectCount = 2, // Q1 ✓  Q2 ✓  Q3 partial (only one option selected)
        CalculatedAt = new DateTime(2026, 2, 1, 10, 25, 30, DateTimeKind.Utc),
        CreatedAt = new DateTime(2026, 2, 1, 10, 25, 30, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    }
);

    }
}
