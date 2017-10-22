#if LINUX
using System;
using System.Collections.Generic;

namespace XmasHell.PlayerData.Desktop
{
    class DesktopPreferences : IPreferences
    {
        public bool Contains(string key)
        {
            return _editor.Contains(key);
        }

        public void Dispose()
        {
        }

        public bool GetBoolean(string key, bool defValue)
        {
            return _editor.GetBoolean(key, defValue);
        }

        public float GetFloat(string key, float defValue)
        {
            return _editor.GetFloat(key, defValue);
        }

        public int GetInt(string key, int defValue)
        {
            return _editor.GetInt(key, defValue);
        }

        public long GetLong(string key, long defValue)
        {
            return _editor.GetLong(key, defValue);
        }

        public string GetString(string key, string defValue)
        {
            return _editor.GetString(key, defValue);
        }

        public ICollection<string> GetStringSet(string key, ICollection<string> defValues)
        {
            return _editor.GetStringSet(key, defValues);
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