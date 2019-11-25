using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Core.Command
{
    public interface ICommandHandler<in TCommand>
        where TCommand : ICommand
    {
        Task Handle(TCommand command, CancellationToken cancellationToken);
    }

    public interface ICommandHandler<in TCommand, TResult> : ICommandHandler<TCommand>
        where TCommand : ICommand<TResult>
    {
        new Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);
    }
}
