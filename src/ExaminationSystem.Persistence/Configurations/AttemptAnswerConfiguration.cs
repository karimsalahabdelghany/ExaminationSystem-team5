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

        // NOTE: Removed static seed for AttemptAnswers to avoid FK insertion order issues.
        // Seed AttemptAnswers only after QuestionOptions exist (via a new migration),
        // or insert them at runtime (test seed script) to guarantee FK referential integrity.
    }
}
