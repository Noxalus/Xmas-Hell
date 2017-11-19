using Microsoft.Xna.Framework;

namespace XmasHell.FSM
{
    public struct FSMStateData<T>
    {
		public FSM<T> Machine { get; internal set; }
		public FSMBehaviour<T> Behaviour { get; internal set; }
        public T State { get; internal set; }
        public GameTime GameTime { get; internal set; }
    }
}
