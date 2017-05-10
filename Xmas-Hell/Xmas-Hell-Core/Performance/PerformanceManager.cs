using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private struct PerformanceStringData
        {
            public Color TextColor;
            public string Text;
            public Color? DotColor;

            public PerformanceStringData(string text, Color? textColor = null, Color? dotColor = null)
            {
                Text = text;
                TextColor = textColor ?? Color.White;
                DotColor = dotColor;
            }
        }

        private XmasHell _game;
        private List<PerformanceStringData> _performanceInfo;
        private Vector2 _performanceInfoPosition;
        private FramesPerSecondCounterComponent _fpsCounter;
        private Dictionary<PerformanceStopwatchType, Stopwatch> _stopWatches;
        private Graph _performanceGraph;
        private int _maxPerformanceSample;
        private Dictionary<PerformanceStopwatchType, Queue<float>> _performanceData;

        public PerformanceManager(Game game)
        {
            _game = (XmasHell) game;
            _performanceInfo = new List<PerformanceStringData>();
            _stopWatches = new Dictionary<PerformanceStopwatchType, Stopwatch>();
            _maxPerformanceSample = 200;
            _performanceData = new Dictionary<PerformanceStopwatchType, Queue<float>>();
            _performanceInfoPosition = Vector2.Zero;
        }

        public void Initialize()
        {
            _game.Components.Add(_fpsCounter = new FramesPerSecondCounterComponent(_game));
            _performanceGraph = new Graph(_game.GraphicsDevice,
                new Point(GameConfig.VirtualResolution.X, GameConfig.VirtualResolution.Y / 2),
                _game.ViewportAdapter.GetScaleMatrix())
            {
                Position = new Vector2(0, GameConfig.VirtualResolution.Y),
                Type = Graph.GraphType.Line,
                MaxValue = 16f // 16ms = 60FPS
            };
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

            _performanceInfo.Add(new PerformanceStringData($"FPS: {_fpsCounter.FramesPerSecond:0}"));
            _performanceInfo.Add(new PerformanceStringData($"Player's bullets: {_game.GameManager.GetPlayerBullets().Count:0}"));
            _performanceInfo.Add(new PerformanceStringData($"Boss' bullets: {_game.GameManager.GetBossBullets().Count:0}"));
            _performanceInfo.Add(new PerformanceStringData($"Active particles: {_game.GameManager.ParticleManager.ActiveParticlesCount()}"));

            foreach (PerformanceStopwatchType type in Enum.GetValues(typeof(PerformanceStopwatchType)))
            {
                if (_stopWatches.ContainsKey(type))
                {
                    _performanceInfo.Add(
                        new PerformanceStringData(
                            $"{type.ToString()}: {_stopWatches[type].Elapsed.TotalMilliseconds} ms",
                            Color.White, PerformanceStopwatchTypeToColor(type)
                        )
                    );
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (!GameConfig.ShowPerformanceInfo)
                return;

            DrawText(gameTime);

            if (GameConfig.ShowPerformanceGraph)
            {
                foreach (var performanceData in _performanceData)
                    _performanceGraph.Draw(performanceData.Value.ToList(), PerformanceStopwatchTypeToColor(performanceData.Key));
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

                var mainFont = Assets.GetFont("Graphics/Fonts/main");

                for (int i = 0; i < _performanceInfo.Count; i++)
                {
                    var performanceInfo = _performanceInfo[i];
                    var textPosition = _performanceInfoPosition;

                    textPosition.Y += mainFont.LineHeight * i;

                    if (performanceInfo.DotColor.HasValue)
                    {
                        _game.SpriteBatch.Draw(
                            Assets.GetTexture2D("pixel"),
                            new Rectangle(
                                (int)textPosition.X,
                                (int)textPosition.Y + (int)(mainFont.LineHeight / 2f) - (20 / 2),
                                20, 20
                            ),
                            null,
                            performanceInfo.DotColor.Value
                        );
                        textPosition.X += 20;
                    }

                    _game.SpriteBatch.DrawString(mainFont, performanceInfo.Text, textPosition, performanceInfo.TextColor);
                }

                _game.SpriteBatch.End();
            }
        }

        private Color PerformanceStopwatchTypeToColor(PerformanceStopwatchType type)
        {
            switch (type)
            {
              case PerformanceStopwatchType.GlobalUpdate:
                    return Color.Red;
                case PerformanceStopwatchType.GlobalDraw:
                    return Color.Green;
                case PerformanceStopwatchType.BossBulletUpdate:
                    return Color.Orange;
                case PerformanceStopwatchType.CollisionUpdate:
                    return Color.Yellow;
                default:
                    return Color.White;
            }
        }
    }
}
