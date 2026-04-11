using ExaminationSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class QuestionConfiguration : BaseEntityConfiguration<Question>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.Property(x => x.QuizId).IsRequired();
        builder.Property(x => x.Text)
            .HasMaxLength(2000)
            .IsRequired();
        builder.Property(x => x.Type)
            .HasConversion<byte>()
            .HasColumnType("tinyint")
            .IsRequired();
        builder.Property(x => x.Explanation)
            .HasMaxLength(4000);
        builder.Property(x => x.OrderIndex).IsRequired();

        builder.HasIndex(x => x.QuizId);

        builder.HasOne(x => x.Quiz)
            .WithMany(x => x.Questions)
            .HasForeignKey(x => x.QuizId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
