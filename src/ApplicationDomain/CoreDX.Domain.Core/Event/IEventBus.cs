using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Event
{
    public interface IEventBus
    {
        void PublishEvent(IEvent @event);

        Task PublishEventAsync(IEvent @event, CancellationToken cancellationToken);
    }

    public interface IEventBus<TResult> : IEventBus
    {
        new TResult PublishEvent(IEvent @event);

        new Task<TResult> PublishEventAsync(IEvent @event, CancellationToken cancellationToken);
    }
}
