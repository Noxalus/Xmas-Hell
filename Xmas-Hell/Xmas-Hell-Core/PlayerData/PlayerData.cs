﻿using XmasHell.Entities.Bosses;
using System;

namespace XmasHell.PlayerData
{
    public class PlayerData
    {
        private IPreferences _preferences;

        public PlayerData(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public void DeathCounter(int value)
        {
            _preferences.Editor().PutInt("deathCounter", value);
            _preferences.Editor().Apply();
        }

        public int DeathCounter()
        {
            return _preferences.GetInt("deathCounter", 0);
        }

        // Boss specific data

        public void BossBeatenCounter(BossType type, int value)
        {
            _preferences.Editor().PutInt("BossBeatenCounter-" + type.ToString(), value);
            _preferences.Editor().Apply();
        }

        public int BossBeatenCounter(BossType type)
        {
            return _preferences.GetInt("BossBeatenCounter-" + type.ToString(), 0);
        }

        public void BossAttempts(BossType type, int value)
        {
            _preferences.Editor().PutInt("BossAttempts-" + type.ToString(), value);
            _preferences.Editor().Apply();
        }

        public int BossAttempts(BossType type)
        {
            return _preferences.GetInt("BossAttempts-" + type.ToString(), 0);
        }

        public void BossBestTime(BossType type, TimeSpan value)
        {
            _preferences.Editor().PutString("BossBestTime-" + type.ToString(), value.TotalSeconds.ToString());
            _preferences.Editor().Apply();
        }

        public TimeSpan BossBestTime(BossType type)
        {
            return TimeSpan.FromSeconds(double.Parse(_preferences.GetString("BossBestTime-" + type.ToString(), "0")));
        }

        public void BossPlayTime(BossType type, TimeSpan value)
        {
            _preferences.Editor().PutString("BossPlayTime-" + type.ToString(), value.TotalSeconds.ToString());
            _preferences.Editor().Apply();
        }

        public TimeSpan BossPlayTime(BossType type)
        {
            return TimeSpan.FromSeconds(double.Parse(_preferences.GetString("BossPlayTime-" + type.ToString(), "0")));
        }
    }
}
