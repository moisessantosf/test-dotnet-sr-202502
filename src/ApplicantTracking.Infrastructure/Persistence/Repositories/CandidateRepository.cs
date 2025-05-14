using System.Threading.Tasks;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Interfaces.Repositories;
using ApplicantTracking.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace ApplicantTracking.Infrastructure.Persistence.Repositories
{
    public class CandidateRepository : GenericRepository<Candidate>, ICandidateRepository
    {
        public CandidateRepository(AppDbContext context) : base(context) { }

        public async Task<Candidate?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Email == email.ToLowerInvariant());
        }
    }
}
