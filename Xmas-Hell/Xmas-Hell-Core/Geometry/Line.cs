using Microsoft.Xna.Framework;
using XmasHell.Extensions;

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

        public float Angle()
        {
            return MathExtension.LineToAngle(this);
        }

        public Vector2 Direction()
        {
            return Vector2.Normalize(MathExtension.AngleToDirection(MathExtension.LineToAngle(this)));
        }

        public float Distance()
        {
            return Vector2.Distance(First, Second);
        }

        public float Normal()
        {
            return MathExtension.LineToAngle(this) + MathHelper.Pi;
        }
    }
}