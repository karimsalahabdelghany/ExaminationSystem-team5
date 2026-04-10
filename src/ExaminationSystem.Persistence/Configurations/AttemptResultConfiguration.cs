using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class AttemptResultConfiguration : IEntityTypeConfiguration<AttemptResult>
{
    public void Configure(EntityTypeBuilder<AttemptResult> builder)
    {
        builder.ToTable("AttemptResults");

        builder.HasKey(x => x.AttemptId);

        builder.Property(x => x.AttemptId).IsRequired();
        builder.Property(x => x.Score).IsRequired();
        builder.Property(x => x.TotalQuestions).IsRequired();
        builder.Property(x => x.CorrectAnswers).IsRequired();
        builder.Property(x => x.Percentage)
            .HasColumnType("real")
            .IsRequired();

        builder.HasQueryFilter(x => !x.Attempt.IsDeleted);
    }
}
