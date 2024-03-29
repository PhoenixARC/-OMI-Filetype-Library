﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace OMI.Formats.Pck
{
    public class PckFileData : IEquatable<PckFileData>
    {
        public string Filename
        {
            get => filename;
            set
            {
                string newFilename = value.Replace('\\', '/');
                OnFilenameChanging?.Invoke(this, newFilename);
                filename = newFilename;
            }
        }
        public PckFileType Filetype
        {
            get => filetype;
            set
            {
                var newValue = value;
                OnFiletypeChanging?.Invoke(this, newValue);
                filetype = newValue;
            }
        }

        internal delegate void OnFilenameChangingDelegate(PckFileData _this, string newFilename);
        internal delegate void OnFiletypeChangingDelegate(PckFileData _this, PckFileType newFiletype);
        internal delegate void OnMoveDelegate(PckFileData _this);

        public byte[] Data => _data;
        public int Size => _data?.Length ?? 0;
        internal PckFileProperties Properties { get; } = new PckFileProperties();

        private string filename;
        private PckFileType filetype;
        private OnFilenameChangingDelegate OnFilenameChanging;
        private OnFiletypeChangingDelegate OnFiletypeChanging;
        private OnMoveDelegate OnMove;
        private byte[] _data = new byte[0];

        internal PckFileData(string filename, PckFileType filetype,
            OnFilenameChangingDelegate onFilenameChanging, OnFiletypeChangingDelegate onFiletypeChanging,
            OnMoveDelegate onMove)
            : this(filename, filetype)
        {
            SetEvents(onFilenameChanging, onFiletypeChanging, onMove);
        }

        public PckFileData(string filename, PckFileType filetype)
        {
            Filetype = filetype;
            Filename = filename;
        }

        internal PckFileData(string filename, PckFileType filetype, int dataSize) : this(filename, filetype)
        {
            _data = new byte[dataSize];
        }

        internal bool HasEventsSet()
        {
            return OnFilenameChanging != null && OnFiletypeChanging != null && OnMove != null;
        }

        internal void SetEvents(OnFilenameChangingDelegate onFilenameChanging, OnFiletypeChangingDelegate onFiletypeChanging, OnMoveDelegate onMove)
        {
            OnFilenameChanging = onFilenameChanging;
            OnFiletypeChanging = onFiletypeChanging;
            OnMove = onMove;
        }

        public void SetData(byte[] data)
        {
            _data = data;
        }

        public bool Equals(PckFileData other)
        {
            var hasher = MD5.Create();
            var thisHash = BitConverter.ToString(hasher.ComputeHash(Data));
            var otherHash = BitConverter.ToString(hasher.ComputeHash(other.Data));
            return Filename.Equals(other.Filename) &&
                Filetype.Equals(other.Filetype) &&
                Size.Equals(other.Size) &&
                thisHash.Equals(otherHash);
        }

        public int PropertyCount => Properties.Count;

        public void AddProperty(KeyValuePair<string, string> property) => Properties.Add(property);
        
        public void AddProperty(string name, string value) => Properties.Add(name, value);

        public void AddProperty<T>(string name, T value) => Properties.Add(name, value);

        public void RemoveProperty(string name) => Properties.Remove(name);
        
        public bool RemoveProperty(KeyValuePair<string, string> property) => Properties.Remove(property);
        
        public void RemoveProperties(string name) => Properties.RemoveAll(p => p.Key == name);

        public void ClearProperties() => Properties.Clear();

        public bool HasProperty(string property) => Properties.Contains(property);

        public int GetPropertyIndex(KeyValuePair<string, string> property) => Properties.IndexOf(property);

        public string GetProperty(string name) => Properties.GetPropertyValue(name);
        
        public T GetProperty<T>(string name, Func<string, T> func) => Properties.GetPropertyValue(name, func);

        public bool TryGetProperty(string name, out string value) => Properties.TryGetProperty(name, out value);

        public KeyValuePair<string, string>[] GetMultipleProperties(string property) => Properties.GetProperties(property);

        public IReadOnlyList<KeyValuePair<string, string>> GetProperties() => Properties.AsReadOnly();

        public void SetProperty(int index, KeyValuePair<string, string> property) => Properties[index] = property;
        
        public void SetProperty(string name, string value) => Properties.SetProperty(name, value);

        public override bool Equals(object obj)
        {
            return obj is PckFileData other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 953938382;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Filename);
            hashCode = hashCode * -1521134295 + Filetype.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(Data);
            hashCode = hashCode * -1521134295 + Size.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<PckFileProperties>.Default.GetHashCode(Properties);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(filename);
            hashCode = hashCode * -1521134295 + EqualityComparer<byte[]>.Default.GetHashCode(_data);
            return hashCode;
        }

        internal void Move()
        {
            OnMove?.Invoke(this);
        }
    }
}
