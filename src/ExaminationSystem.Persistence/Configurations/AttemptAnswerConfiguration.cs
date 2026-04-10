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

        builder.HasIndex(x => x.AttemptId);
        builder.HasIndex(x => x.QuestionId);

        builder.HasOne(x => x.Attempt)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.AttemptId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Question)
            .WithMany(x => x.AttemptAnswers)
            .HasForeignKey(x => x.QuestionId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
