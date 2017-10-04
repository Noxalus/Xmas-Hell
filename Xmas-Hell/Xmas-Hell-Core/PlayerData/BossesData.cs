using System;

namespace XmasHell.PlayerData
{
    struct BossData
    {
        string name;
        bool beaten;
        TimeSpan timeSpent;
        TimeSpan bestScore;
        int deathCounter;

        public static implicit operator BossData(string name)
        {
            return new BossData() {
                name = name,
                beaten = false,
                timeSpent = TimeSpan.Zero,
                bestScore = TimeSpan.Zero,
                deathCounter = 0
            };
        }
    }

    static class BossesData
    {
        public static BossData XmasBall = "XmasBall";
        public static BossData XmasBell = "XmasBell";
        public static BossData XmasSnowflake = "XmasSnowflake";
        public static BossData XmasCandy = "XmasCandy";
        public static BossData XmasGift = "XmasGift";
        public static BossData XmasLog = "XmasLog";
        public static BossData XmasTree = "XmasTree";
        public static BossData XmasReindeer = "XmasReindeer";
        public static BossData XmasSnowman = "XmasSnowman";
        public static BossData XmasSanta = "XmasSanta";
    }
}
