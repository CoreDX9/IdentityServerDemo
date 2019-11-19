using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDX.Domain.Core.Message
{
    public interface IMessage
    {
        Guid Id { get; }

        DateTimeOffset Timestamp { get; }
    }
}
