#if LINUX
using System;
using System.Collections.Generic;
using System.Xml;

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

        private void ReadXmlElement<T>(XmlReader reader, Dictionary<string, T> dictionary, Func<string, T> parseFunction)
        {
            XmlReader inner = reader.ReadSubtree();

            while (inner.ReadToFollowing("element"))
            {
                reader.MoveToAttribute("key");
                var key = reader.Value;
                reader.MoveToAttribute("value");
                var value = reader.Value;

                dictionary.Add(key, parseFunction(value));
            }
        }

        private void Load()
        {
            try
            {
                using (XmlReader reader = XmlReader.Create("preferences.xml"))
                {
                    reader.ReadToFollowing("intDictionary");
                    ReadXmlElement(reader, _intDictionnary, int.Parse);
                    reader.ReadToFollowing("boolDictionary");
                    ReadXmlElement(reader, _boolDictionnary, bool.Parse);
                    reader.ReadToFollowing("floatDictionary");
                    ReadXmlElement(reader, _floatDictionnary, float.Parse);
                    reader.ReadToFollowing("longDictionary");
                    ReadXmlElement(reader, _longDictionnary, long.Parse);
                    reader.ReadToFollowing("stringDictionary");
                    ReadXmlElement(reader, _stringDictionnary, (s) => s);

                    // TODO: Handle string set
                };
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void WriteXmlElement<T>(XmlWriter xmlWriter, Dictionary<string, T> dictionary)
        {
            foreach (var pair in dictionary)
            {
                xmlWriter.WriteStartElement("element");
                xmlWriter.WriteAttributeString("key", pair.Key);
                xmlWriter.WriteAttributeString("value", pair.Value.ToString());
                xmlWriter.WriteEndElement();
            }
        }

        private void Save()
        {
            XmlWriterSettings ws = new XmlWriterSettings();
            ws.Indent = true;
            XmlWriter xmlWriter = XmlWriter.Create("preferences.xml", ws);

            xmlWriter.WriteStartDocument();

            xmlWriter.WriteStartElement("preferences");

            xmlWriter.WriteStartElement("intDictionary");
            WriteXmlElement(xmlWriter, _intDictionnary);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("boolDictionary");
            WriteXmlElement(xmlWriter, _boolDictionnary);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("floatDictionary");
            WriteXmlElement(xmlWriter, _floatDictionnary);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("longDictionary");
            WriteXmlElement(xmlWriter, _longDictionnary);
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("stringDictionary");
            WriteXmlElement(xmlWriter, _stringDictionnary);
            xmlWriter.WriteEndElement();

            foreach (var pair in _stringSetDictionnary)
            {
                xmlWriter.WriteStartElement("element");
                xmlWriter.WriteStartElement(pair.Key);

                foreach (var str in pair.Value)
                    xmlWriter.WriteString(str);

                xmlWriter.WriteEndElement();
                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
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
            Save();
        }

        public IPreferencesEditor Clear()
        {
            _intDictionnary.Clear();
            _boolDictionnary.Clear();
            _floatDictionnary.Clear();
            _longDictionnary.Clear();
            _stringDictionnary.Clear();
            _stringSetDictionnary.Clear();

            return this;
        }

        public bool Commit()
        {
            return false;
        }

        public void Dispose()
        {

        }

        public IPreferencesEditor PutBoolean(string key, bool value)
        {
            _boolDictionnary.Add(key, value);
            return this;
        }

        public IPreferencesEditor PutFloat(string key, float value)
        {
            _floatDictionnary.Add(key, value);
            return this;
        }

        public IPreferencesEditor PutInt(string key, int value)
        {
            _intDictionnary.Add(key, value);
            return this;
        }

        public IPreferencesEditor PutLong(string key, long value)
        {
            _floatDictionnary.Add(key, value);
            return this;
        }

        public IPreferencesEditor PutString(string key, string value)
        {
            _stringDictionnary.Add(key, value);
            return this;
        }

        public IPreferencesEditor PutStringSet(string key, ICollection<string> values)
        {
            _stringSetDictionnary.Add(key, values);
            return this;
        }

        public IPreferencesEditor Remove(string key)
        {
            if (_intDictionnary.ContainsKey(key))
                _intDictionnary.Remove(key);
            if (_floatDictionnary.ContainsKey(key))
                _floatDictionnary.Remove(key);
            if (_boolDictionnary.ContainsKey(key))
                _boolDictionnary.Remove(key);
            if (_longDictionnary.ContainsKey(key))
                _longDictionnary.Remove(key);
            if (_stringDictionnary.ContainsKey(key))
                _stringDictionnary.Remove(key);
            if (_stringSetDictionnary.ContainsKey(key))
                _stringSetDictionnary.Remove(key);

            return this;
        }
        #endregion
    }
}
#endif