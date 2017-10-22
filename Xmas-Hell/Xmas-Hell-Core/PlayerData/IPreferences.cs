using System.Collections.Generic;

#if ANDROID
using Android.Content;
#endif

namespace XmasHell.PlayerData
{
#if ANDROID
    public interface IPreferences : ISharedPreferences
    {
        IPreferencesEditor Editor();
    }
#elif LINUX
    public interface IPreferences
    {
        IPreferencesEditor Editor();

        bool Contains(string key);
        void Dispose();
        bool GetBoolean(string key, bool defValue);
        float GetFloat(string key, float defValue);
        int GetInt(string key, int defValue);
        long GetLong(string key, long defValue);
        string GetString(string key, string defValue);
        ICollection<string> GetStringSet(string key, ICollection<string> defValues);
    }
#endif
}
