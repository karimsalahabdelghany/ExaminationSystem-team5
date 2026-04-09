using Microsoft.EntityFrameworkCore;

namespace ExaminationSystem.Persistence;

public class ApplicationContext :DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
        
    }
}