using System.ComponentModel.DataAnnotations;

namespace ExaminationSystem.Domain.Interfaces;

public interface IBaseEntity
{
    DateTime CreatedAt { get; set; }
    string CreatedBy { get; set; }
    DateTime? UpdatedAt { get; set; }
    string? UpdatedBy { get; set; }
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
    [Timestamp]
    byte[] RowVersion { get; set; }
}
