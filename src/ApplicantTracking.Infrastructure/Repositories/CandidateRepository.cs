using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Interfaces;
using ApplicantTracking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ApplicantTracking.Infrastructure.Repositories
{
    public class CandidateRepository : ICandidateRepository
    {
        private readonly ApplicantTrackingContext _context;

        public CandidateRepository(ApplicantTrackingContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Candidate>> GetAllAsync()
        {
            return await _context.Candidates.ToListAsync();
        }

        public async Task<Candidate> GetByIdAsync(int id)
        {
            return await _context.Candidates.FindAsync(id);
        }

        public async Task AddAsync(Candidate entity)
        {
            await _context.Candidates.AddAsync(entity);
        }

        public void Update(Candidate entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(Candidate entity)
        {
            _context.Candidates.Remove(entity);
        }
    }
}