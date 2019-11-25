using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Command
{
    public interface ICommandStore
    {
        void Save(ICommand command);

        Task SaveAsync(ICommand command, CancellationToken cancellationToken);
    }

    public interface ICommandStore<TResult> : ICommandStore
    {
        new TResult Save(ICommand command);

        new Task<TResult> SaveAsync(ICommand command, CancellationToken cancellationToken);
    }
}
