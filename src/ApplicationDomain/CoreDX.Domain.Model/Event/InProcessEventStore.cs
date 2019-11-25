using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Event;

namespace CoreDX.Domain.Model.Event
{
    public class InProcessEventStore : IEventStore
    {
        public void Save(IEvent @event)
        {
            SaveAsync(@event).Wait();
        }

        public Task SaveAsync(IEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }

    public class InProcessEventStore<TResult> : InProcessEventStore, IEventStore<TResult>
    {
        TResult IEventStore<TResult>.Save(IEvent @event)
        {
            SaveAsync(@event);
            return default;
        }

        Task<TResult> IEventStore<TResult>.SaveAsync(IEvent @event, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(TResult));
        }
    }
}
