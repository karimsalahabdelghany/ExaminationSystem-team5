using ExaminationSystem.Domain.Interfaces;

namespace ExaminationSystem.Domain.Entities;

public abstract class BaseEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get ; set ; }
    public Guid CreatedBy { get ; set ; }
    public DateTime? UpdatedAt { get; set; }
    public Guid? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Guid? DeletedBy { get; set; }
}
