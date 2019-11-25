using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Command
{
    public interface ICommandBus<in TCommand>
        where TCommand : ICommand
    {
        Task SendCommandAsync(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandBus<in TCommand, TResult> : ICommandBus<TCommand>
        where TCommand : ICommand<TResult>
    {
        new Task<TResult> SendCommandAsync(TCommand command, CancellationToken cancellationToken);
    }
}
