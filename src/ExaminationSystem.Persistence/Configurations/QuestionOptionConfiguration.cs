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

        var createdAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        builder.HasData(
            // ===== Quiz 1 - Question 1: HTTP method =====
            new
            {
                Id = Guid.Parse("A9B0C1D2-E3F4-5678-ABC9-789012345678"),
                QuestionId = Guid.Parse("E1F2A3B4-C5D6-7890-EFA1-901234567890"),
                Text = "PUT",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("A9B0C1D2-E3F4-5678-ABC9-789012345679"),
                QuestionId = Guid.Parse("E1F2A3B4-C5D6-7890-EFA1-901234567890"),
                Text = "POST",
                IsCorrect = false,
                OrderIndex = 2,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("A9B0C1D2-E3F4-5678-ABC9-78901234567A"),
                QuestionId = Guid.Parse("E1F2A3B4-C5D6-7890-EFA1-901234567890"),
                Text = "PATCH",
                IsCorrect = false,
                OrderIndex = 3,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("A9B0C1D2-E3F4-5678-ABC9-78901234567B"),
                QuestionId = Guid.Parse("E1F2A3B4-C5D6-7890-EFA1-901234567890"),
                Text = "DELETE",
                IsCorrect = false,
                OrderIndex = 4,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },

            // ===== Quiz 1 - Question 2: SOLID 'D' =====
            new
            {
                Id = Guid.Parse("D2E3F4A5-B6C7-8901-DEF2-012345678902"),
                QuestionId = Guid.Parse("F2A3B4C5-D6E7-8901-FAB2-012345678901"),
                Text = "Dependency Inversion",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("D2E3F4A5-B6C7-8901-DEF2-012345678903"),
                QuestionId = Guid.Parse("F2A3B4C5-D6E7-8901-FAB2-012345678901"),
                Text = "Data Driven",
                IsCorrect = false,
                OrderIndex = 2,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("D2E3F4A5-B6C7-8901-DEF2-012345678904"),
                QuestionId = Guid.Parse("F2A3B4C5-D6E7-8901-FAB2-012345678901"),
                Text = "Domain Driven",
                IsCorrect = false,
                OrderIndex = 3,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("D2E3F4A5-B6C7-8901-DEF2-012345678905"),
                QuestionId = Guid.Parse("F2A3B4C5-D6E7-8901-FAB2-012345678901"),
                Text = "Decorator Pattern",
                IsCorrect = false,
                OrderIndex = 4,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },

            // ===== Quiz 1 - Question 3: REST constraints =====
            new
            {
                Id = Guid.Parse("A5B6C7D8-E9F0-1234-ABC5-345678901235"),
                QuestionId = Guid.Parse("A3B4C5D6-E7F8-9012-ABC3-123456789012"),
                Text = "Statelessness",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("A5B6C7D8-E9F0-1234-ABC5-345678901236"),
                QuestionId = Guid.Parse("A3B4C5D6-E7F8-9012-ABC3-123456789012"),
                Text = "Persistent sessions",
                IsCorrect = false,
                OrderIndex = 2,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("A5B6C7D8-E9F0-1234-ABC5-345678901237"),
                QuestionId = Guid.Parse("A3B4C5D6-E7F8-9012-ABC3-123456789012"),
                Text = "Server-side rendering",
                IsCorrect = false,
                OrderIndex = 3,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("A5B6C7D8-E9F0-1234-ABC5-345678901238"),
                QuestionId = Guid.Parse("A3B4C5D6-E7F8-9012-ABC3-123456789012"),
                Text = "Tight coupling",
                IsCorrect = false,
                OrderIndex = 4,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },

            // ===== Quiz 2 - Question 1: Liveness probe =====
            new
            {
                Id = Guid.Parse("B4C5D6E7-F8A9-0124-BCD4-234567890124"),
                QuestionId = Guid.Parse("B4C5D6E7-F8A9-0123-BCD4-234567890123"),
                Text = "Tells Kubernetes whether to restart a container",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("B4C5D6E7-F8A9-0124-BCD4-234567890125"),
                QuestionId = Guid.Parse("B4C5D6E7-F8A9-0123-BCD4-234567890123"),
                Text = "Measures CPU usage",
                IsCorrect = false,
                OrderIndex = 2,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("B4C5D6E7-F8A9-0124-BCD4-234567890126"),
                QuestionId = Guid.Parse("B4C5D6E7-F8A9-0123-BCD4-234567890123"),
                Text = "Configures ingress routing",
                IsCorrect = false,
                OrderIndex = 3,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("B4C5D6E7-F8A9-0124-BCD4-234567890127"),
                QuestionId = Guid.Parse("B4C5D6E7-F8A9-0123-BCD4-234567890123"),
                Text = "Authenticates users",
                IsCorrect = false,
                OrderIndex = 4,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },

            // ===== Quiz 2 - Question 2: IaC tool (HCL) =====
            new
            {
                Id = Guid.Parse("C5D6E7F8-A9B0-1235-CDE5-345678901235"),
                QuestionId = Guid.Parse("C5D6E7F8-A9B0-1234-CDE5-345678901234"),
                Text = "Terraform",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("C5D6E7F8-A9B0-1235-CDE5-345678901236"),
                QuestionId = Guid.Parse("C5D6E7F8-A9B0-1234-CDE5-345678901234"),
                Text = "Ansible",
                IsCorrect = false,
                OrderIndex = 2,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("C5D6E7F8-A9B0-1235-CDE5-345678901237"),
                QuestionId = Guid.Parse("C5D6E7F8-A9B0-1234-CDE5-345678901234"),
                Text = "CloudFormation",
                IsCorrect = false,
                OrderIndex = 3,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("C5D6E7F8-A9B0-1235-CDE5-345678901238"),
                QuestionId = Guid.Parse("C5D6E7F8-A9B0-1234-CDE5-345678901234"),
                Text = "Puppet",
                IsCorrect = false,
                OrderIndex = 4,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },

            // ===== Quiz 2 - Question 3: Container orchestration =====
            new
            {
                Id = Guid.Parse("D6E7F8A9-B0C1-2346-DEF6-456789012346"),
                QuestionId = Guid.Parse("D6E7F8A9-B0C1-2345-DEF6-456789012345"),
                Text = "Kubernetes",
                IsCorrect = true,
                OrderIndex = 1,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("D6E7F8A9-B0C1-2346-DEF6-456789012347"),
                QuestionId = Guid.Parse("D6E7F8A9-B0C1-2345-DEF6-456789012345"),
                Text = "Git",
                IsCorrect = false,
                OrderIndex = 2,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("D6E7F8A9-B0C1-2346-DEF6-456789012348"),
                QuestionId = Guid.Parse("D6E7F8A9-B0C1-2345-DEF6-456789012345"),
                Text = "Jenkins",
                IsCorrect = false,
                OrderIndex = 3,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            },
            new
            {
                Id = Guid.Parse("D6E7F8A9-B0C1-2346-DEF6-456789012349"),
                QuestionId = Guid.Parse("D6E7F8A9-B0C1-2345-DEF6-456789012345"),
                Text = "Nginx",
                IsCorrect = false,
                OrderIndex = 4,
                CreatedAt = createdAt,
                CreatedBy = "seed",
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (string?)null,
                IsDeleted = false,
                DeletedAt = (DateTime?)null
            }
        );
    }
}
