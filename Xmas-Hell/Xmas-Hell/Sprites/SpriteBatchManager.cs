using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Sprites;
using Xmas_Hell.BulletML;
using Xmas_Hell.Entities;
using Xmas_Hell.Physics.Collision;
using Xmas_Hell.Shaders;
using Sprite = MonoGame.Extended.Sprites.Sprite;

namespace Xmas_Hell
{
    // This class is used to batch the draw calls per category
    // This will not work, it needs more reflexion...
    public class SpriteBatchManager
    {
        private XmasHell _game;

        public List<Sprite> BackgroundSprites;
        public List<ParticleEffect> BackgroundParticles;
        public List<Sprite> UISprites;
        public List<Mover> BossBullets;
        public List<Sprite> GameSprites;
        public List<ParticleEffect> GameParticles;
        public List<CollisionElement> DebugCollisionElements;

        public Boss Boss;
        public Sprite Player;

        // Bloom
        private Bloom _bloom;
        private int _bloomSettingsIndex = 0;
        private RenderTarget2D _renderTarget1;
        private RenderTarget2D _renderTarget2;

        public SpriteBatchManager(XmasHell game)
        {
            _game = game;

            BackgroundSprites = new List<Sprite>();
            BackgroundParticles = new List<ParticleEffect>();
            UISprites = new List<Sprite>();
            BossBullets = new List<Mover>();
            GameSprites = new List<Sprite>();
            GameParticles = new List<ParticleEffect>();
            DebugCollisionElements = new List<CollisionElement>();
        }

        public void Initialize()
        {
            _bloom = new Bloom(_game.GraphicsDevice, _game.SpriteBatch);

            var pp = _game.GraphicsDevice.PresentationParameters;

            _renderTarget1 = new RenderTarget2D(
                _game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents
            );
            _renderTarget2 = new RenderTarget2D(_game.GraphicsDevice, pp.BackBufferWidth, pp.BackBufferHeight, false,
                pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents
            );
        }

        public void LoadContent()
        {
            _bloom.LoadContent(_game.Content, _game.GraphicsDevice.PresentationParameters);
        }

        public void UnloadContent()
        {
            _bloom.UnloadContent();
            _renderTarget1.Dispose();
            _renderTarget2.Dispose();
        }

        public void Update()
        {
        }

        public void Draw()
        {
            _game.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start by render the bloomed elements into a render target
            if (!GameConfig.DisableBloom)
            {
                // The next draw calls will be rendered in the first render target
                _game.GraphicsDevice.SetRenderTarget(_renderTarget1);
                _game.GraphicsDevice.Clear(Color.Transparent);

                _game.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    blendState: BlendState.AlphaBlend,
                    transformMatrix: _game.Camera.GetViewMatrix()
                );

                foreach (var mover in BossBullets)
                    _game.SpriteBatch.Draw(mover.Sprite);

                _game.SpriteBatch.End();

                // Apply bloom effect on the first render target and store the
                // result into the second render target
                _bloom.Draw(_renderTarget1, _renderTarget2);

                // We want to render into the back buffer from now on
                _game.GraphicsDevice.SetRenderTarget(null);
            }

            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.ViewportAdapter.GetScaleMatrix()
            );

            // Draw background sprites
            foreach (var sprite in BackgroundSprites)
                sprite.Draw(_game.SpriteBatch);

            foreach (var particle in BackgroundParticles)
                _game.SpriteBatch.Draw(particle);

            _game.SpriteBatch.End();

            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.Camera.GetViewMatrix()
            );

            // Draw player
            if (Player != null)
                _game.SpriteBatch.Draw(Player);

            _game.SpriteBatch.End();

            // Draw boss
            if (Boss != null)
            {
                if (Boss.Tinted)
                {
                    var basicTintEffect = Assets.GetShader("Graphics/Shaders/BasicTint");
                    basicTintEffect.Parameters["tintColor"].SetValue(Boss.HitColor.ToVector3());

                    _game.SpriteBatch.Begin(
                        samplerState: SamplerState.PointClamp,
                        blendState: BlendState.AlphaBlend,
                        transformMatrix: _game.Camera.GetViewMatrix(),
                        effect: basicTintEffect,
                        sortMode: SpriteSortMode.Immediate
                    );
                }
                else
                {
                    _game.SpriteBatch.Begin(
                        samplerState: SamplerState.PointClamp,
                        blendState: BlendState.AlphaBlend,
                        transformMatrix: _game.Camera.GetViewMatrix()
                    );
                }

                Boss.CurrentAnimator.Draw(_game.SpriteBatch);

                _game.SpriteBatch.End();
            }

            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.Camera.GetViewMatrix()
            );

            // Draw game sprites
            foreach (var sprite in GameSprites)
                sprite.Draw(_game.SpriteBatch);

            foreach (var particle in GameParticles)
                _game.SpriteBatch.Draw(particle);

            if (GameConfig.DisplayCollisionBoxes)
            {
                foreach (var collisionElement in DebugCollisionElements)
                    collisionElement.Draw(_game.SpriteBatch);
            }

            _game.SpriteBatch.End();

            if (!GameConfig.DisableBloom)
            {
                // Draw bloom render target
                // Draw the second render target on top of everything
                _game.SpriteBatch.Begin(0, BlendState.AlphaBlend);
                _game.SpriteBatch.Draw(_renderTarget2, new Rectangle(
                    0, 0,
                    _game.GraphicsDevice.PresentationParameters.BackBufferWidth,
                    _game.GraphicsDevice.PresentationParameters.BackBufferHeight
                ), Color.White);
                _game.SpriteBatch.End();
            }
            else
            {
                _game.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    blendState: BlendState.AlphaBlend,
                    transformMatrix: _game.Camera.GetViewMatrix()
                );

                foreach (var mover in BossBullets)
                    _game.SpriteBatch.Draw(mover.Sprite);

                _game.SpriteBatch.End();
            }

            if (GameConfig.DisplayCollisionBoxes)
            {
                _game.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    blendState: BlendState.AlphaBlend,
                    transformMatrix: _game.Camera.GetViewMatrix()
                );


                foreach (var collisionElement in DebugCollisionElements)
                    collisionElement.Draw(_game.SpriteBatch);

                _game.SpriteBatch.End();
            }

            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.AlphaBlend,
                transformMatrix: _game.ViewportAdapter.GetScaleMatrix()
            );

            // Draw UI elements
            foreach (var sprite in UISprites)
                sprite.Draw(_game.SpriteBatch);

            _game.SpriteBatch.End();

            // Draw strings
            // TODO: Think to a good way to do that
        }
    }
}