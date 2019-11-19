using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Event;

namespace CoreDX.Domain.Model.Event
{
    public class InProcessEventStore : IEventStore
    {
        public TResult Save<TResult>(IEvent @event, CancellationToken cancellationToken)
        {
            return SaveAsync<TResult>(@event, cancellationToken).Result;
        }

        public Task<TResult> SaveAsync<TResult>(IEvent @event, CancellationToken cancellationToken)
        {
            return Task.FromResult(default(TResult));
        }
    }
}
