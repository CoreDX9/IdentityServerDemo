using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Event
{
    public interface IEventHandler
    {
        Task Handle(IEvent @event, CancellationToken cancellationToken);
    }
}
