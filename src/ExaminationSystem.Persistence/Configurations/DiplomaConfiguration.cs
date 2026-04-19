using ExaminationSystem.Domain.Enums;

namespace ExaminationSystem.Persistence.Configurations;

public sealed class DiplomaConfiguration : BaseEntityConfiguration<Diploma>
{
    protected override void ConfigureEntity(EntityTypeBuilder<Diploma> builder)
    {
        builder.ToTable("Diplomas");

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(x => x.Description)
            .HasMaxLength(1000);
        

        builder.HasData(
            new
            {
                Id = Guid.Parse("2D21AE7D-D8A0-4F19-9509-F39B5B339A7F"),
                Title = "Backend Engineering Diploma",
                Description = "A foundational backend program covering architecture, APIs, and persistence.",
                Duration = 24,
                QuizCount = 1,
                Status = DiplomaStatus.Published,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("8480D832-E7DA-4F56-9A58-91D90A51E683"),
                Title = "Cloud & DevOps Diploma",
                Description = "A practical cloud engineering track with CI/CD, IaC, and monitoring.",
                Duration = 20,
                QuizCount = 1,
                Status = DiplomaStatus.Draft,
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            });
    }
}
