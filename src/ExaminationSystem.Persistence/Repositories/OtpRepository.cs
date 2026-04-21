using ExaminationSystem.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExaminationSystem.Persistence.Repositories
{
    public class OtpRepository : IOTPRepository
    {
        private readonly ApplicationContext _context;
        public OtpRepository(ApplicationContext context)
        {
            _context = context;
        }
        public async Task AddAsync(OtpCode record, CancellationToken ct)
        {
            await _context.OtpCodes.AddAsync(record, ct); 
            await _context.SaveChangesAsync(ct);
        }

        public async Task<OtpCode?> GetActiveByUserIdAsync(Guid userId, CancellationToken ct)
        {
              return   await _context.OtpCodes.Where(x => x.UserId == userId && !x.IsUsed)
                .OrderByDescending(x => x.ExpiresAt)
                .FirstOrDefaultAsync(ct); 
        }

        public async Task InvalidateAllForUserAsync(Guid userId, CancellationToken ct)
        {
            await _context.OtpCodes.Where(x => x.UserId == userId && !x.IsUsed)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsUsed, true) , ct);
        }

        public async Task UpdateAsync(OtpCode record, CancellationToken ct)
        {
             _context.OtpCodes.Update(record);  
            await _context.SaveChangesAsync(ct);    
            
        }
    }
}
