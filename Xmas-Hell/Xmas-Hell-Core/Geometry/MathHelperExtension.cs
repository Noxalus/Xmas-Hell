using System;
using Microsoft.Xna.Framework;

namespace XmasHell.Geometry
{
    public static class MathHelperExtension
    {
        public static Vector2 AngleToDirection(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        public static bool LinesIntersect(Line line1, Line line2, ref Vector2 intersectionPosition)
        {
            // Denominator for ua and ub are the same, so store this calculation
            double d = (line2.Y2 - line2.Y1) * (line1.X2 - line1.X1) - (line2.X2 - line2.X1) * (line1.Y2 - line1.Y1);

            //n_a and n_b are calculated as seperate values for readability
            double nA = (line2.X2 - line2.X1) * (line1.Y1 - line2.Y1) - (line2.Y2 - line2.Y1) * (line1.X1 - line2.X1);
            double nB = (line1.X2 - line1.X1) * (line1.Y1 - line2.Y1) - (line1.Y2 - line1.Y1) * (line1.X1 - line2.X1);

            // Make sure there is not a division by zero - this also indicates that
            // the lines are parallel.
            // If n_a and n_b were both equal to zero the lines would be on top of each
            // other (coincidental).  This check is not done because it is not
            // necessary for this implementation (the parallel check accounts for this).
            if (System.Math.Abs(d) < 0.001f)
                return false;

            // Calculate the intermediate fractional point that the lines potentially intersect.
            var ua = nA / d;
            var ub = nB / d;

            // The fractional point will be between 0 and 1 inclusive if the lines
            // intersect.  If the fractional calculation is larger than 1 or smaller
            // than 0 the lines would need to be longer to intersect.
            if (ua >= 0d && ua <= 1d && ub >= 0d && ub <= 1d)
            {
                intersectionPosition.X = (float)(line1.X1 + (ua * (line1.X2 - line1.X1)));
                intersectionPosition.Y = (float)(line1.Y1 + (ua * (line1.Y2 - line1.Y1)));

                return !(intersectionPosition.Equals(line1.First) || intersectionPosition.Equals(line2.First));
            }
            return false;
        }

        public static Vector2 RotatePoint(Vector2 point, float angle, Vector2? origin)
        {
            if (!origin.HasValue)
                origin = Vector2.Zero;

            return new Vector2(
                origin.Value.X + (float)((point.X - origin.Value.X) * Math.Cos(angle) - (point.Y - origin.Value.Y) * Math.Sin(angle)),
                origin.Value.Y + (float)((point.X - origin.Value.X) * Math.Sin(angle) + (point.Y - origin.Value.Y) * Math.Cos(angle))
            );
        }
    }
}