using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Event
{
    public interface IEventStore
    {
        void Save(IEvent @event);

        Task SaveAsync(IEvent @event, CancellationToken cancellationToken = default);
    }

    public interface IEventStore<TResult> : IEventStore
    {
        new TResult Save(IEvent @event);

        new Task<TResult> SaveAsync(IEvent @event, CancellationToken cancellationToken = default);
    }
}
