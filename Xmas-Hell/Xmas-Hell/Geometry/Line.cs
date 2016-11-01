using Microsoft.Xna.Framework;

namespace Xmas_Hell.Geometry
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
    }
}