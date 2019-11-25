using System;
using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Command;
using MediatR;

namespace CoreDX.Domain.Model.Command
{
    public class MediatRCommandBus<TCommand, TResult> : ICommandBus<TCommand, TResult>
        where TCommand : MediatRCommand<TResult>
    {
        private readonly IMediator mediator;
        private readonly ICommandStore commandStore;

        public MediatRCommandBus(IMediator mediator, ICommandStore commandStore)
        {
            this.mediator = mediator;
            this.commandStore = commandStore;
        }

        public virtual Task<TResult> SendCommandAsync(TCommand command, CancellationToken cancellationToken = default)
        {
            commandStore?.SaveAsync(command, cancellationToken);
            return mediator.Send(command, cancellationToken);
        }

        Task ICommandBus<TCommand>.SendCommandAsync(TCommand command, CancellationToken cancellationToken)
        {
            return SendCommandAsync(command, cancellationToken);
        }
    }

    public class MediatRCommandBus<TCommand> : MediatRCommandBus<MediatRCommand<Unit>, Unit>
        where TCommand : MediatRCommand<Unit>
    {
        public MediatRCommandBus(IMediator mediator, ICommandStore commandStore) : base(mediator, commandStore)
        {
        }
    }
}
