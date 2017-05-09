using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace XmasHell.Performance
{
    public enum PerformanceStopwatchType
    {
        GlobalUpdate,
        ParticleUpdate,
        BossBulletUpdate,
        PlayerBulletUpdate,
        BossBehaviourUpdate,
        GlobalDraw,
        ParticleDraw,
        BossBulletDraw,
        PlayerBulletDraw,
        BloomDraw,
    }

    public class PerformanceManager
    {
        private XmasHell _game;
        private StringBuilder _performanceInfo;
        private FramesPerSecondCounterComponent _fpsCounter;

        private Dictionary<PerformanceStopwatchType, Stopwatch> _stopWatches;

        public PerformanceManager(Game game)
        {
            _game = (XmasHell) game;
            _performanceInfo = new StringBuilder();
            _stopWatches = new Dictionary<PerformanceStopwatchType, Stopwatch>();
        }

        public void Initialize()
        {
            // FPS counter
            _game.Components.Add(_fpsCounter = new FramesPerSecondCounterComponent(_game));
        }

        public void Update(GameTime gameTime)
        {
            UpdatePerformanceStrings();
        }

        public void Draw(GameTime gameTime)
        {
            DrawText(gameTime);
        }

        private void UpdatePerformanceStrings()
        {
            _performanceInfo.Clear();

            _performanceInfo.AppendLine($"FPS: {_fpsCounter.FramesPerSecond:0}");
            _performanceInfo.AppendLine($"Player's bullets: {_game.GameManager.GetPlayerBullets().Count:0}");
            _performanceInfo.AppendLine($"Boss' bullets: {_game.GameManager.GetBossBullets().Count:0}");
            _performanceInfo.AppendLine($"Active particles: {_game.GameManager.ParticleManager.ActiveParticlesCount()}");

            if (_stopWatches.ContainsKey(PerformanceStopwatchType.GlobalUpdate))
                _performanceInfo.AppendLine($"Update time: {_stopWatches[PerformanceStopwatchType.GlobalUpdate].ElapsedMilliseconds} ms");

            if (_stopWatches.ContainsKey(PerformanceStopwatchType.GlobalDraw))
                _performanceInfo.AppendLine($"Draw time: {_stopWatches[PerformanceStopwatchType.GlobalDraw].ElapsedMilliseconds} ms");
        }

        public void StartStopwatch(PerformanceStopwatchType type)
        {
            if (_stopWatches.ContainsKey(type))
                _stopWatches[type].Reset();
            else
                _stopWatches.Add(type, new Stopwatch());

            _stopWatches[type].Start();
        }

        public void StopStopwatch(PerformanceStopwatchType type)
        {
            if (!_stopWatches.ContainsKey(type))
                return;

            _stopWatches[type].Stop();
        }

        public void DrawText(GameTime gameTime)
        {
            if (GameConfig.ShowDebugInfo)
            {
                _game.SpriteBatch.Begin(
                    samplerState: SamplerState.PointClamp,
                    blendState: BlendState.AlphaBlend,
                    transformMatrix: _game.ViewportAdapter.GetScaleMatrix()
                );

                _game.SpriteBatch.DrawString(Assets.GetFont("Graphics/Fonts/main"), _performanceInfo.ToString(), Vector2.Zero, Color.White);

                _game.SpriteBatch.End();
            }
        }
    }
}
