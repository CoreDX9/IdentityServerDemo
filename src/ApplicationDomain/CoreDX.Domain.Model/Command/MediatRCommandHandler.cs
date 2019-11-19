using CoreDX.Domain.Core.Command;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CoreDX.Domain.Model.Command
{
    public class MediatRCommandHandler : ICommandHandler, IRequestHandler<MediatRCommand>
    {
        public Task<Unit> Handle(MediatRCommand request, CancellationToken cancellationToken = default)
        {
            return (Task<Unit>)Handle((ICommand)request, cancellationToken);
        }

        public Task Handle(ICommand command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(Unit));
        }
    }

    public class MediatRCommandHandler<TResult> : ICommandHandler<TResult>, IRequestHandler<MediatRCommand<TResult>, TResult>
    {
        public Task<TResult> Handle(ICommand<TResult> command, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(default(TResult));
        }

        public Task Handle(ICommand command, CancellationToken cancellationToken = default)
        {
            return Handle((ICommand<TResult>)command, cancellationToken);
        }

        public Task<TResult> Handle(MediatRCommand<TResult> request, CancellationToken cancellationToken = default)
        {
            return Handle((ICommand<TResult>)request, cancellationToken);
        }
    }
}
