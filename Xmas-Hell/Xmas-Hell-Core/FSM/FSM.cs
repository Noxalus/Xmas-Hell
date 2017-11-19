using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace XmasHell.FSM
{
    public class FSM<T>
    {
        private string _name;
        private readonly Dictionary<T, FSMBehaviour<T>> _behaviours = new Dictionary<T, FSMBehaviour<T>>();
        private T _currentState;
        private FSMBehaviour<T> _currentBehaviour;

        public FSM(string name)
        {
            _name = name;
        }

        public T CurrentState
        {
            get => _currentState;
            set
            {
                _currentBehaviour?.TriggerLeave();

                _currentState = value;
                _currentBehaviour = _behaviours[value];

                _currentBehaviour?.TriggerEnter();
            }
        }

        public FSMBehaviour<T> Add(T state)
        {
            var behaviour = new FSMBehaviour<T>(state);

            _behaviours.Add(state, behaviour);
            return behaviour;
        }

        public void Update(GameTime gameTime)
        {
            if (_currentBehaviour == null)
                return;

            var data = new FSMStateData<T>()
            {
                Machine = this,
                Behaviour = _currentBehaviour,
                State = _currentState,
                GameTime = gameTime
            };

            _currentBehaviour.Update(data);
        }
    }
}
