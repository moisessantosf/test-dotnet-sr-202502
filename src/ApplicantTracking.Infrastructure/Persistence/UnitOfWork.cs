using System;
using System.Threading.Tasks;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Domain.Interfaces.Repositories;
using ApplicantTracking.Infrastructure.Context;
using ApplicantTracking.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace ApplicantTracking.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        private ICandidateRepository? _candidateRepository;
        private ITimelineRepository? _timelineRepository;

        private bool _disposed = false;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public ICandidateRepository CandidateRepository => _candidateRepository ??= new CandidateRepository(_context);
        public ITimelineRepository TimelineRepository => _timelineRepository ??= new TimelineRepository(_context);

        public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            _transaction = await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _context.SaveChangesAsync();
                await (_transaction?.CommitAsync() ?? Task.CompletedTask);
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await (_transaction?.RollbackAsync() ?? Task.CompletedTask);
            }
            finally
            {
                if (_transaction != null)
                {
                    _transaction.Dispose();
                    _transaction = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }

            _disposed = true;
        }

    }
}
