using System.Threading.Tasks;
using ApplicantTracking.Domain.Entities;

namespace ApplicantTracking.Domain.Interfaces.Repositories
{
    public interface ICandidateRepository : IGenericRepository<Candidate>
    {
        Task<Candidate?> GetByEmailAsync(string email);
    }
}
