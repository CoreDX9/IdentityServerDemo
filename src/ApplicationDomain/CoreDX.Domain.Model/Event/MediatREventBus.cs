using CoreDX.Domain.Core.Event;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Model.Event
{
    public class MediatREventBus : IEventBus
    {
        protected readonly IMediator mediator;
        protected readonly IEventStore eventStore;

        public MediatREventBus(IMediator mediator, IEventStore eventStore)
        {
            this.mediator = mediator;
            this.eventStore = eventStore;
        }

        public void PublishEvent(IEvent @event)
        {
            PublishEventAsync(@event).Wait();
        }

        public Task PublishEventAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            eventStore?.SaveAsync(@event, cancellationToken);
            mediator.Publish(@event, cancellationToken);
            return Task.CompletedTask;
        }
    }

    public class MediatREventBus<TResult> : MediatREventBus, IEventBus<TResult>
    {
        public MediatREventBus(IMediator mediator, IEventStore eventStore)
            : base(mediator, eventStore)
        {
        }

        TResult IEventBus<TResult>.PublishEvent(IEvent @event)
        {
            PublishEventAsync(@event).Wait();
            return default;
        }

        Task<TResult> IEventBus<TResult>.PublishEventAsync(IEvent @event, CancellationToken cancellationToken)
        {
            eventStore?.SaveAsync(@event, cancellationToken);
            mediator.Publish(@event, cancellationToken);
            return Task.FromResult(default(TResult));
        }
    }
}
