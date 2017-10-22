#if ANDROID
using Android.Content;
#endif

namespace XmasHell.PlayerData
{
#if ANDROID
    public interface IPreferencesEditor : ISharedPreferencesEditor
    {
    }
#else
    public interface IPreferencesEditor
    {
        void Apply();
        bool Commit();
        void Dispose();
        IPreferencesEditor Clear();
        IPreferencesEditor PutBoolean(string key, bool value);
        IPreferencesEditor PutFloat(string key, float value);
        IPreferencesEditor PutInt(string key, int value);
        IPreferencesEditor PutLong(string key, long value);
        IPreferencesEditor PutString(string key, string value);
        IPreferencesEditor PutStringSet(string key, System.Collections.Generic.ICollection<string> values);
        IPreferencesEditor Remove(string key);
    }
#endif
}
