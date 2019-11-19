using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Event
{
    public interface IEventStore
    {
        TResult Save<TResult>(IEvent @event, CancellationToken cancellationToken);

        Task<TResult> SaveAsync<TResult>(IEvent @event, CancellationToken cancellationToken);
    }
}
