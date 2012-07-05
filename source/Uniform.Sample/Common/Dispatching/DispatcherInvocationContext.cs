using System;

namespace Uniform.Common.Dispatching
{
    public class DispatcherInvocationContext
    {
        private readonly Dispatching.Dispatcher _dispatcher;
        private readonly object _handler;
        private readonly object _message;

        public object Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherInvocationContext()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public DispatcherInvocationContext(Dispatching.Dispatcher dispatcher, Object handler, Object message)
        {
            _dispatcher = dispatcher;
            _handler = handler;
            _message = message;
        }

        public virtual void Invoke()
        {
            _dispatcher.InvokeByReflection(_handler, _message);
        }
    }
}