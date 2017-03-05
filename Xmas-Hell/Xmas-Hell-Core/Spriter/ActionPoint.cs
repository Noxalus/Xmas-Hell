using Microsoft.Xna.Framework;

namespace XmasHell.Spriter
{
    public class ActionPoint
    {
        public Vector2 Position;
        public float Direction;

        public ActionPoint()
        {
            Position = Vector2.Zero;
            Direction = 0f;
        }

        public ActionPoint(Vector2 position, float direction)
        {
            Position = position;
            Direction = direction;
        }
    }
}
