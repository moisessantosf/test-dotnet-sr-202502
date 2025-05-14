using System;
using System.Threading.Tasks;
using ApplicantTracking.Domain.Interfaces.Repositories;

namespace ApplicantTracking.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ICandidateRepository CandidateRepository { get; }
        ITimelineRepository TimelineRepository { get; }
        Task<int> CommitAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
