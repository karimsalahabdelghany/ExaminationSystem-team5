using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class QuizConfiguration : BaseEntityConfiguration<Quiz>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Quiz> builder)
    {
        builder.ToTable("Quizzes");

        builder.Property(x => x.DiplomaId).IsRequired();
        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Duration).IsRequired();
        builder.Property(x => x.MaxAttempts).IsRequired();

        builder.HasIndex(x => x.DiplomaId);

        builder.HasOne(x => x.Diploma)
            .WithMany(x => x.Quizzes)
            .HasForeignKey(x => x.DiplomaId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
