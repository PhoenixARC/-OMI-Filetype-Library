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
        public int AssetCount => Assets.Count;

        private PckAssetCollection Assets { get; } = new PckAssetCollection();
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
            foreach (var file in Assets)
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
        public PckAsset CreateNewAsset(string filename, PckAssetType assetType)
        {
            var file = new PckAsset(filename, assetType);
            AddAsset(file);
            return file;
        }

        /// <summary>
        /// Create, add and initialize new <see cref="PckAsset"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="assetType">Filetype</param>
        /// <returns>Initialized <see cref="PckAsset"/> object</returns>
        public PckAsset CreateNewAsset(string filename, PckAssetType assetType, Func<byte[]> dataInitializier)
        {
            var asset = CreateNewAsset(filename, assetType);
            asset.SetData(dataInitializier?.Invoke());
            return asset;
        }

        /// <summary>
        /// Checks wether a file with <paramref name="filename"/> and <paramref name="assetType"/> exists
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="assetType">Type of the file <see cref="PckAsset.FileType"/></param>
        /// <returns>True when file exists, otherwise false </returns>
        public bool HasAsset(string filename, PckAssetType assetType)
        {
            return Assets.Contains(filename, assetType);
        }

        /// <summary>
        /// Gets the first file that Equals <paramref name="filename"/> and <paramref name="assetType"/>
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="assetType">Type of the file <see cref="PckAsset.FileType"/></param>
        /// <returns>FileData if found, otherwise null</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public PckAsset GetAsset(string filename, PckAssetType assetType)
        {
            return Assets.GetAsset(filename, assetType);
        }

        /// <summary>
        /// Tries to get a file with <paramref name="filename"/> and <paramref name="assetType"/>.
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="assetType">Type of the file <see cref="PckAsset.FileType"/></param>
        /// <param name="asset">If succeeded <paramref name="asset"/> will be non-null, otherwise null</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public bool TryGetAsset(string filename, PckAssetType assetType, out PckAsset asset)
        {
            if (HasAsset(filename, assetType))
            {
                asset = GetAsset(filename, assetType);
                return true;
            }
                asset = null;
                return false;
            }

        private void OnPckAssetNameChanging(PckAsset value, string newFilename)
        {
            if (value.Filename.Equals(newFilename))
                return;
            Assets[newFilename, value.Type] = value;
            Assets.RemoveKeyFromCollection(value);
        }

        private void OnPckAssetTypeChanging(PckAsset value, PckAssetType newAssetType)
        {
            if (value.Type == newAssetType)
                return;
            Assets[value.Filename, newAssetType] = value;
            Assets.RemoveKeyFromCollection(value);
        }

        private void OnMoveFile(PckAsset value)
        {
            if (Assets.Contains(value.Filename, value.Type))
            {
                Assets.Remove(value);
            }
        }

        public PckAsset GetOrCreate(string filename, PckAssetType assetType)
        {
            if (Assets.Contains(filename, assetType))
            {
                return Assets.GetAsset(filename, assetType);
            }
            return CreateNewAsset(filename, assetType);
        }

        public bool Contains(string filename, PckAssetType assetType)
        {
            return Assets.Contains(filename, assetType);
        }

        public bool Contains(PckAssetType assetType)
        {
            return Assets.Contains(assetType);
        }

        public void AddAsset(PckAsset asset)
        {
            asset.Move();
            asset.SetEvents(OnPckAssetNameChanging, OnPckAssetTypeChanging, OnMoveFile);
            Assets.Add(asset);
        }

        public IReadOnlyCollection<PckAsset> GetAssets()
        {
            return new ReadOnlyCollection<PckAsset>(Assets);
        }

        public bool TryGetValue(string filename, PckAssetType assetType, out PckAsset asset)
        {
            return Assets.TryGetValue(filename, assetType, out asset);
        }

        public bool RemoveAsset(PckAsset asset)
        {
            return Assets.Remove(asset);
        }

        public void RemoveAll(Predicate<PckAsset> value)
        {
            Assets.RemoveAll(value);
        }

        public void InsertAsset(int index, PckAsset asset)
        {
            Assets.Insert(index, asset);
        }

        public int IndexOfAsset(PckAsset asset)
        {
            return Assets.IndexOf(asset);
        }
    }
}
