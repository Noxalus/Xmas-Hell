#if LINUX
using System;
using System.Collections.Generic;

namespace XmasHell.PlayerData.Desktop
{
    class DesktopPreferencesEditor : IPreferencesEditor
    {
        private Dictionary<string, int> _intDictionnary = new Dictionary<string, int>();
        private Dictionary<string, bool> _boolDictionnary = new Dictionary<string, bool>();
        private Dictionary<string, float> _floatDictionnary = new Dictionary<string, float>();
        private Dictionary<string, long> _longDictionnary = new Dictionary<string, long>();
        private Dictionary<string, string> _stringDictionnary = new Dictionary<string, string>();
        private Dictionary<string, ICollection<string>> _stringSetDictionnary = new Dictionary<string, ICollection<string>>();

        public DesktopPreferencesEditor()
        {
            Load();
        }

        private void Load()
        {
            // TODO: Load preferences.xml file to fill dictionaries
        }

        public bool Contains(string key)
        {
            return false;
        }

        public bool GetBoolean(string key, bool defValue)
        {
            var value = defValue;

            if (_boolDictionnary.ContainsKey(key))
                value = _boolDictionnary[key];

            return value;
        }

        public float GetFloat(string key, float defValue)
        {
            var value = defValue;

            if (_floatDictionnary.ContainsKey(key))
                value = _floatDictionnary[key];

            return value;
        }

        public int GetInt(string key, int defValue)
        {
            var value = defValue;

            if (_intDictionnary.ContainsKey(key))
                value = _intDictionnary[key];

            return value;
        }

        public long GetLong(string key, long defValue)
        {
            var value = defValue;

            if (_longDictionnary.ContainsKey(key))
                value = _longDictionnary[key];

            return value;
        }

        public string GetString(string key, string defValue)
        {
            var value = defValue;

            if (_stringDictionnary.ContainsKey(key))
                value = _stringDictionnary[key];

            return value;
        }

        public ICollection<string> GetStringSet(string key, ICollection<string> defValue)
        {
            var value = defValue;

            if (_stringSetDictionnary.ContainsKey(key))
                value = _stringSetDictionnary[key];

            return value;
        }

        #region Common methods
        public void Apply()
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor Clear()
        {
            throw new NotImplementedException();
        }

        public bool Commit()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor PutBoolean(string key, bool value)
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor PutFloat(string key, float value)
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor PutInt(string key, int value)
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor PutLong(string key, long value)
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor PutString(string key, string value)
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor PutStringSet(string key, ICollection<string> values)
        {
            throw new NotImplementedException();
        }

        public IPreferencesEditor Remove(string key)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
#endif