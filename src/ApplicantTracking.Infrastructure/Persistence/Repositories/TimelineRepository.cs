using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Interfaces.Repositories;
using ApplicantTracking.Infrastructure.Context;

namespace ApplicantTracking.Infrastructure.Persistence.Repositories
{
    public class TimelineRepository : GenericRepository<Timeline>, ITimelineRepository
    {
        public TimelineRepository(AppDbContext context) : base(context) { }
    }
}
