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
            _preferences.Editor().PutInt("DeathCounter", value);
            _preferences.Editor().Apply();
        }

        public int DeathCounter()
        {
            return _preferences.GetInt("DeathCounter", 0);
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

        public void DeathCounter(BossType type, int value)
        {
            _preferences.Editor().PutInt("DeathCounter-" + type.ToString(), value);
            _preferences.Editor().Apply();
        }

        public int DeathCounter(BossType type)
        {
            return _preferences.GetInt("DeathCounter-" + type.ToString(), 0);
        }

        public void BossBestTime(BossType type, TimeSpan value)
        {
            _preferences.Editor().PutFloat("BossBestTime-" + type.ToString(), (float)value.TotalMilliseconds);
            _preferences.Editor().Apply();
        }

        public TimeSpan BossBestTime(BossType type)
        {
            return TimeSpan.FromMilliseconds(_preferences.GetFloat("BossBestTime-" + type.ToString(), 0));
        }

        public void BossPlayTime(BossType type, TimeSpan value)
        {
            _preferences.Editor().PutFloat("BossPlayTime-" + type.ToString(), (float)value.TotalSeconds);
            _preferences.Editor().Apply();
        }

        public TimeSpan BossPlayTime(BossType type)
        {
            return TimeSpan.FromSeconds(_preferences.GetFloat("BossPlayTime-" + type.ToString(), 0));
        }
    }
}
