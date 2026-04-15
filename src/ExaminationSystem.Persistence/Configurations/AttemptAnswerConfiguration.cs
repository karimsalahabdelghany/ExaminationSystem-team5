using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class AttemptAnswerConfiguration : BaseEntityConfiguration<AttemptAnswer>
{
    protected override void ConfigureEntity(EntityTypeBuilder<AttemptAnswer> builder)
    {
        builder.ToTable("AttemptAnswers");

        builder.Property(x => x.AttemptId).IsRequired();
        builder.Property(x => x.QuestionId).IsRequired();
        builder.Property(x => x.SelectedOptionId).IsRequired();
        builder.Property(x => x.AnsweredAt)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(x => x.AttemptId);
        builder.HasIndex(x => x.QuestionId);
        builder.HasIndex(x => x.SelectedOptionId);

        builder.HasOne(x => x.Attempt)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Question)
            .WithMany(x => x.AttemptAnswers)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.SelectedOption)
            .WithMany(x => x.AttemptAnswers)
            .HasForeignKey(x => x.SelectedOptionId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(
    // Q1 answered: PUT (correct)
    new
    {
        Id = Guid.Parse("B8C9D0E1-F2A3-4567-BCD8-678901234568"),
        AttemptId = Guid.Parse("F6A7B8C9-D0E1-2345-FABC-456789012345"),
        QuestionId = Guid.Parse("E1F2A3B4-C5D6-7890-EFA1-901234567890"),
        SelectedOptionId = Guid.Parse("A9B0C1D2-E3F4-5678-ABC9-789012345678"), // PUT ?
        AnsweredAt = new DateTime(2026, 2, 1, 10, 8, 0, DateTimeKind.Utc),
        CreatedAt = new DateTime(2026, 2, 1, 10, 8, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    // Q2 answered: Dependency Inversion (correct)
    new
    {
        Id = Guid.Parse("C9D0E1F2-A3B4-5678-CDE9-789012345679"),
        AttemptId = Guid.Parse("F6A7B8C9-D0E1-2345-FABC-456789012345"),
        QuestionId = Guid.Parse("F2A3B4C5-D6E7-8901-FAB2-012345678901"),
        SelectedOptionId = Guid.Parse("D2E3F4A5-B6C7-8901-DEF2-012345678902"), // Dependency Inversion ?
        AnsweredAt = new DateTime(2026, 2, 1, 10, 15, 0, DateTimeKind.Utc),
        CreatedAt = new DateTime(2026, 2, 1, 10, 15, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    },
    // Q3 answered: Statelessness (one of two correct options)
    new
    {
        Id = Guid.Parse("D0E1F2A3-B4C5-6789-DEF0-890123456780"),
        AttemptId = Guid.Parse("F6A7B8C9-D0E1-2345-FABC-456789012345"),
        QuestionId = Guid.Parse("A3B4C5D6-E7F8-9012-ABC3-123456789012"),
        SelectedOptionId = Guid.Parse("A5B6C7D8-E9F0-1234-ABC5-345678901235"), // Statelessness ?
        AnsweredAt = new DateTime(2026, 2, 1, 10, 22, 0, DateTimeKind.Utc),
        CreatedAt = new DateTime(2026, 2, 1, 10, 22, 0, DateTimeKind.Utc),
        CreatedBy = "seed",
        UpdatedAt = (DateTime?)null,
        UpdatedBy = (string?)null,
        IsDeleted = false,
        DeletedAt = (DateTime?)null
    }
);
    }
}
