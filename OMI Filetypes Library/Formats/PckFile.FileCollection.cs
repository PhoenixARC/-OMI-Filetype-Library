using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace OMI.Formats.Pck
{
    public class FileCollection : IList<PckFile.FileData>
    {
        private OrderedDictionary _files = new OrderedDictionary();

        public PckFile.FileData this[string key]
        {
            get
            {
                if (!_files.Contains(key))
                {
                    throw new KeyNotFoundException(key);
                }
                return (PckFile.FileData)_files[key];
            }
            set
            {
                _ = value ?? throw new ArgumentNullException(nameof(value));
                _files[key] = value;
            }
        }

        public PckFile.FileData this[int index]
        {
            get => (PckFile.FileData)_files[index];
            set
            {
                _ = value ?? throw new ArgumentNullException(nameof(value));
                _files[index] = value;
            }
        }

        public int Count => _files.Count;

        public bool IsReadOnly => false;

        public void Add(string key, PckFile.FileData value)
        {
            if (_files.Contains(key))
            {
                var markedKey = key + value.GetHashCode().ToString();
                Debug.WriteLine($"'{key}' is already present! Adding it as '{markedKey}'", category: $"{nameof(FileCollection)}.{nameof(Add)}");
                _files.Add(markedKey, value);
                return;
            }
            _files.Add(key, value);
        }

        public void Add(PckFile.FileData item)
        {
            Add(item.Filename, item);
        }

        public void Clear()
        {
            _files.Clear();
        }

        public bool Contains(PckFile.FileData item)
        {
            return Contains(item.Filename);
        }

        public bool Contains(string key)
        {
            return _files.Contains(key);
        }

        public void CopyTo(PckFile.FileData[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<PckFile.FileData> GetEnumerator()
        {
            return _files.Values.Cast<PckFile.FileData>().GetEnumerator();
        }

        public int IndexOf(PckFile.FileData key)
        {
            for (int i = 0; i < _files.Count; i++)
            {
                object key2 = ((DictionaryEntry)_files[i]).Key;
                if (key2.Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, PckFile.FileData item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));
            _files.Insert(index, item.Filename, item);
        }

        public bool Remove(string key)
        {
            if (Contains(key))
            {
                _files.Remove(key);
                return true;
            }
            return false;
        }

        public bool Remove(PckFile.FileData item)
        {
            return Remove(item.Filename);
        }

        public void RemoveAll(Predicate<PckFile.FileData> value)
        {
            List<string> values = new List<string>();
            foreach (PckFile.FileData item in _files.Values)
            {
                if (value(item))
                    values.Add(item.Filename);
            }

            values.ForEach(v => Remove(v));
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, out PckFile.FileData value)
        {
            if (Contains(key))
            {
                value = this[key];
                return true;
            }
            value = null;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _files.GetEnumerator();
        }
    }
}
