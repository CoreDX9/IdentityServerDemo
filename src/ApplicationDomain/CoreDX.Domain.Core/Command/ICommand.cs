using CoreDX.Domain.Core.Message;
using System;

namespace CoreDX.Domain.Core.Command
{
    public interface ICommand<out TResult> : ICommand
    {
    }

    public interface ICommand : IMessage
    {
    }
}
