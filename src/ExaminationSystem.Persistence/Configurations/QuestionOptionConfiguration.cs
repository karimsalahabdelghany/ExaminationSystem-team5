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
    }
}
