using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;

namespace OMI.Formats.Pck
{
    public class FileCollection : IList<PckFileData>
    {
        private OrderedDictionary _files = new OrderedDictionary();
        private ArrayList duplicates = new ArrayList();

        public int Count => _files.Count;

        public bool IsReadOnly => false;

        public PckFileData this[string filename, PckFileType type]
        {
            get
            {
                var storeKey = GetStorageKey(filename, type);
                if (!_files.Contains(storeKey))
                {
                    throw new KeyNotFoundException(storeKey.ToString());
                }
                return (PckFileData)_files[storeKey];
            }
            set
            {
                _ = value ?? throw new ArgumentNullException(nameof(value));
                _files[GetStorageKey(filename, type)] = value;
            }
        }

        public PckFileData this[int index]
        {
            get => _files[index] as PckFileData;
            set
            {
                _ = value ?? throw new ArgumentNullException(nameof(value));
                _files[index] = value;
            }
        }

        public void Add(PckFileData value)
        {
            var key = GetStorageKey(value.Filename, value.Filetype);
            if (_files.Contains(key))
            {
                if (this[value.Filename, value.Filetype].Equals(value))
                {
                    Debug.WriteLine($"Duplicate file: '{value.Filename}'", category: $"{nameof(FileCollection)}.{nameof(Add)}");
                    Debug.WriteLine($"Merging '{value.Filename}' Properties", category: $"{nameof(FileCollection)}.{nameof(Add)}");
                    PckFileData first = GetFile(value.Filename, value.Filetype);
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

        internal PckFileData GetFile(string filename, PckFileType filetype)
        {
            return _files[GetStorageKey(filename, filetype)] as PckFileData;
        }

        public void Clear()
        {
            _files.Clear();
        }

        public bool Contains(PckFileData item)
        {
            return Contains(item.Filename, item.Filetype);
        }

        public bool Contains(string filename, PckFileType filetype)
        {
            return _files.Contains(GetStorageKey(filename, filetype));
        }

        public bool Contains(PckFileType filetype)
        {
            foreach (var file in _files.Values.Cast<PckFileData>())
            {
                if (file.Filetype == filetype)
                    return true;
            }
            return false;
        }

        private object GetStorageKey(string key, PckFileType fileType)
        {
            return $"{key}_{fileType}";
        }

        private object GetStoreKey(PckFileData item)
        {
            return GetStorageKey(item.Filename, item.Filetype);
        }

        public void CopyTo(PckFileData[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<PckFileData> GetEnumerator()
        {
            return _files.Values.Cast<PckFileData>().GetEnumerator();
        }

        public int IndexOf(PckFileData key)
        {
            for (int i = 0; i < _files.Count; i++)
            {
                object key2 = _files[i];
                if (key2.Equals(key))
                {
                    return i;
                }
            }
            return -1;
        }

        public void Insert(int index, PckFileData item)
        {
            _ = item ?? throw new ArgumentNullException(nameof(item));
            _files.Insert(index, GetStoreKey(item), item);
        }

        internal bool Remove(string filename, PckFileType filetype)
        {
            if (Contains(filename, filetype))
            {
                _files.Remove(GetStorageKey(filename, filetype));
                return true;
            }
            return false;
        }

        public bool Remove(PckFileData item)
        {
            if (item is not null)
            {
                item.SetEvents(null, null, null);
                return Remove(item.Filename, item.Filetype);
            }
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

        public void RemoveAll(Predicate<PckFileData> value)
        {
            var valuesToRemove = new List<PckFileData>();
            foreach (PckFileData item in _files.Values)
            {
                if (value(item))
                    valuesToRemove.Add(item);
            }
            valuesToRemove.ForEach(v => Remove(v));
        }

        public void RemoveAt(int index)
        {
            _files.RemoveAt(index);
        }

        internal bool TryGetValue(string key, PckFileType fileType, out PckFileData value)
        {
            if (Contains(key, fileType))
            {
                value = GetFile(key, fileType);
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
