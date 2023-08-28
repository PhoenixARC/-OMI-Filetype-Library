using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.Pck
{
    public class PckFile
    {
        public readonly int type;

        public FileCollection Files { get; } = new FileCollection();

        public const string XMLVersionString = "XMLVERSION";
        public bool HasVerionString => _hasVerionString;
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
            if (HasVerionString)
                LUT.Insert(0, XMLVersionString);
            return LUT;
        }

        /// <summary>
        /// Create and add new <see cref="PckFileData"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="filetype">Filetype</param>
        /// <returns>Added <see cref="PckFileData"/> object</returns>
        public PckFileData CreateNewFile(string filename, PckFileType filetype)
        {
            var file = new PckFileData(filename, filetype);
            Files.Add(file);
            return file;
        }

        /// <summary>
        /// Create, add and initialize new <see cref="PckFileData"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="filetype">Filetype</param>
        /// <returns>Initialized <see cref="PckFileData"/> object</returns>
        public PckFileData CreateNewFile(string filename, PckFileType filetype, Func<byte[]> dataInitializier)
        {
            var file = CreateNewFile(filename, filetype);
            file.SetData(dataInitializier?.Invoke());
            return file;
        }

        /// <summary>
        /// Checks wether a file with <paramref name="filename"/> and <paramref name="type"/> exists
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="PckFileData.FileType"/></param>
        /// <returns>True when file exists, otherwise false </returns>
        public bool HasFile(string filename, PckFileType type)
        {
            return GetFile(filename, type) is PckFileData;
        }

        /// <summary>
        /// Gets the first file that Equals <paramref name="filename"/> and <paramref name="type"/>
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="PckFileData.FileType"/></param>
        /// <returns>FileData if found, otherwise null</returns>
        public PckFileData GetFile(string filename, PckFileType type)
        {
            return Files[filename, type];
        }

        /// <summary>
        /// Tries to get a file with <paramref name="filename"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="PckFileData.FileType"/></param>
        /// <param name="file">If succeeded <paramref name="file"/> will be non-null, otherwise null</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public bool TryGetFile(string filename, PckFileType type, out PckFileData file)
        {
            file = GetFile(filename, type);
            return file is PckFileData;
        }
    }
}
