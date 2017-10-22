#if ANDROID
using System;
using System.Collections.Generic;
using Android.Content;

namespace XmasHell.PlayerData.Android
{
    class AndroidPreferencesEditor : IPreferencesEditor
    {
        private ISharedPreferencesEditor _editor;

        public AndroidPreferencesEditor(ISharedPreferencesEditor androidPreferencesEditor)
        {
            _editor = androidPreferencesEditor;
        }

        #region Wrapper
        public IntPtr Handle
        {
            get
            {
                return _editor.Handle;
            }
        }

        public void Apply()
        {
            _editor.Apply();
        }

        public ISharedPreferencesEditor Clear()
        {
            return _editor.Clear();
        }

        public bool Commit()
        {
            return _editor.Commit();
        }

        public void Dispose()
        {
            _editor.Dispose();
        }

        public ISharedPreferencesEditor PutBoolean(string key, bool value)
        {
            return _editor.PutBoolean(key, value);
        }

        public ISharedPreferencesEditor PutFloat(string key, float value)
        {
            return _editor.PutFloat(key, value);
        }

        public ISharedPreferencesEditor PutInt(string key, int value)
        {
            return _editor.PutInt(key, value);
        }

        public ISharedPreferencesEditor PutLong(string key, long value)
        {
            return _editor.PutLong(key, value);
        }

        public ISharedPreferencesEditor PutString(string key, string value)
        {
            return _editor.PutString(key, value);
        }

        public ISharedPreferencesEditor PutStringSet(string key, ICollection<string> values)
        {
            return _editor.PutStringSet(key, values);
        }

        public ISharedPreferencesEditor Remove(string key)
        {
            return _editor.Remove(key);
        }
        #endregion
    }
}
#endif