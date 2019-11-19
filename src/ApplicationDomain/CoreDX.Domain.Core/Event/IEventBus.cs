using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Event
{
    public interface IEventBus
    {
        TResult PublishEvent<TResult>(IEvent @event, CancellationToken cancellationToken);

        Task<TResult> PublishEventAsync<TResult>(IEvent @event, CancellationToken cancellationToken);
    }
}
