using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Command
{
    public interface ICommandHandler
    {
        Task Handle(ICommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<TResult> : ICommandHandler
    {
        Task<TResult> Handle(ICommand<TResult> command, CancellationToken cancellationToken);
    }
}
