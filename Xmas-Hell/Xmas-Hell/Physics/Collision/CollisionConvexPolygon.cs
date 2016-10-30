using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Shapes;

namespace Xmas_Hell.Physics.Collision
{
    class CollisionConvexPolygon : CollisionElement
    {
        private Vector2 _relativePosition;
        // Local positions relative to the entity position
        private List<Vector2> _vertices;


        public CollisionConvexPolygon(IPhysicsEntity entity, Vector2 relativePosition, List<Vector2> vertices) : base(entity)
        {
            _relativePosition = relativePosition;
            _vertices = vertices;
        }

        public Vector2 GetWorldPosition(Vector2 vertex)
        {
            var entityTransformMatrix = GetMatrix();
            var vertexPosition = new Vector3(vertex.X, vertex.Y, 0f);
            var transformedVertexPosition = Vector3.Transform(vertexPosition, entityTransformMatrix);
            var newPosition = new Vector2(transformedVertexPosition.X, transformedVertexPosition.Y);

            return newPosition;
        }

        public override bool Intersects(CollisionElement element)
        {
            throw new System.NotImplementedException();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_vertices.Count == 0)
                return;

            var previousPosition = GetWorldPosition(_vertices[0]);

            for (int i = 1; i <= _vertices.Count; i++)
            {
                var position = GetWorldPosition(i == _vertices.Count ? _vertices[0] : _vertices[i]);

                spriteBatch.DrawLine(
                    previousPosition.X,
                    previousPosition.Y,
                    position.X,
                    position.Y, Color.Red);

                /*
                Vector2 axis = Vector2.Normalize(position - previousPosition);
                spriteBatch.DrawLine(
                    (previousPosition.X + position.X) / 2f,
                    (previousPosition.Y + position.Y) / 2f,
                    (previousPosition.X) + axis.Y * 2000,
                    (previousPosition.Y) - axis.X * 2000,
                    Color.Red);
                */

                previousPosition = position;
            }
        }
    }
}