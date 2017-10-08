using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Particles;
using MonoGame.Extended.Sprites;
using XmasHell.Background;
using XmasHell.BulletML;
using XmasHell.Entities;
using XmasHell.Entities.Bosses;
using XmasHell.Performance;
using XmasHell.Shaders;
using System;
using XmasHell.Spriter;

namespace XmasHell.Rendering
{
    public enum Layer
    {
        BACKGROUND,
        BACK,
        FRONT,
        UI
    };


    // This class is used to batch the draw calls per category
    public class SpriteBatchManager
    {
        private XmasHell _game;

        public AbstractBackground Background;
        public List<Sprite> BackgroundSprites = new List<Sprite>();
        public List<ParticleEffect> BackgroundParticles = new List<ParticleEffect>();
        public List<Sprite> UISprites = new List<Sprite>();
        public List<Mover> BossBullets = new List<Mover>();
        public List<Laser> Lasers = new List<Laser>();
        public List<Sprite> GameSprites = new List<Sprite>();
        public List<ParticleEffect> GameParticles = new List<ParticleEffect>();

        public Boss Boss;
        public Player Player;
        public Sprite PlayerHitbox;

        // Bloom
        public Bloom Bloom;
        public int BloomSettingsIndex = 0;
        private float _bloomSaturationPulse = 1f;
        private float _bloomSaturationDirection = 0.09f;

        private RenderTarget2D _renderTarget1;
        private RenderTarget2D _renderTarget2;

        private List<CustomSpriterAnimator> _backgroundSpriterAnimators = new List<CustomSpriterAnimator>();
        private List<CustomSpriterAnimator> _uiSpriterAnimators = new List<CustomSpriterAnimator>();

        // Use to delayed Spriter animator collection modification (to be able to easily call code in CustomSpriterAnimator callbacks)
        private List<Tuple<CustomSpriterAnimator, Layer>> _animatorsToRemove = new List<Tuple<CustomSpriterAnimator, Layer>>();
        private List<Tuple<CustomSpriterAnimator, Layer>> _animatorsToAdd = new List<Tuple<CustomSpriterAnimator, Layer>>();

        public void AddSpriterAnimator(CustomSpriterAnimator animator, Layer layer)
        {
            _animatorsToAdd.Add(new Tuple<CustomSpriterAnimator, Layer>(animator, layer));
        }

        public void RemoveSpriterAnimator(CustomSpriterAnimator animator, Layer layer)
        {
            _animatorsToRemove.Add(new Tuple<CustomSpriterAnimator, Layer>(animator, layer));
        }

        public SpriteBatchManager(XmasHell game)
        {
            _game = game;
        }

        public void SortSpriterAnimator(Layer? layer = null)
        {
            if (layer.HasValue)
            {
                switch (layer.Value)
                {
                    case Layer.BACKGROUND:
                        _backgroundSpriterAnimators.Sort();
                        break;
                    case Layer.UI:
                        _uiSpriterAnimators.Sort();
                        break;
                }
            }
            else
            {
                _backgroundSpriterAnimators.Sort();
                _uiSpriterAnimators.Sort();
            }
        }

        public void Initialize()
        {
            if (GameConfig.EnableBloom)
            {
                Bloom = new Bloom(_game.GraphicsDevice, _game.SpriteBatch);

                var pp = _game.GraphicsDevice.PresentationParameters;

                _renderTarget1 = new RenderTarget2D(
                    _game.GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y, false,
                    pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents
                );
                _renderTarget2 = new RenderTarget2D(_game.GraphicsDevice, GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y, false,
                    pp.BackBufferFormat, pp.DepthStencilFormat, pp.MultiSampleCount, RenderTargetUsage.DiscardContents
                );
            }
        }

        public void LoadContent()
        {
            if (GameConfig.EnableBloom)
                Bloom.LoadContent(_game.Content, _game.GraphicsDevice.PresentationParameters);
        }

        public void UnloadContent()
        {
            if (GameConfig.EnableBloom)
            {
                Bloom.UnloadContent();
                _renderTarget1.Dispose();
                _renderTarget2.Dispose();
            }
        }

        private void AddSpriterAnimators()
        {
            foreach (var animatorTuple in _animatorsToAdd)
            {
                var animator = animatorTuple.Item1;
                var layer = animatorTuple.Item2;

                switch (layer)
                {
                    case Layer.BACKGROUND:
                        if (!_backgroundSpriterAnimators.Exists(a => a.Entity.Name == animator.Entity.Name))
                        {
                            _backgroundSpriterAnimators.Add(animator);
                        }
                        break;
                    case Layer.UI:
                        _uiSpriterAnimators.Add(animator);
                        break;
                }
            }

            if (_animatorsToAdd.Count > 0)
            {
                SortSpriterAnimator();
                _animatorsToAdd.Clear();
            }
        }

        private void CleanSpriterAnimators()
        {
            foreach (var animatorTuple in _animatorsToRemove)
            {
                var animator = animatorTuple.Item1;
                var layer = animatorTuple.Item2;

                switch (layer)
                {
                    case Layer.BACKGROUND:
                        if (_backgroundSpriterAnimators.Exists(a => a.Entity.Name == animator.Entity.Name))
                            _backgroundSpriterAnimators.Remove(animator);
                        break;
                    case Layer.UI:
                        if (_uiSpriterAnimators.Exists(a => a.Entity.Name == animator.Entity.Name))
                            _uiSpriterAnimators.Remove(animator);
                        break;
                }
            }

            _animatorsToRemove.Clear();
        }

