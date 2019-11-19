using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Command
{
    public interface ICommandBus
    {
        Task SendCommandAsync(ICommand command, CancellationToken cancellationToken);

        Task<TResult> SendCommandAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken);
    }
}
