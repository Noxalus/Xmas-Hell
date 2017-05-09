using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        CollisionUpdate,
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
        private Graph _performanceGraph;
        private int _maxPerformanceSample;
        private Dictionary<PerformanceStopwatchType, Queue<float>> _performanceData;

        public PerformanceManager(Game game)
        {
            _game = (XmasHell) game;
            _performanceInfo = new StringBuilder();
            _stopWatches = new Dictionary<PerformanceStopwatchType, Stopwatch>();
            _maxPerformanceSample = 200;
            _performanceData = new Dictionary<PerformanceStopwatchType, Queue<float>>();
        }

        public void Initialize()
        {
            _game.Components.Add(_fpsCounter = new FramesPerSecondCounterComponent(_game));
            _performanceGraph = new Graph(_game.GraphicsDevice, new Point(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y / 2), _game.ViewportAdapter.GetScaleMatrix());
            _performanceGraph.Position = new Vector2(0, GameConfig.VirtualResolution.Y);
            _performanceGraph.Type = Graph.GraphType.Line;
            _performanceGraph.MaxValue = 16f; // 16ms = 60FPS
        }

        public void StartStopwatch(PerformanceStopwatchType type)
        {
            if (!GameConfig.ShowPerformanceInfo)
               return;

                if (_stopWatches.ContainsKey(type))
                _stopWatches[type].Reset();
            else
            {
                _stopWatches.Add(type, new Stopwatch());
                _performanceData.Add(type, new Queue<float>(_maxPerformanceSample));
            }

            _stopWatches[type].Start();
        }

        public void StopStopwatch(PerformanceStopwatchType type)
        {
            if (!GameConfig.ShowPerformanceInfo || !_stopWatches.ContainsKey(type))
                return;

            _stopWatches[type].Stop();
        }

        public void Update(GameTime gameTime)
        {
            if (!GameConfig.ShowPerformanceInfo)
                return;

            UpdatePerformanceStrings();

            if (GameConfig.ShowPerformanceGraph)
            {
                foreach (var performanceData in _performanceData)
                {
                    if (performanceData.Value.Count >= _maxPerformanceSample)
                        performanceData.Value.Dequeue();

                    performanceData.Value.Enqueue((float)_stopWatches[performanceData.Key].Elapsed.TotalMilliseconds);
                }
            }
        }

        private void UpdatePerformanceStrings()
        {
            _performanceInfo.Clear();

            _performanceInfo.AppendLine($"FPS: {_fpsCounter.FramesPerSecond:0}");
            _performanceInfo.AppendLine($"Player's bullets: {_game.GameManager.GetPlayerBullets().Count:0}");
            _performanceInfo.AppendLine($"Boss' bullets: {_game.GameManager.GetBossBullets().Count:0}");
            _performanceInfo.AppendLine($"Active particles: {_game.GameManager.ParticleManager.ActiveParticlesCount()}");

            if (_stopWatches.ContainsKey(PerformanceStopwatchType.GlobalUpdate))
                _performanceInfo.AppendLine($"Update time: {_stopWatches[PerformanceStopwatchType.GlobalUpdate].Elapsed.TotalMilliseconds} ms");

            if (_stopWatches.ContainsKey(PerformanceStopwatchType.BossBulletUpdate))
                _performanceInfo.AppendLine($"  Boss' bullets update time: {_stopWatches[PerformanceStopwatchType.BossBulletUpdate].Elapsed.TotalMilliseconds} ms");

            if (_stopWatches.ContainsKey(PerformanceStopwatchType.CollisionUpdate))
                _performanceInfo.AppendLine($"  Collision update time: {_stopWatches[PerformanceStopwatchType.CollisionUpdate].Elapsed.TotalMilliseconds} ms");

            if (_stopWatches.ContainsKey(PerformanceStopwatchType.GlobalDraw))
                _performanceInfo.AppendLine($"Draw time: {_stopWatches[PerformanceStopwatchType.GlobalDraw].Elapsed.TotalMilliseconds} ms");
        }

        public void Draw(GameTime gameTime)
        {
            if (!GameConfig.ShowPerformanceInfo)
                return;

            DrawText(gameTime);

            if (GameConfig.ShowPerformanceGraph)
            {
                foreach (var performanceData in _performanceData)
                {
                    _performanceGraph.Draw(performanceData.Value.ToList(), PerformanceStopwatchTypeToColor(performanceData.Key));
                }
            }
        }

        private void DrawText(GameTime gameTime)
        {
            if (GameConfig.ShowPerformanceInfo)
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

        private Color PerformanceStopwatchTypeToColor(PerformanceStopwatchType type)
        {
            switch (type)
            {
              case PerformanceStopwatchType.GlobalUpdate:
                    return Color.Red;
                    break;

                case PerformanceStopwatchType.GlobalDraw:
                    return Color.Green;
                    break;

                case PerformanceStopwatchType.BossBulletUpdate:
                    return Color.Orange;
                    break;

                case PerformanceStopwatchType.CollisionUpdate:
                    return Color.Yellow;
                    break;

                default:
                    return Color.White;
                    break;
            }
        }
    }
}
