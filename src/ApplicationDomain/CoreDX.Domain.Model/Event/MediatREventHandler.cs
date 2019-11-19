using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Event;

namespace CoreDX.Domain.Model.Event
{
    public class MediatREventHandler : IEventHandler, INotificationHandler<MediatREvent>
    {
        public Task Handle(MediatREvent notification, CancellationToken cancellationToken = default)
        {
            return Handle((IEvent)notification, cancellationToken);
        }

        public Task Handle(IEvent @event, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
