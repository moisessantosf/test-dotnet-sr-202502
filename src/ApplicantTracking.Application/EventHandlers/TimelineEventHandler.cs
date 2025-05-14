using System.Threading;
using System.Threading.Tasks;
using ApplicantTracking.Domain.Entities;
using ApplicantTracking.Domain.Enumerators;
using ApplicantTracking.Domain.Events;
using ApplicantTracking.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace ApplicantTracking.Application.EventHandlers
{
    public class TimelineEventHandler :
        INotificationHandler<CandidateCreatedEvent>,
        INotificationHandler<CandidateUpdatedEvent>,
        INotificationHandler<CandidateDeletedEvent>
    {

        private readonly IServiceScopeFactory _scopeFactory;

        public TimelineEventHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        private async Task CreateTimelineEntry(TimelineTypes actionType, int aggregateRootId, object? oldData, object? newData)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                var oldDataJson = oldData != null ? JsonConvert.SerializeObject(oldData, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) : null;
                var newDataJson = newData != null ? JsonConvert.SerializeObject(newData, Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }) : null;

                var timeline = new Timeline(actionType, aggregateRootId, oldDataJson, newDataJson);

                await unitOfWork.TimelineRepository.AddAsync(timeline);
                await unitOfWork.CommitAsync();
            }
        }

        public Task Handle(CandidateCreatedEvent notification, CancellationToken cancellationToken)
        {
            return CreateTimelineEntry(
                TimelineTypes.Create,
                notification.CandidateSnapshot.IdCandidate,
                null,
                notification.CandidateSnapshot
            );
        }

        public Task Handle(CandidateUpdatedEvent notification, CancellationToken cancellationToken)
        {
            return CreateTimelineEntry(
                TimelineTypes.Update,
                notification.CandidateSnapshot.IdCandidate,
                notification.OldCandidateSnapshot,
                notification.CandidateSnapshot
            );
        }

        public Task Handle(CandidateDeletedEvent notification, CancellationToken cancellationToken)
        {
            return CreateTimelineEntry(
                TimelineTypes.Delete,
                notification.CandidateSnapshot.IdCandidate,
                notification.CandidateSnapshot,
                null
            );
        }
    }
}
