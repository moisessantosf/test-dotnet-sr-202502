using System.Threading.Tasks;

namespace ApplicantTracking.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();
    }
}