#if LINUX
using System;
using System.Collections.Generic;

namespace XmasHell.PlayerData.Desktop
{
    class DesktopPreferences : IPreferences
    {
        public bool Contains(string key)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public bool GetBoolean(string key, bool defValue)
        {
            throw new NotImplementedException();
        }

        public float GetFloat(string key, float defValue)
        {
            throw new NotImplementedException();
        }

        public int GetInt(string key, int defValue)
        {
            throw new NotImplementedException();
        }

        public long GetLong(string key, long defValue)
        {
            throw new NotImplementedException();
        }

        public string GetString(string key, string defValue)
        {
            throw new NotImplementedException();
        }

        public ICollection<string> GetStringSet(string key, ICollection<string> defValues)
        {
            throw new NotImplementedException();
        }

        private DesktopPreferencesEditor _editor;

        public IPreferencesEditor Editor()
        {
            return _editor;
        }

        public DesktopPreferences()
        {
            _editor = new DesktopPreferencesEditor();
        }
    }
}
#endif