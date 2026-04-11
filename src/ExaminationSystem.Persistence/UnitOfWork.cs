using ExaminationSystem.Application.Interfaces;

namespace ExaminationSystem.Persistence;

public class UnitOfWork(ApplicationContext context) : IUnitOfWork
{
    private readonly ApplicationContext _context = context;

    public async Task<int> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }
}
