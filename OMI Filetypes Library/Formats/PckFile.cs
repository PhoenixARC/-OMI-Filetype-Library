using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.Pck
{
    public class PckFile
    {
        public readonly int type;
        public const string XMLVersionString = "XMLVERSION";
        public bool HasVerionString => _hasVerionString;
        public int FileCount => Files.Count;

        private FileCollection Files { get; } = new FileCollection();
        private bool _hasVerionString = false;

        public PckFile(int type, bool hasVersionStr)
            : this(type)
        {
            SetVersion(hasVersionStr);
        }

        public PckFile(int type)
        {
            this.type = type;
        }

        public void SetVersion(bool enabled)
        {
            _hasVerionString = enabled;
        }

        public List<string> GetPropertyList()
        {
            var LUT = new List<string>();
            foreach (var file in Files)
            {
                file.Properties.ForEach(pair =>
                {
                    if (!LUT.Contains(pair.Key))
                        LUT.Add(pair.Key);
                });
            }
            return LUT;
        }

        /// <summary>
        /// Create and add new <see cref="PckAsset"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="assetType">Filetype</param>
        /// <returns>Added <see cref="PckAsset"/> object</returns>
        public PckAsset CreateNewFile(string filename, PckAssetType assetType)
        {
            var file = new PckAsset(filename, assetType);
            AddFile(file);
            return file;
        }

        /// <summary>
        /// Create, add and initialize new <see cref="PckAsset"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="assetType">Filetype</param>
        /// <returns>Initialized <see cref="PckAsset"/> object</returns>
        public PckAsset CreateNewFile(string filename, PckAssetType assetType, Func<byte[]> dataInitializier)
        {
            var file = CreateNewFile(filename, assetType);
            file.SetData(dataInitializier?.Invoke());
            return file;
        }

        /// <summary>
        /// Checks wether a file with <paramref name="filename"/> and <paramref name="assetType"/> exists
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="assetType">Type of the file <see cref="PckAsset.FileType"/></param>
        /// <returns>True when file exists, otherwise false </returns>
        public bool HasFile(string filename, PckAssetType assetType)
        {
            return Files.Contains(filename, assetType);
        }

        /// <summary>
        /// Gets the first file that Equals <paramref name="filename"/> and <paramref name="assetType"/>
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="assetType">Type of the file <see cref="PckAsset.FileType"/></param>
        /// <returns>FileData if found, otherwise null</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public PckAsset GetFile(string filename, PckAssetType assetType)
        {
            return Files.GetFile(filename, assetType);
        }

        /// <summary>
        /// Tries to get a file with <paramref name="filename"/> and <paramref name="assetType"/>.
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="assetType">Type of the file <see cref="PckAsset.FileType"/></param>
        /// <param name="file">If succeeded <paramref name="file"/> will be non-null, otherwise null</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public bool TryGetFile(string filename, PckAssetType assetType, out PckAsset file)
        {
            if (HasFile(filename, assetType))
            {
                file = GetFile(filename, assetType);
                return true;
            }
                file = null;
                return false;
            }

        private void OnPckFileNameChanging(PckAsset value, string newFilename)
        {
            if (value.Filename.Equals(newFilename))
                return;
            Files[newFilename, value.Type] = value;
            Files.RemoveKeyFromCollection(value);
        }

        private void OnPckAssetTypeChanging(PckAsset value, PckAssetType newAssetType)
        {
            if (value.Type == newAssetType)
                return;
            Files[value.Filename, newAssetType] = value;
            Files.RemoveKeyFromCollection(value);
        }

        private void OnMoveFile(PckAsset value)
        {
            if (Files.Contains(value.Filename, value.Type))
            {
                Files.Remove(value);
            }
        }

        public PckAsset GetOrCreate(string filename, PckAssetType assetType)
        {
            if (Files.Contains(filename, assetType))
            {
                return Files.GetFile(filename, assetType);
            }
            return CreateNewFile(filename, assetType);
        }

        public bool Contains(string filename, PckAssetType assetType)
        {
            return Files.Contains(filename, assetType);
        }

        public bool Contains(PckAssetType assetType)
        {
            return Files.Contains(assetType);
        }

        public void AddFile(PckAsset file)
        {
            file.Move();
            file.SetEvents(OnPckFileNameChanging, OnPckAssetTypeChanging, OnMoveFile);
            Files.Add(file);
        }

        public IReadOnlyCollection<PckAsset> GetFiles()
        {
            return new ReadOnlyCollection<PckAsset>(Files);
        }

        public bool TryGetValue(string filename, PckAssetType assetType, out PckAsset file)
        {
            return Files.TryGetValue(filename, assetType, out file);
        }

        public bool RemoveFile(PckAsset file)
        {
            return Files.Remove(file);
        }

        public void RemoveAll(Predicate<PckAsset> value)
        {
            Files.RemoveAll(value);
        }

        public void InsertFile(int index, PckAsset file)
        {
            Files.Insert(index, file);
        }

        public int IndexOfFile(PckAsset file)
        {
            return Files.IndexOf(file);
        }
    }
}
