using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Application.Interfaces
{
    public  interface IOTPRepository
    {
        Task<OtpCode?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct);
        Task AddAsync(OtpCode record, CancellationToken ct);
        Task UpdateAsync(OtpCode record, CancellationToken ct);
        Task InvalidateAllForUserAsync(Guid userId, CancellationToken ct);
    }
}
