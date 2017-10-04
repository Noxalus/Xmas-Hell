using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace XmasHell.PlayerData
{
    public static class StaticClassSerializer
    {
        public static bool Save(Type staticClass, string fileName)
        {
            try
            {
                FieldInfo[] fields = staticClass.GetFields(BindingFlags.Static | BindingFlags.Public);
                var data = new object[fields.Length, 2];

                int i = 0;
                foreach (FieldInfo field in fields)
                {
                    data[i, 0] = field.Name;
                    data[i, 1] = field.GetValue(null);
                    i++;
                }

#if ANDROID
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fileName = Path.Combine(path, fileName);
#endif

                FileStream file = File.Open(fileName, FileMode.Create);

                IFormatter formatter = new BinaryFormatter();

                formatter.Serialize(file, data);
                file.Close();
                return true;
            }
            catch(Exception ex)
            {
                Debug.Print(ex.Message);

                return false;
            }
        }

        public static bool Load(Type staticClass, string fileName)
        {
            try
            {
                FieldInfo[] fields = staticClass.GetFields(BindingFlags.Static | BindingFlags.Public);

#if ANDROID
                string path = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                fileName = Path.Combine(path, fileName);
#endif

                Stream file = File.Open(fileName, FileMode.Open);

                IFormatter formatter = new BinaryFormatter();

                var data = formatter.Deserialize(file) as object[,];

                file.Close();

                if (data != null && data.GetLength(0) != fields.Length) return false;

                int i = 0;
                foreach (FieldInfo field in fields)
                {
                    if (data != null && field.Name == (data[i, 0] as string))
                    {
                        field.SetValue(null, data[i, 1]);
                    }

                    i++;
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                return false;
            }
        }
    }
}
