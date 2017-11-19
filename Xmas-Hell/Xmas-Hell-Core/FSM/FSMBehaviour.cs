using System;
using System.Collections.Generic;

namespace XmasHell.FSM
{
    public class FSMBehaviour<T>
    {
        private readonly List<Action<FSMStateData<T>>> _updateCallbacks = new List<Action<FSMStateData<T>>>();
        private readonly List<Action> _enterCallbacks = new List<Action>();
        private readonly List<Action> _exitCallbacks = new List<Action>();

        public T State { get; private set; }

        public FSMBehaviour(T state)
        {
			State = state;
        }

        public FSMBehaviour<T> OnEnter(Action callback)
        {
            _enterCallbacks.Add(callback);
            return this;
        }

        public FSMBehaviour<T> OnExit(Action callback)
        {
            _exitCallbacks.Add(callback);
            return this;
        }

        public FSMBehaviour<T> OnUpdate(Action<FSMStateData<T>> callback)
        {
            _updateCallbacks.Add(callback);
            return this;
        }

        internal void Update(FSMStateData<T> data)
        {
            foreach (var callback in _updateCallbacks)
                callback(data);
        }

        internal void TriggerEnter()
        {
            foreach (var callback in _enterCallbacks)
                callback();
        }

        internal void TriggerLeave()
        {
            foreach (var callback in _exitCallbacks)
                callback();
        }
    }
}
