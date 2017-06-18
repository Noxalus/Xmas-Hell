using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using XmasHell.Extensions;

namespace XmasHell.Physics.Collision
{
    public class CollisionConvexPolygon : CollisionElement
    {
        private Vector2 _relativePosition;
        // Local positions relative to the entity position
        protected readonly List<Vector2> Vertices = new List<Vector2>();

        public CollisionConvexPolygon(IPhysicsEntity entity, Vector2 relativePosition, List<Vector2> vertices) : base(entity)
        {
            _relativePosition = relativePosition;
            Vertices = vertices;
        }

        protected CollisionConvexPolygon(IPhysicsEntity entity) : base(entity)
        {
        }

        public virtual Vector2 GetWorldPosition(Vector2 vertex)
        {
            var entityTransformMatrix = GetMatrix();
            var vertexPosition = new Vector3(vertex + _relativePosition, 0f);
            var transformedVertexPosition = Vector3.Transform(vertexPosition, entityTransformMatrix);
            var newPosition = new Vector2(transformedVertexPosition.X, transformedVertexPosition.Y);

            return newPosition;
        }

        public override bool Intersects(CollisionCircle circle)
        {
            var radiusSquared = circle.GetRadius() * circle.GetRadius();
            var vertex = GetWorldPosition(Vertices[Vertices.Count - 1]);
            var circleCenter = circle.GetCenter();
            var nearestDistance = float.MaxValue;
            var nearestIsInside = false;
            var nearestVertex = -1;
            var lastIsInside = false;

            for (var i = 0; i < Vertices.Count; i++)
            {
                var nextVertex = GetWorldPosition(Vertices[i]);
                var axis = circleCenter - vertex;
                var distance = axis.LengthSquared() - radiusSquared;

                if (distance <= 0)
                    return true;

                var isInside = false;
                var edge = nextVertex - vertex;
                var edgeLengthSquared = edge.LengthSquared();

                if (!edgeLengthSquared.Equals(0))
                {
                    // Check whether the circle overlap an edge of the polyhon
                    var dot = Vector2.Dot(edge, axis);

                    if (dot >= 0 && dot <= edgeLengthSquared)
                    {
                        var projection = vertex + (dot / edgeLengthSquared) * edge;
                        axis = projection - circleCenter;

                        if (axis.LengthSquared() <= radiusSquared)
                            return true;

                        if (edge.X > 0)
                        {
                            if (axis.Y > 0)
                                return false;
                        }
                        else if (edge.X < 0)
                        {
                            if (axis.Y < 0)
                                return false;
                        }
                        else if (edge.Y > 0)
                        {
                            if (axis.X < 0)
                                return false;
                        }
                        else if (edge.Y < 0)
                        {
                            if (axis.X > 0)
                                return false;
                        }
                        else
                            isInside = true;
                    }
                }

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestIsInside = isInside;
                    nearestVertex = i;
                }

                vertex = nextVertex;
                lastIsInside = isInside;
            }

            if (nearestVertex == 0)
                return nearestIsInside || lastIsInside;

            return nearestIsInside;
        }

        public override bool Intersects(CollisionConvexPolygon element)
        {
            throw new System.NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Vertices.Count == 0)
                return;

            var previousPosition = GetWorldPosition(Vertices[0]);

            for (int i = 1; i <= Vertices.Count; i++)
            {
                var position = GetWorldPosition(i == Vertices.Count ? Vertices[0] : Vertices[i]);

                // Draw edges
                spriteBatch.DrawLine(
                    previousPosition.X,
                    previousPosition.Y,
                    position.X,
                    position.Y,
                    Color.Red,
                    5f
                );

                // Draw normals
                Vector2 axis = Vector2.Normalize(position - previousPosition);
                spriteBatch.DrawLine(
                    (previousPosition.X + position.X) / 2f,
                    (previousPosition.Y + position.Y) / 2f,
                    (previousPosition.X) + axis.Y * 2000,
                    (previousPosition.Y) - axis.X * 2000,
                    Color.Red,
                    2f);

                previousPosition = position;
            }
        }
    }
}