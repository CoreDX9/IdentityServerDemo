using CoreDX.Domain.Core.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Model.Command
{
    public abstract class MediatRCommandHandler<TCommand, TResult> : ICommandHandler<TCommand, TResult>, IRequestHandler<TCommand, TResult>
    where TCommand : MediatRCommand<TResult>
    {
        public abstract Task<TResult> Handle(TCommand command, CancellationToken cancellationToken = default);

        Task ICommandHandler<TCommand>.Handle(TCommand command, CancellationToken cancellationToken)
        {
            return Handle(command, cancellationToken);
        }
    }

    public abstract class MediatRCommandHandler<TCommand> : MediatRCommandHandler<TCommand, Unit>
        where TCommand : MediatRCommand
    {
    }
}
