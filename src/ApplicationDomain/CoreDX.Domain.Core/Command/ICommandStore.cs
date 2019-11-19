using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Command
{
    public interface ICommandStore
    {
        TResult Save<TResult>(ICommand command);

        Task<TResult> SaveAsync<TResult>(ICommand command, CancellationToken cancellationToken);
    }
}
