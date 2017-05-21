using Microsoft.Xna.Framework;

namespace XmasHell.Geometry
{
    public class Line
    {
        public Vector2 First;
        public Vector2 Second;

        public float X1 => First.X;
        public float Y1 => First.Y;
        public float X2 => Second.X;
        public float Y2 => Second.Y;

        public Line(Vector2 firstPosition, Vector2 secondPosition)
        {
            First = firstPosition;
            Second = secondPosition;
        }

        public Vector2 Direction()
        {
            return MathHelperExtension.AngleToDirection(MathHelperExtension.LineToAngle(this));
        }

        public float Distance()
        {
            return Vector2.Distance(First, Second);
        }
    }
}