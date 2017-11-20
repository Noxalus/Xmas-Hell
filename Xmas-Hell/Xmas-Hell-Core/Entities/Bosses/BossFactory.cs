using System;
using BulletML;
using XmasHell.Background;

namespace XmasHell.Entities.Bosses
{
    public enum BossType
    {
        Unknown,
        Debug,
        XmasBall,
        XmasBell,
        XmasCandy,
        XmasSnowflake,
        XmasLog,
        XmasTree,
        XmasGift,
        XmasReindeer,
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
                    return new XmasTree.XmasTree(game, playerPositionDelegate);
                case BossType.XmasGift:
                    return new XmasGift.XmasGift(game, playerPositionDelegate);
                case BossType.XmasReindeer:
                    return new XmasReindeer.XmasReindeer(game, playerPositionDelegate);
                case BossType.XmasSnowman:
                    return new XmasSnowman.XmasSnowman(game, playerPositionDelegate);
                case BossType.XmasSanta:
                    return new XmasSanta.XmasSanta(game, playerPositionDelegate);
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
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
                case "reindeer":
                    return BossType.XmasReindeer;
                case "snowman":
                    return BossType.XmasSnowman;
                case "santa":
                    return BossType.XmasSanta;
                default:
                    return BossType.Unknown;
            }
        }

        public static BackgroundLevel BossTypeToBackgroundLevel(BossType bossType)
        {
            switch (bossType)
            {
                case BossType.Debug:
                    return BackgroundLevel.Level1;
                case BossType.XmasBall:
                    return BackgroundLevel.Level1;
                case BossType.XmasBell:
                    return BackgroundLevel.Level1;
                case BossType.XmasCandy:
                    return BackgroundLevel.Level1;
                case BossType.XmasSnowflake:
                    return BackgroundLevel.Level1;
                case BossType.XmasLog:
                    return BackgroundLevel.Level2;
                case BossType.XmasTree:
                    return BackgroundLevel.Level2;
                case BossType.XmasGift:
                    return BackgroundLevel.Level2;
                case BossType.XmasReindeer:
                    return BackgroundLevel.Level3;
                case BossType.XmasSnowman:
                    return BackgroundLevel.Level3;
                case BossType.XmasSanta:
                    return BackgroundLevel.Level4;
                default:
                    throw new ArgumentOutOfRangeException(nameof(bossType), bossType, null);
            }
        }
    }
}