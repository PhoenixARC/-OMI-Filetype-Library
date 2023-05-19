using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.Pck
{
    using PropertyValueType = KeyValuePair<string, string>;
    public class PckFile
    {
        public int type { get; }
        public List<FileData> Files { get; } = new List<FileData>();

        public const string XMLVersionString = "XMLVERSION";
        public bool HasVerionString => _hasVerionString;
        private bool _hasVerionString = false;

        public class PCKProperties : List<PropertyValueType>
        {
            public void Add<T>(string key, T value)
            {
                Add(new PropertyValueType(key, value.ToString()));
            }

            public void Add<T>((string key, T value) property)
            {
                Add(new PropertyValueType(property.key, property.value.ToString()));
            }

            public bool Contains(string property)
            {
                return HasProperty(property);
            }

            public bool HasProperty(string property)
            {
                return GetProperty(property).Key != string.Empty;
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

        public class FileData
        {
            public enum FileType : int
            {
                SkinFile = 0,  // *.png
                CapeFile = 1,  // *.png
                TextureFile = 2,  // *.png
                UIDataFile = 3,  // *.fui ????
                /// <summary>
                /// "0" file
                /// </summary>
                InfoFile = 4,
                /// <summary>
                /// (x16|x32|x64)Info.pck
                /// </summary>
                TexturePackInfoFile = 5,
                /// <summary>
                /// languages.loc/localisation.loc
                /// </summary>
                LocalisationFile = 6,
                /// <summary>
                /// GameRules.grf
                /// </summary>
                GameRulesFile = 7,
                /// <summary>
                /// audio.pck
                /// </summary>
                AudioFile = 8,
                /// <summary>
                /// colours.col
                /// </summary>
                ColourTableFile = 9,
                /// <summary>
                /// GameRules.grh
                /// </summary>
                GameRulesHeader = 10,
                /// <summary>
                /// Skins.pck
                /// </summary>
                SkinDataFile = 11,
                /// <summary>
                /// models.bin
                /// </summary>
		        ModelsFile = 12,
                /// <summary>
                /// behaviours.bin
                /// </summary>
                BehavioursFile = 13,
                /// <summary>
                /// entityMaterials.bin
                /// </summary>
                MaterialFile = 14,
            }

            public string Filename
            {
                get => filename;
                set => filename = value.Replace('\\', '/');
            }

            public FileType Filetype { get; set; }
            public byte[] Data => _data;
            public int Size => _data?.Length ?? 0;
            public PCKProperties Properties { get; } = new PCKProperties();

            private string filename;
            private byte[] _data = new byte[0];

            public FileData(string filename, FileType filetype)
            {
                Filetype = filetype;
                Filename = filename;
            }

            public FileData(string filename, FileType filetype, int dataSize) : this(filename, filetype)
            {
                _data = new byte[dataSize];
            }

            public FileData(FileData file) : this(file.Filename, file.Filetype)
            {
                Properties = file.Properties;
                SetData(file.Data);
            }

            public void SetData(byte[] data)
            {
                _data = data;
            }

        }

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
            Files.ForEach(file => file.Properties.ForEach(pair =>
            {
                if (!LUT.Contains(pair.Key))
                    LUT.Add(pair.Key);
            })
            );
            if (HasVerionString)
                LUT.Insert(0, XMLVersionString);
            return LUT;
        }

        /// <summary>
        /// Create and add new <see cref="FileData"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="filetype">Filetype</param>
        /// <returns>Added <see cref="FileData"/> object</returns>
        public FileData CreateNewFile(string filename, FileData.FileType filetype)
        {
            var file = new FileData(filename, filetype);
            Files.Add(file);
            return file;
        }

        /// <summary>
        /// Create, add and initialize new <see cref="FileData"/> object.
        /// </summary>
        /// <param name="filename">Filename</param>
        /// <param name="filetype">Filetype</param>
        /// <returns>Initialized <see cref="FileData"/> object</returns>
        public FileData CreateNewFile(string filename, FileData.FileType filetype, Func<byte[]> dataInitializier)
        {
            var file = CreateNewFile(filename, filetype);
            file.SetData(dataInitializier?.Invoke());
            return file;
        }

        /// <summary>
        /// Checks wether a file with <paramref name="filename"/> and <paramref name="type"/> exists
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.FileType"/></param>
        /// <returns>True when file exists, otherwise false </returns>
        public bool HasFile(string filename, FileData.FileType type)
        {
            return GetFile(filename, type) is FileData;
        }

        /// <summary>
        /// Gets the first file that Equals <paramref name="filename"/> and <paramref name="type"/>
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.FileType"/></param>
        /// <returns>FileData if found, otherwise null</returns>
        public FileData GetFile(string filename, FileData.FileType type)
        {
            return Files.FirstOrDefault(file => file.Filename.Equals(filename) && file.Filetype.Equals(type));
        }

        /// <summary>
        /// Tries to get a file with <paramref name="filename"/> and <paramref name="type"/>.
        /// </summary>
        /// <param name="filename">Path to the file in the pck</param>
        /// <param name="type">Type of the file <see cref="FileData.FileType"/></param>
        /// <param name="file">If succeeded <paramref name="file"/> will be non-null, otherwise null</param>
        /// <returns>True if succeeded, otherwise false</returns>
        public bool TryGetFile(string filename, FileData.FileType type, out FileData file)
        {
            file = GetFile(filename, type);
            return file is FileData;
        }
    }
}
