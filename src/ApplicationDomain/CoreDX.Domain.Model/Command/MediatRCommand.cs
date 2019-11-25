using System;
using CoreDX.Domain.Core.Command;
using MediatR;

namespace CoreDX.Domain.Model.Command
{
    public abstract class MediatRCommand : MediatRCommand<Unit>, ICommand, IRequest
    {
    }

    public abstract class MediatRCommand<TResult> : ICommand<TResult>, IRequest<TResult>
    {
        public Guid Id { get; }

        public DateTimeOffset Timestamp { get; }

        public MediatRCommand()
        {
            Id = Guid.NewGuid();
            Timestamp = DateTimeOffset.Now;
        }
    }
}
