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

        public PckFile.FileData this[string key, PckFile.FileData.FileType type]
        {
            get
            {
                var storeKey = GetStoreKey(key, type);
                if (!_files.Contains(storeKey))
                {
                    throw new KeyNotFoundException(storeKey.ToString());
                }
                return (PckFile.FileData)_files[storeKey];
            }
            set
            {
                _ = value ?? throw new ArgumentNullException(nameof(value));
                _files[GetStoreKey(key, type)] = value;
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

        public void Add(PckFile.FileData value)
        {
            var key = GetStoreKey(value.Filename, value.Filetype);
            if (_files.Contains(key))
            {
                var markedKey = key + value.GetHashCode().ToString();
                Debug.WriteLine($"'{key}' is already present! Adding it as '{markedKey}'", category: $"{nameof(FileCollection)}.{nameof(Add)}");
                _files.Add(markedKey, value);
                return;
            }
            _files.Add(key, value);
        }

        public void Clear()
        {
            _files.Clear();
        }

        public bool Contains(PckFile.FileData item)
        {
            return Contains(item.Filename, item.Filetype);
        }

        public bool Contains(string key, PckFile.FileData.FileType fileType)
        {
            return _files.Contains(GetStoreKey(key, fileType));
        }

        private object GetStoreKey(string key, PckFile.FileData.FileType fileType)
        {
            return $"{key}_{fileType}";
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

        private bool Remove(string key, PckFile.FileData.FileType fileType)
        {
            if (Contains(key, fileType))
            {
                _files.Remove(key);
                return true;
            }
            return false;
        }

        public bool Remove(PckFile.FileData item)
        {
            return Remove(item.Filename, item.Filetype);
        }

        public void RemoveAll(Predicate<PckFile.FileData> value)
        {
            var valuesToRemove = new List<PckFile.FileData>();
            foreach (PckFile.FileData item in _files.Values)
            {
                if (value(item))
                    valuesToRemove.Add(item);
            }
            valuesToRemove.ForEach(v => Remove(v));
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(string key, PckFile.FileData.FileType fileType, out PckFile.FileData value)
        {
            if (Contains(key, fileType))
            {
                value = this[key, fileType];
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
