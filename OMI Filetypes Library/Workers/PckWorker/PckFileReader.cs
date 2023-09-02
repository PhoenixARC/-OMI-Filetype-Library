/* Copyright (c) 2022-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using OMI.Formats.Pck;

namespace OMI.Workers.Pck
{
    public class PckFileReader : IDataFormatReader<PckFile>, IDataFormatReader
    {
        private readonly Endianness _endianness;

        private IList<PckFileData> _files;

        /// <summary>
        /// Constructs a new Instance of PckFileReader with the default Endianness (<see cref="Endianness.BigEndian"/>)
        /// </summary>
        public PckFileReader()
            : this(Endianness.BigEndian)
        { }

        public PckFileReader(Endianness endianness)
        {
            _endianness = endianness;
        }

        public PckFile FromFile(string filename)
        {
            if (File.Exists(filename))
            {
                using (var fs = File.OpenRead(filename))
                {
                    return FromStream(fs);
                }
            }
            throw new FileNotFoundException(filename);
        }

        public PckFile FromStream(Stream stream)
        {
            PckFile pckFile = null;
            using (var reader = new EndiannessAwareBinaryReader(stream,
                _endianness == Endianness.LittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode,
                leaveOpen: true,
                endianness: _endianness))
            {
                int pck_type = reader.ReadInt32();
                if (pck_type > 0x00_F0_00_00)
                    throw new OverflowException(nameof(pck_type));
                else if (pck_type < 3) throw new Exception(pck_type.ToString());

                IList<string> propertyList = ReadLookUpTable(reader, out bool hasVersionStr);
                pckFile = new PckFile(pck_type, hasVersionStr);
                ReadFileEntries(reader);
                ReadFileContents(pckFile, propertyList, reader);
            }
            return pckFile;
        }

        private IList<string> ReadLookUpTable(EndiannessAwareBinaryReader reader, out bool hasVerStr)
        {
            int count = reader.ReadInt32();
            var propertyLookUp = new List<string>(count);
            for (int i = 0; i < count; i++)
            {
                int index = reader.ReadInt32();
                string value = ReadString(reader);
                propertyLookUp.Insert(index, value);
            }
            if (hasVerStr = propertyLookUp.Contains(PckFile.XMLVersionString))
            {
                Debug.WriteLine($"XML Version num: {reader.ReadInt32()}"); // xml version num ??
            }
            return propertyLookUp;
        }

        private void ReadFileEntries(EndiannessAwareBinaryReader reader)
        {
            int file_entry_count = reader.ReadInt32();
            _files = new List<PckFileData>(file_entry_count);
            for (; 0 < file_entry_count; file_entry_count--)
            {
                int file_size = reader.ReadInt32();
                var file_type = (PckFileType)reader.ReadInt32();
                string file_name = ReadString(reader).Replace('\\', '/');
                var entry = new PckFileData(file_name, file_type, file_size);
                _files.Add(entry);
            }
        }

        private void ReadFileContents(PckFile pckFile, IList<string> propertyList, EndiannessAwareBinaryReader reader)
        {
            foreach (var file in _files)
            {
                int property_count = reader.ReadInt32();
                for (; 0 < property_count; property_count--)
                {
                    string key = propertyList[reader.ReadInt32()];
                    string value = ReadString(reader);
                    file.Properties.Add(key, value);
                }
                reader.Read(file.Data, 0, file.Size);
                pckFile.AddFile(file);
            };
            _files.Clear();
        }

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            int len = reader.ReadInt32();
            string s = reader.ReadString(len);
            reader.ReadInt32(); // padding
            return s;
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}