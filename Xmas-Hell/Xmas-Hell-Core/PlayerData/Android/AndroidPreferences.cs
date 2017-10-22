#if ANDROID
using System;
using System.Collections.Generic;
using Android.Content;

namespace XmasHell.PlayerData.Android
{
    class AndroidPreferences : IPreferences
    {
        #region Wrapper
        public IDictionary<string, object> All
        {
            get
            {
                return _preferences.All;
            }
        }

        public IntPtr Handle
        {
            get
            {
                return _preferences.Handle;
            }
        }

        public bool Contains(string key)
        {
            return _preferences.Contains(key);
        }

        public void Dispose()
        {
            _preferences.Dispose();
        }

        public ISharedPreferencesEditor Edit()
        {
            return _preferences.Edit();
        }

        public bool GetBoolean(string key, bool defValue)
        {
            return _preferences.GetBoolean(key, defValue);
        }

        public float GetFloat(string key, float defValue)
        {
            return _preferences.GetFloat(key, defValue);
        }

        public int GetInt(string key, int defValue)
        {
            return _preferences.GetInt(key, defValue);
        }

        public long GetLong(string key, long defValue)
        {
            return _preferences.GetLong(key, defValue);
        }

        public string GetString(string key, string defValue)
        {
            return _preferences.GetString(key, defValue);
        }

        public ICollection<string> GetStringSet(string key, ICollection<string> defValues)
        {
            return _preferences.GetStringSet(key, defValues);
        }

        public void RegisterOnSharedPreferenceChangeListener(ISharedPreferencesOnSharedPreferenceChangeListener listener)
        {
            _preferences.RegisterOnSharedPreferenceChangeListener(listener);
        }

        public void UnregisterOnSharedPreferenceChangeListener(ISharedPreferencesOnSharedPreferenceChangeListener listener)
        {
            _preferences.UnregisterOnSharedPreferenceChangeListener(listener);
        }
        #endregion

        private ISharedPreferences _preferences;
        private IPreferencesEditor _editor;

        public IPreferencesEditor Editor()
        {
            return _editor;
        }

        public AndroidPreferences(ISharedPreferences androidPreferences)
        {
            _preferences = androidPreferences;
            _editor = new AndroidPreferencesEditor(_preferences.Edit());
        }
    }
}
#endif