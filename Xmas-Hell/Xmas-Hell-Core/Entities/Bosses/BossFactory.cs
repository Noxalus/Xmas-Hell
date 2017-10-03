using System;
using System.Diagnostics;
using BulletML;
using XmasHell.Entities.Bosses.XmasBall;

namespace XmasHell.Entities.Bosses
{
    public enum BossType
    {
        Debug,
        XmasBall,
        XmasBell,
        XmasCandy,
        XmasSnowflake,
        XmasLog,
        XmasTree,
        XmasGift,
        XmasReinder,
        XmasSnowman,
        XmasSanta
    }

    public static class BossFactory
    {
        public static Boss CreateBoss(BossType type, XmasHell game, PositionDelegate playerPositionDelegate)
        {
            switch (type)
            {
                case BossType.Debug:
                    return new DebugBoss.BossDebug(game, playerPositionDelegate);
                case BossType.XmasBall:
                    return new XmasBall.XmasBall(game, playerPositionDelegate);
                case BossType.XmasBell:
                    return new XmasBell.XmasBell(game, playerPositionDelegate);
                case BossType.XmasCandy:
                    return new XmasCandy.XmasCandy(game, playerPositionDelegate);
                case BossType.XmasSnowflake:
                    return new XmasSnowflake.XmasSnowflake(game, playerPositionDelegate);
                case BossType.XmasLog:
                    return new XmasLog.XmasLog(game, playerPositionDelegate);
                case BossType.XmasTree:
                    break;
                case BossType.XmasGift:
                    return new XmasGift.XmasGift(game, playerPositionDelegate);
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

        public static BossType StringToBossType(string type)
        {
            switch (type)
            {
                case "debug":
                    return BossType.Debug;
                case "ball":
                    return BossType.XmasBall;
                case "bell":
                    return BossType.XmasBell;
                case "candy":
                    return BossType.XmasCandy;
                case "snowflake":
                    return BossType.XmasSnowflake;
                case "log":
                    return BossType.XmasLog;
                case "tree":
                    return BossType.XmasTree;
                case "gift":
                    return BossType.XmasGift;
                case "reinder":
                    return BossType.XmasReinder;
                case "snowman":
                    return BossType.XmasSnowman;
                case "santa":
                    return BossType.XmasSanta;
                default:
                    return BossType.Debug;
            }
        }
    }
}