using System;
using System.Collections.Generic;
using System.Linq;

namespace OMI.Formats.Pck
{
    using PropertyValueType = KeyValuePair<string, string>;

    public class PckFileProperties : List<PropertyValueType>
    {
        public void Add<T>(string key, T value)
        {
            Add(new PropertyValueType(key, value.ToString()));
        }

        public void Add<T>((string key, T value) property)
        {
            Add(new PropertyValueType(property.key, property.value.ToString()));
        }

        public void Add(string key, string value)
        {
            Add(new PropertyValueType(key, value));
        }

        public bool Contains(string property)
        {
            return HasProperty(property);
        }

        public bool HasProperty(string property)
        {
            return GetProperty(property).Key is not null;
        }

        public PropertyValueType GetProperty(string property)
        {
            return this.FirstOrDefault(p => p.Key.Equals(property))!;
        }

        public T GetPropertyValue<T>(string property, Func<string, T> func)
        {
            return func(GetPropertyValue(property));
        }

        public string GetPropertyValue(string property)
        {
            return GetProperty(property).Value;
        }

        public PropertyValueType[] GetProperties(string property)
        {
            return FindAll(p => p.Key == property).ToArray();
        }

        public bool HasMoreThanOneOf(string property)
        {
            return GetProperties(property).Length > 1;
        }

        public void Merge(PckFileProperties other)
        {
            AddRange(other);
        }

        public void SetProperty(string property, string value)
        {
            if (HasProperty(property))
            {
                this[IndexOf(GetProperty(property))] = new PropertyValueType(property, value);
                return;
            }
            Add(new PropertyValueType(property, value));
        }

    }
}
