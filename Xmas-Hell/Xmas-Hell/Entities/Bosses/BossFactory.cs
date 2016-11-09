using System;
using BulletML;

namespace XmasHell.Entities.Bosses
{
    public static class BossFactory
    {
        public static Boss CreateBoss(BossType type, XmasHell game, PositionDelegate playerPositionDelegate)
        {
            switch (type)
            {
                case BossType.XmasBall:
                    return new XmasBall.XmasBall(game, playerPositionDelegate);
                case BossType.XmasBell:
                    return new XmasBell.XmasBell(game, playerPositionDelegate);
                case BossType.XmasCandy:
                    break;
                case BossType.XmasSnowflake:
                    break;
                case BossType.XmasLog:
                    return new XmasLog.XmasLog(game, playerPositionDelegate);
                case BossType.XmasTree:
                    break;
                case BossType.XmasGift:
                    break;
                case BossType.XmasReinder:
                    break;
                case BossType.XmasSnowman:
                    break;
                case BossType.XmasSanta:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            return new XmasBall.XmasBall(game, playerPositionDelegate);
        }

    }
}