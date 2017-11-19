using FarseerPhysics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using XmasHell.Physics;
using XmasHell.Physics.Collision;
using XmasHell.Spriter;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.Entities.Bosses.XmasSnowman
{
    class Snowball : ISpriterPhysicsEntity
    {
        private readonly XmasSnowman _boss;
        private CollisionElement _boundingBox;
        private CustomSpriterAnimator _animator;
        private Vector2 _initialPosition;
        public bool Destroyed = false;
        private Body _body;

        public CustomSpriterAnimator GetCurrentAnimator()
        {
            return _animator;
        }

        public Vector2 Position()
        {
            return _animator.Position;
        }

        public void Position(Vector2 value)
        {
            _animator.Position = value;
        }

        public float Rotation()
        {
            return _animator.Rotation;
        }

        public Vector2 Origin()
        {
            return _animator.Origin();
        }

        public Vector2 ScaleVector()
        {
            return _animator.Scale;
        }

        public void TakeDamage(float damage)
        {
            _boss.TakeDamage(damage * 0.25f);
        }

        public Snowball(XmasSnowman boss, CustomSpriterAnimator animator, Vector2 position)
        {
            _boss = boss;
            _animator = animator.Clone();
            _initialPosition = position;

            // TODO: Random scale
            var randomScale = 1f;
            _animator.Scale = new Vector2(randomScale);

            // Physics
            _boundingBox = CreateBoundingBox(_animator.Scale.X);
            _boss.Game.GameManager.CollisionWorld.AddBossHitBox(_boundingBox);

            var randomSpawnBounds = new Rectangle(
                (int)(270 * randomScale), 0,
                (int)(GameConfig.VirtualResolution.X - (270 * randomScale)), 300
            );

            //_body = _boss.CreateGiftBody(_boss.Game.GameManager.GetRandomPosition(false, randomSpawnBounds), _animator.Scale.X);

            Position(_initialPosition);
            _animator.Play("Spawn");

            // Swap body and ribbon with random textures
            var randomIndex = _boss.Game.GameManager.Random.Next(1, 7);

            if (randomIndex > 1)
            { 
                _animator.AddTextureSwap("Graphics/Sprites/Bosses/XmasGift/body", Assets.GetTexture2D("Graphics/Sprites/Bosses/XmasGift/body" + randomIndex));
                _animator.AddTextureSwap("Graphics/Sprites/Bosses/XmasGift/ribbon", Assets.GetTexture2D("Graphics/Sprites/Bosses/XmasGift/ribbon" + randomIndex));
            }
        }

        public void Dispose()
        {
            _boss.Game.GameManager.CollisionWorld.RemoveBossHitBox(_boundingBox);
            Destroyed = true;
        }

        private CollisionElement CreateBoundingBox(float scale = 1f)
        {
            return new SpriterCollisionCircle(_animator, "snowball.png", Vector2.Zero, scale);
        }

        public void Update(GameTime gameTime)
        {
            //_animator.Position = ConvertUnits.ToDisplayUnits(_body.Position);
            //_animator.Rotation = _body.Rotation;

            _animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _animator.Draw(spriteBatch);
        }
    }
}
