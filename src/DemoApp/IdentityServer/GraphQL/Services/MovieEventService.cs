using IdentityServer.GraphQL.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace IdentityServer.GraphQL.Services
{
    public interface IMovieEventService
    {
        ConcurrentStack<MovieEvent> AllEvents { get; }
        void AddError(Exception ex);
        MovieEvent AddEvent(MovieEvent e);
        IObservable<MovieEvent> EventStream();
    }

    public class MovieEventService : IMovieEventService
    {
        private readonly ISubject<MovieEvent> _eventStream = new ReplaySubject<MovieEvent>();
        public ConcurrentStack<MovieEvent> AllEvents { get; } = new ConcurrentStack<MovieEvent>();

        public void AddError(Exception ex)
        {
            _eventStream.OnError(ex);
        }

        public MovieEvent AddEvent(MovieEvent e)
        {
            AllEvents.Push(e);
            _eventStream.OnNext(e);
            return e;
        }

        public IObservable<MovieEvent> EventStream()
        {
            return _eventStream.AsObservable();
        }
    }
}
