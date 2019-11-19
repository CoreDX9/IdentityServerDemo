using CoreDX.Domain.Core.Event;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Model.Event
{
    public class MediatREventBus : IEventBus
    {
        private readonly IMediator mediator;
        private readonly IEventStore eventStore;

        public MediatREventBus(IMediator mediator, IEventStore eventStore)
        {
            this.mediator = mediator;
            this.eventStore = eventStore;
        }

        public TResult PublishEvent<TResult>(IEvent @event, CancellationToken cancellationToken)
        {
            return PublishEventAsync<TResult>(@event, cancellationToken).Result;
        }

        public Task<TResult> PublishEventAsync<TResult>(IEvent @event, CancellationToken cancellationToken)
        {
            eventStore?.SaveAsync<byte>(@event, cancellationToken);
            mediator.Publish(@event, cancellationToken);
            return Task.FromResult(default(TResult));
        }
    }
}
