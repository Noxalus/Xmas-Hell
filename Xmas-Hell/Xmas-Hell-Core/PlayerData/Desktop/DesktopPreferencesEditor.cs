#if LINUX
using System;
using System.Collections.Generic;

namespace XmasHell.PlayerData.Desktop
{
    class DesktopPreferencesEditor : IPreferencesEditor
    {
        public DesktopPreferencesEditor()
        {
        }

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
    }
}
#endif