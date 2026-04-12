using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class QuizAttemptConfiguration : BaseEntityConfiguration<QuizAttempt>
{
    protected override void ConfigureEntity(EntityTypeBuilder<QuizAttempt> builder)
    {
        builder.ToTable("QuizAttempts");

        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.QuizId).IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();
        builder.Property(x => x.StartTime)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.Deadline)
            .HasColumnType("datetime2")
            .IsRequired();
        builder.Property(x => x.SubmittedAt)
            .HasColumnType("datetime2");

        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.QuizId);

        builder.HasOne(x => x.User)
            .WithMany(x => x.QuizAttempts)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.QuizAttempts)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Result)
            .WithOne(x => x.Attempt)
            .HasForeignKey<AttemptResult>(x => x.AttemptId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
