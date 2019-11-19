using System;
using CoreDX.Domain.Core.Event;
using MediatR;

namespace CoreDX.Domain.Model.Event
{
    public abstract class MediatREvent : IEvent, INotification
    {
        public Guid Id { get; }

        public DateTimeOffset Timestamp { get; }

        public MediatREvent()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTimeOffset.Now;
        }
    }
}
