using System.Collections.Generic;
using BulletML;
using Microsoft.Xna.Framework;

namespace Xmas_Hell.BulletML
{
    public class MoverManager : IBulletManager
    {
        public readonly List<Mover> Movers = new List<Mover>();
        private readonly List<Mover> _topLevelMovers = new List<Mover>();
        private PositionDelegate _getPlayerPosition;

        public MoverManager()
        {
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
            var mover = new Mover(this);
            mover.Init();

            if (topBullet)
                _topLevelMovers.Add(mover);
            else
                Movers.Add(mover);

            return mover;
        }

        public void RemoveBullet(IBullet deadBullet)
        {
            var mover = deadBullet as Mover;

            if (mover != null)
                mover.Used = false;
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
                    Movers.Remove(Movers[i]);
                    i--;
                }
            }

            for (int i = 0; i < _topLevelMovers.Count; i++)
            {
                if (_topLevelMovers[i].TasksFinished())
                {
                    _topLevelMovers.RemoveAt(i);
                    i--;
                }
            }
        }

        public void Clear()
        {
            Movers.Clear();
            _topLevelMovers.Clear();
        }
    }
}