        public void Update(GameTime gameTime)
        {
            // Clean Spriter animators to remove
            CleanSpriterAnimators();

            AddSpriterAnimators();

            if (GameConfig.EnableBloom)
            {
                _bloomSaturationPulse += _bloomSaturationDirection;
                if (_bloomSaturationPulse > 2.5f) _bloomSaturationDirection = -0.09f;
                if (_bloomSaturationPulse < 0.1f) _bloomSaturationDirection = 0.09f;

                Bloom.Settings.BloomSaturation = _bloomSaturationPulse;
            }

            _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.BackgroundUpdate);
            Background?.Update(gameTime);
            _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.BackgroundUpdate);

            foreach (var animator in _backgroundSpriterAnimators)
                animator.Update(gameTime.ElapsedGameTime.Milliseconds);

            foreach (var animator in _uiSpriterAnimators)
                animator.Update(gameTime.ElapsedGameTime.Milliseconds);
        }

        private void BeginDrawViewportSpace()
        {
            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.NonPremultiplied,
                transformMatrix: _game.ViewportAdapter.GetScaleMatrix()
            );
        }

        private void BeginDrawCameraSpace()
        {
            _game.SpriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                blendState: BlendState.NonPremultiplied,
                transformMatrix: _game.Camera.GetViewMatrix()
            );
        }

        private void DrawBloomedElements(GameTime gameTime)
        {
            BeginDrawCameraSpace();

            PlayerHitbox?.Draw(_game.SpriteBatch);

            _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.BossBulletDraw);

            foreach (var mover in BossBullets)
                _game.SpriteBatch.Draw(mover.Sprite);


            foreach (var laser in Lasers)
                laser.Draw(gameTime);


            _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.BossBulletDraw);

            _game.SpriteBatch.End();
        }

        private void DrawBoss()
        {
            if (Boss == null)
                return;

            if (Boss.Tinted)
            {
                var basicTintEffect = Assets.GetShader("Graphics/Shaders/BasicTint");
                basicTintEffect.Parameters["tintColor"].SetValue(Boss.HitColor.ToVector3());

                _game.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    blendState: BlendState.NonPremultiplied,
                    transformMatrix: _game.Camera.GetViewMatrix(),
                    effect: basicTintEffect
                );
            }
            else
            {
                BeginDrawCameraSpace();
            }

            Boss.Draw();

            _game.SpriteBatch.End();
        }

        public void Draw(GameTime gameTime)
        {
            // Start by render the bloomed elements into a render target
            if (GameConfig.EnableBloom)
            {
                _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.BloomDraw);

                // The next draw calls will be rendered in the first render target
                _game.GraphicsDevice.SetRenderTarget(_renderTarget1);
                _game.GraphicsDevice.Clear(Color.Transparent);

                DrawBloomedElements(gameTime);

                // Apply bloom effect on the first render target and store the
                // result into the second render target
                Bloom.Draw(_renderTarget1, _renderTarget2);

                // We want to render into the back buffer from now on
                _game.GraphicsDevice.SetRenderTarget(null);

                // Reset the viewport
                _game.ViewportAdapter.Reset();

                _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.BloomDraw);
            }

            _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.BackgroundDraw);
            Background?.Draw();
            _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.BackgroundDraw);

            BeginDrawViewportSpace();

            // Draw background sprites
            foreach (var sprite in BackgroundSprites)
                sprite.Draw(_game.SpriteBatch);

            foreach (var animator in _backgroundSpriterAnimators)
                animator.Draw(_game.SpriteBatch);

            _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.BackgroundParticleDraw);

            foreach (var particle in BackgroundParticles)
                _game.SpriteBatch.Draw(particle);

            _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.BackgroundParticleDraw);

            _game.SpriteBatch.End();

            BeginDrawCameraSpace();

            // Draw player
            Player?.CurrentAnimator.Draw(_game.SpriteBatch);

            _game.SpriteBatch.End();

            // Draw boss
            DrawBoss();

            BeginDrawCameraSpace();

            // Draw game sprites
            foreach (var sprite in GameSprites)
                sprite.Draw(_game.SpriteBatch);

            _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.GameParticleDraw);

            foreach (var particle in GameParticles)
                _game.SpriteBatch.Draw(particle);

            _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.GameParticleDraw);

            _game.SpriteBatch.End();

            if (GameConfig.EnableBloom)
            {
                _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.BloomRenderTargetDraw);

                // Draw bloom render target
                // Draw the second render target on top of everything
                BeginDrawViewportSpace();

                _game.SpriteBatch.Draw(_renderTarget2, new Rectangle(
                    0, 0,
                    GameConfig.VirtualResolution.X,
                    GameConfig.VirtualResolution.Y
                ), Color.White);

                _game.SpriteBatch.End();

                _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.BloomRenderTargetDraw);
            }
            else
            {
                DrawBloomedElements(gameTime);
            }

            _game.PerformanceManager.StartStopwatch(PerformanceStopwatchType.UIDraw);

            BeginDrawViewportSpace();

            // Draw UI elements
            foreach (var sprite in UISprites)
                sprite.Draw(_game.SpriteBatch);

            foreach (var animator in _uiSpriterAnimators)
                animator.Draw(_game.SpriteBatch);

            _game.SpriteBatch.End();

            _game.PerformanceManager.StopStopwatch(PerformanceStopwatchType.UIDraw);

            // Draw strings
            // TODO: Think to a good way to do that
        }
    }
}