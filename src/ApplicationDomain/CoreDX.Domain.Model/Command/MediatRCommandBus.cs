using System;
using System.Threading;
using System.Threading.Tasks;
using CoreDX.Domain.Core.Command;
using MediatR;

namespace CoreDX.Domain.Model.Command
{
    public class MediatRCommandBus : ICommandBus
    {
        private readonly IMediator mediator;
        private readonly ICommandStore commandStore;

        public MediatRCommandBus(IMediator mediator, ICommandStore commandStore)
        {
            this.mediator = mediator;
            this.commandStore = commandStore;
        }

        public Task SendCommandAsync(ICommand command, CancellationToken cancellationToken = default)
        {
            var _command = command as MediatRCommand ?? throw new Exception($"{nameof(command)}没有继承自MediatRCommand。");
            return SendCommandAsync(_command, cancellationToken);
        }

        public Task<T> SendCommandAsync<T>(ICommand<T> command, CancellationToken cancellationToken = default)
        {
            var _command = command as MediatRCommand<T> ?? throw new Exception($"{nameof(command)}没有继承自MediatRCommand<T>。");
            commandStore?.SaveAsync<byte>(command, cancellationToken);
            return mediator.Send(_command, cancellationToken);
        }
    }
}
