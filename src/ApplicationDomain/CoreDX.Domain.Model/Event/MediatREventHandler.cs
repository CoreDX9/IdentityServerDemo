using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Event;

namespace CoreDX.Domain.Model.Event
{
    public class MediatREventHandler<TEvent> : IEventHandler<TEvent>, INotificationHandler<TEvent>
        where TEvent : MediatREvent
    {
        public Task Handle(TEvent @event, CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task Handle(MediatREvent notification, CancellationToken cancellationToken)
        {
            return Handle((TEvent)notification, cancellationToken);
        }
    }
}
