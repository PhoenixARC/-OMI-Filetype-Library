using System;
using System.Collections.Generic;
using System.Linq;

namespace OMI.Formats.Pck
{
    using PropertyValueType = KeyValuePair<string, string>;

    internal class PckFileProperties : List<PropertyValueType>
    {
        internal void Add<T>(string key, T value)
        {
            Add(new PropertyValueType(key, value.ToString()));
        }

        internal void Add<T>((string key, T value) property)
        {
            Add(new PropertyValueType(property.key, property.value.ToString()));
        }

        internal void Add(string key, string value)
        {
            Add(new PropertyValueType(key, value));
        }

        internal bool TryGetProperty(string property, out string value)
        {
            if (Contains(property))
            {
                value = GetPropertyValue(property);
                return true;
            }
            value = null;
            return false;
        }

        internal bool Remove(string property)
        {
            if (!Contains(property))
                return false;
            int index = FindIndex(p => p.Key == property);
            RemoveAt(index);
            return true;
        }

        internal bool Contains(string property)
        {
            return Exists(p => p.Key == property);
        }

        internal PropertyValueType GetProperty(string property)
        {
            return this.FirstOrDefault(p => p.Key.Equals(property))!;
        }

        internal T GetPropertyValue<T>(string property, Func<string, T> func)
        {
            return func(GetPropertyValue(property));
        }

        internal string GetPropertyValue(string property)
        {
            return GetProperty(property).Value;
        }

        internal PropertyValueType[] GetProperties(string property)
        {
            return FindAll(p => p.Key == property).ToArray();
        }

        internal bool HasMoreThanOneOf(string property)
        {
            return GetProperties(property).Length > 1;
        }

        internal void Merge(PckFileProperties other)
        {
            AddRange(other);
        }

        internal void SetProperty(string property, string value)
        {
            if (Contains(property))
            {
                this[IndexOf(GetProperty(property))] = new PropertyValueType(property, value);
                return;
            }
            Add(new PropertyValueType(property, value));
        }

    }
}
