using BulletML;
using Microsoft.Xna.Framework;

namespace Xmas_Hell.BulletML
{
    public class Mover : Bullet
    {
        public Vector2 Position;

        public override float X
        {
            get { return Position.X; }
            set { Position.X = value; }
        }

        public override float Y
        {
            get { return Position.Y; }
            set { Position.Y = value; }
        }

        public bool Used { get; set; }

        public Mover(IBulletManager bulletManager) : base(bulletManager)
        {
        }

        public void Init()
        {
            Used = true;
        }

        public override void Update()
        {
            base.Update();

            //if (X < 0 || X > Config.GameAeraSize.X || Y < 0 || Y > Config.GameAeraSize.Y)
            //    Used = false;
        }
    }
}
