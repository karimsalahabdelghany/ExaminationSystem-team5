using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class QuestionOptionConfiguration : BaseEntityConfiguration<QuestionOption>
{
    protected override void ConfigureEntity(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");

        builder.Property(x => x.QuestionId).IsRequired();
        builder.Property(x => x.Text)
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(x => x.IsCorrect).IsRequired();
        builder.Property(x => x.OrderIndex).IsRequired();

        builder.HasIndex(x => x.QuestionId);

        builder.HasOne(x => x.Question)
            .WithMany(x => x.Options)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Seed the specific QuestionOptions referenced by AttemptAnswers seed to satisfy FK constraints.
        builder.HasData(
            new
            {
                Id = Guid.Parse("A9B0C1D2-E3F4-5678-ABC9-789012345678"),
                QuestionId = Guid.Parse("E1F2A3B4-C5D6-7890-EFA1-901234567890"),
                Text = "PUT",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("D2E3F4A5-B6C7-8901-DEF2-012345678902"),
                QuestionId = Guid.Parse("F2A3B4C5-D6E7-8901-FAB2-012345678901"),
                Text = "Dependency Inversion",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("A5B6C7D8-E9F0-1234-ABC5-345678901235"),
                QuestionId = Guid.Parse("A3B4C5D6-E7F8-9012-ABC3-123456789012"),
                Text = "Statelessness",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            }
        );
    }
}