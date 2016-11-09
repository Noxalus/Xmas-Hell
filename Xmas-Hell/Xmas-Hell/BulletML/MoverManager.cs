using System.Collections.Generic;
using BulletML;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XmasHell.BulletML
{
    public class MoverManager : IBulletManager
    {
        private XmasHell _game;
        public readonly List<Mover> Movers = new List<Mover>();
        private readonly List<Mover> _topLevelMovers = new List<Mover>();
        private PositionDelegate _getPlayerPosition;
        public BulletType CurrentBulletType;

        public MoverManager(XmasHell game)
        {
            _game = game;
        }

        public void SetPlayerPositionDelegate(PositionDelegate playerDelegate)
        {
            _getPlayerPosition = playerDelegate;
        }

        public Vector2 PlayerPosition(IBullet targettedBullet)
        {
            return _getPlayerPosition();
        }

        public IBullet CreateBullet(bool topBullet = false)
        {
            var mover = new Mover(_game, this)
            {
                Texture = BulletTypeUtils.BulletTypeToTexture(CurrentBulletType)
            };

            mover.Init(topBullet);

            if (topBullet)
                _topLevelMovers.Add(mover);
            else
                Movers.Add(mover);

            return mover;
        }

        public void RemoveBullet(IBullet deadBullet)
        {
            var mover = deadBullet as Mover;
            mover?.Destroy();
        }

        public void Update()
        {
            for (int i = 0; i < Movers.Count; i++)
                Movers[i].Update();

            for (int i = 0; i < _topLevelMovers.Count; i++)
                _topLevelMovers[i].Update();

            FreeMovers();
        }

        private void FreeMovers()
        {
            for (int i = 0; i < Movers.Count; i++)
            {
                if (!Movers[i].Used)
                {
                    Movers[i].Destroy();

                    Movers.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < _topLevelMovers.Count; i++)
            {
                if (_topLevelMovers[i].TasksFinished())
                {
                    _topLevelMovers[i].Destroy();

                    _topLevelMovers.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Clear()
        {
            foreach (var mover in Movers)
                mover.Destroy();

            foreach (var topLevelMover in _topLevelMovers)
                topLevelMover.Destroy();

            Movers.Clear();
            _topLevelMovers.Clear();
        }
    }
}
