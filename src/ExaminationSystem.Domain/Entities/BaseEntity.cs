using ExaminationSystem.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ExaminationSystem.Domain.Entities;

public abstract class BaseEntity :IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; } = "system";
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

}

