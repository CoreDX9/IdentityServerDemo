using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Command;

namespace CoreDX.Domain.Model.Command
{
    public class InProcessCommandStore : ICommandStore
    {
        public TResult Save<TResult>(ICommand command)
        {
            return SaveAsync<TResult>(command).Result;
        }

        public Task<TResult> SaveAsync<TResult>(ICommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(TResult));
        }
    }
}
