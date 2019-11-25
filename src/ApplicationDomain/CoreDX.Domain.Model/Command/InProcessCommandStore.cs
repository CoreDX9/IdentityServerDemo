using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Command;

namespace CoreDX.Domain.Model.Command
{
    public class InProcessCommandStore : ICommandStore<bool>
    {
        public bool Save(ICommand command)
        {
            return SaveAsync(command).Result;
        }

        public Task<bool> SaveAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(true);
        }

        void ICommandStore.Save(ICommand command)
        {
            Save(command);
        }

        Task ICommandStore.SaveAsync(ICommand command, CancellationToken cancellationToken)
        {
            return SaveAsync(command, cancellationToken);
        }
    }
}
