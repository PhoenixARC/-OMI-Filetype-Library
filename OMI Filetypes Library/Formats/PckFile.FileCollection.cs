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
        private ArrayList duplicates = new ArrayList();

        public int Count => _files.Count;

        public bool IsReadOnly => false;

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
            get => _files[index] as PckFile.FileData;
            set
            {
                _ = value ?? throw new ArgumentNullException(nameof(value));
                _files[index] = value;
            }
        }

        public void Add(PckFile.FileData value)
        {
            var key = GetStoreKey(value.Filename, value.Filetype);
            if (_files.Contains(key))
            {
                if (this[value.Filename, value.Filetype].Equals(value))
                {
                    Debug.WriteLine($"Duplicate file: '{value.Filename}'", category: $"{nameof(FileCollection)}.{nameof(Add)}");
                    Debug.WriteLine($"Merging '{value.Filename}' Properties", category: $"{nameof(FileCollection)}.{nameof(Add)}");
                    var first = this[value.Filename, value.Filetype];
                    first.Properties.Merge(value.Properties);
                    return;
                }
                var markedKey = key + value.GetHashCode().ToString();
                Debug.WriteLine($"'{key}' is already present! Adding it as '{markedKey}'",
                    category: $"{nameof(FileCollection)}.{nameof(Add)}");
                duplicates.Add(markedKey);

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

        public bool Contains(string filename, PckFile.FileData.FileType filetype)
        {
            return _files.Contains(GetStoreKey(filename, filetype));
        }

        private object GetStoreKey(string key, PckFile.FileData.FileType fileType)
        {
            return $"{key}_{fileType}";
        }

        private object GetStoreKey(PckFile.FileData item)
        {
            return GetStoreKey(item.Filename, item.Filetype);
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
            _files.Insert(index, GetStoreKey(item), item);
        }

        private bool Remove(string key, PckFile.FileData.FileType filetype)
        {
            if (Contains(key, filetype))
            {
                _files.Remove(GetStoreKey(key, filetype));
                return true;
            }
            return false;
        }

        public bool Remove(PckFile.FileData item)
        {
            if (item is not null)
                return Remove(item.Filename, item.Filetype);
            return false;
        }

        public void RemoveDuplicates()
        {
            foreach (var key in duplicates)
            {
                _files.Remove(key);
            }
            duplicates.Clear();
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
            Remove(this[index]);
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
