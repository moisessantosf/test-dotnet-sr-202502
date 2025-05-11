using System.Threading.Tasks;

namespace ApplicantTracking.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicantTrackingContext _context;

        public UnitOfWork(ApplicantTrackingContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}