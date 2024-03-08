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
        /// Create and add new <see cref="PckFileData"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="filetype">Filetype</param>
        /// <returns>Added <see cref="PckFileData"/> object</returns>
        public PckFileData CreateNewFile(string filename, PckFileType filetype)
        {
            var file = new PckFileData(filename, filetype);
            AddFile(file);
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
        /// Checks wether a file with <paramref name="filename"/> and <paramref name="filetype"/> exists
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="filetype">Type of the file <see cref="PckFileData.FileType"/></param>
        /// <returns>True when file exists, otherwise false </returns>
        public bool HasFile(string filename, PckFileType filetype)
        {
            return Files.Contains(filename, filetype);
        }

        /// <summary>
        /// Gets the first file that Equals <paramref name="filename"/> and <paramref name="filetype"/>
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="filetype">Type of the file <see cref="PckFileData.FileType"/></param>
        /// <returns>FileData if found, otherwise null</returns>
        /// <exception cref="KeyNotFoundException"></exception>
        public PckFileData GetFile(string filename, PckFileType filetype)
        {
            return Files[filename, filetype];
        }

        /// <summary>
        /// Tries to get a file with <paramref name="filename"/> and <paramref name="filetype"/>.
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="filetype">Type of the file <see cref="PckFileData.FileType"/></param>
        /// <param name="file">If succeeded <paramref name="file"/> will be non-null, otherwise null</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public bool TryGetFile(string filename, PckFileType filetype, out PckFileData file)
        {
            try
            {
                file = GetFile(filename, filetype);
                return file is PckFileData;
            }
            catch (KeyNotFoundException)
            {
                file = null;
                return false;
            }
        }

        private void OnPckFileNameChanging(PckFileData value, string newFilename)
        {
            if (value.Filename.Equals(newFilename))
                return;
            Files[newFilename, value.Filetype] = value;
            if (Files.Contains(value.Filename, value.Filetype))
            {
                Files.Remove(value);
            }
        }

        private void OnPckFileTypeChanging(PckFileData value, PckFileType newFiletype)
        {
            if (value.Filetype == newFiletype)
                return;
            Files[value.Filename, newFiletype] = value;
            if (Files.Contains(value.Filename, value.Filetype))
            {
                Files.Remove(value);
            }
        }

        private void OnMoveFile(PckFileData value)
        {
            if (Files.Contains(value.Filename, value.Filetype))
            {
                Files.Remove(value);
            }
        }

        public PckFileData GetOrCreate(string filename, PckFileType filetype)
        {
            if (Files.Contains(filename, filetype))
            {
                return Files[filename, filetype];
            }
            return new PckFileData(filename, filetype, OnPckFileNameChanging, OnPckFileTypeChanging, OnMoveFile);
        }

        public bool Contains(string filename, PckFileType filetype)
        {
            return Files.Contains(filename, filetype);
        }

        public void AddFile(PckFileData file)
        {
            file.Move();
            file.SetEvents(OnPckFileNameChanging, OnPckFileTypeChanging, OnMoveFile);
            Files.Add(file);
        }

        public IReadOnlyCollection<PckFileData> GetFiles()
        {
            return new ReadOnlyCollection<PckFileData>(Files);
        }

        public bool TryGetValue(string filename, PckFileType filetype, out PckFileData file)
        {
            return Files.TryGetValue(filename, filetype, out file);
        }

        public bool RemoveFile(PckFileData file)
        {
            return Files.Remove(file);
        }

        public void RemoveAll(Predicate<PckFileData> value)
        {
            Files.RemoveAll(value);
        }

        public void InsertFile(int index, PckFileData file)
        {
            Files.Insert(index, file);
        }

        public int IndexOfFile(PckFileData file)
        {
            return Files.IndexOf(file);
        }
    }
}
