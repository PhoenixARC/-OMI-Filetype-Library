﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using OMI.Formats.Languages;

namespace OMI.Workers.Language
{
    public class LOCFileReader : IDataFormatReader<LOCFile>, IDataFormatReader
    {
        public LOCFile FromFile(string filename)
        {
            if (File.Exists(filename))
            {
                LOCFile locFile = null;
                using (var fs = File.OpenRead(filename))
                {
                    locFile = FromStream(fs);
                }
                return locFile;
            }
            throw new FileNotFoundException(filename);
        }

        public LOCFile FromStream(Stream stream)
        {
            LOCFile locFile = new LOCFile();
            using (var reader = new EndiannessAwareBinaryReader(stream, Endianness.BigEndian))
            {
                int loc_type = reader.ReadInt32();
                int language_count = reader.ReadInt32();
                bool lookUpKey = loc_type == 2;
                List<string> keys = lookUpKey ? ReadKeys(reader) : null;
                for (int i = 0; i < language_count; i++)
                {
                    string language = ReadString(reader);
                    reader.ReadInt32();
                    locFile.Languages.Add(language);
                }
                for (int i = 0; i < language_count; i++)
                {
                    if (0 < reader.ReadInt32())
                        stream.ReadByte();
                    string language = ReadString(reader);
                    if (!locFile.Languages.Contains(language))
                        throw new KeyNotFoundException(nameof(language));
                    int count = reader.ReadInt32();
                    for (int j = 0; j < count; j++)
                    {
                        string key = lookUpKey ? keys[j] : ReadString(reader);
                        string value = ReadString(reader);
                        locFile.SetLocEntry(key, language, value);
                    }
                }
            }
            return locFile;
        }

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        private List<string> ReadKeys(EndiannessAwareBinaryReader reader)
        {
            bool useUniqueIds = reader.ReadBoolean();
            int keyCount = reader.ReadInt32();
            List<string> keys = new List<string>(keyCount);
            for (int i = 0; i < keyCount; i++)
            {
                string key = useUniqueIds ? reader.ReadInt32().ToString("X08") : ReadString(reader);
                keys.Add(key);
            }
            return keys;
        }

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            int length = reader.ReadUInt16();
            return reader.ReadString(length, Encoding.UTF8);
        }
    }
}