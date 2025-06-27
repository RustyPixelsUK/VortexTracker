using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace VortexTracker
{
    public class IniFile
    {
        private Dictionary<string, Dictionary<string, string>> _data;
        private string _fileName = null;

        public IniFile()
        {
            _data = new Dictionary<string, Dictionary<string, string>>();
        }

        public IniFile(string fileName)
        {
            _fileName = fileName;
            _data = new Dictionary<string, Dictionary<string, string>>();
            LoadFromFile(fileName);
        }

        public IniFile(Stream stream)
        {
            _data = new Dictionary<string, Dictionary<string, string>>();
            LoadFromStream(stream);
        }

        private void LoadFromFile(string fileName)
        {
            using (var stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Read))
            {
                LoadFromStream(stream);
            }
        }

        private void LoadFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
            {
                string line;
                string currentSection = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();

                    // Ignore comments
                    if (line.StartsWith("#"))
                        continue;

                    // Section
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        currentSection = line.Substring(1, line.Length - 2).Trim();
                        if (!_data.ContainsKey(currentSection))
                        {
                            _data[currentSection] = new Dictionary<string, string>();
                        }
                    }
                    // Key-value pair
                    else if (currentSection != null && line.Contains("="))
                    {
                        var parts = line.Split(new[] { '=' }, 2);

                        if (parts.Length == 2)
                        {
                            var key = parts[0].Trim();
                            var value = parts[1].Trim();

                            _data[currentSection][key] = value;
                        }
                    }
                }
            }
        }

        public void SaveToFile()
        {
            SaveToFile(_fileName);
        }

        public void SaveToFile(string fileName)
        {
            if (_fileName == null)
                return;

            using (var stream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                SaveToStream(stream);
            }
        }

        public void SaveToStream(Stream stream)
        {
            using (var writer = new StreamWriter(stream))
            {
                foreach (var section in _data)
                {
                    writer.WriteLine($"[{section.Key}]");
                    foreach (var kvp in section.Value)
                        writer.WriteLine($"{kvp.Key}={kvp.Value}");
                }
            }
        }

        public void AddSection(string section)
        {
            if (_data.ContainsKey(section))
                return;

            _data[section] = new Dictionary<string, string>();
        }

        public void RemoveSection(string section)
        {
            if (!_data.ContainsKey(section))
                return;

            _data.Remove(section);
        }

        public void SetValue(string section, string key, string value)
        {
            SetValue<string>(section, key, value);
        }

        public void SetValue<T>(string section, string key, T value)
        {
            AddSection(section);
            _data[section][key] = value?.ToString() ?? string.Empty;
        }

        public string GetValue(string section, string key, string defaultValue = null)
        {
            return GetValue<string>(section, key, defaultValue);
        }

        public T GetValue<T>(string section, string key, T defaultValue = default)
        {
            if (_data.ContainsKey(section) && _data[section].ContainsKey(key))
            {
                string value = _data[section][key];
                try
                {
                    if (typeof(T) == typeof(bool))
                    {
                        if (value == "0") return (T)(object)false;
                        if (value == "1") return (T)(object)true;
                    }

                    TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                    if (converter != null && converter.CanConvertFrom(typeof(string)))
                    {
                        return (T)converter.ConvertFromString(value);
                    }
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                catch
                {
                    return defaultValue;
                }
            }
            return defaultValue;
        }

        public bool ContainsKey(string section, string key)
        {
            if (!_data.ContainsKey(section))
                return false;

            if (!_data[section].ContainsKey(key))
                return false;

            return true;
        }

        public void RemoveKey(string section, string key)
        {
            if (!ContainsKey(section, key))
                return;

            _data[section].Remove(key);
        }

        public IEnumerable<string> EnumerateSections()
        {
            return _data.Keys;
        }

        public IEnumerable<string> EnumerateKeys(string section)
        {
            if (_data.ContainsKey(section))
                return _data[section].Keys;

            return new List<string>();
        }
    }
}