using OMI.Formats.Languages;
using OMI.Workers;
using System;
using System.IO;
using System.Text;

namespace OMI.Workers.Language
{
    public class LOCFileWriter : IDataFormatWriter
    {
        private LOCFile _locfile;
        private int _type;

        public LOCFileWriter(LOCFile file, int type)
        {
            _ = file ?? throw new ArgumentNullException(nameof(file));
            _locfile = file;
            _type = type;
        }

        public void WriteToFile(string filename)
        {
            using(var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream, Endianness.BigEndian))
            {
                writer.Write(_type);
                writer.Write(_locfile.Languages.Count);
                if (_type == 2)
                {
                    // dont use uids
                    writer.Write(false);
                    writer.Write(_locfile.LocKeys.Count);
                    foreach (var key in _locfile.LocKeys.Keys)
                        WriteString(writer, key);
                }
                WriteLanguages(writer);
                WriteLanguageEntries(writer);
            }
        }

        private void WriteLanguages(EndiannessAwareBinaryWriter writer)
        {
            _locfile.Languages.ForEach(language =>
            {
                WriteString(writer, language);
                
                //Calculate the size of the language entry

                int size = 0;
                size += sizeof(int); // null int
                size += sizeof(byte); // null byte
                size += sizeof(short) + writer.EncodingScheme.GetByteCount(language);
                size += sizeof(int); // key count

                foreach (var locKey in _locfile.LocKeys.Keys)
                {
                    if (_type == 0)
                        size += sizeof(short) + writer.EncodingScheme.GetByteCount(locKey); // loc key string
                    size += sizeof(short) + writer.EncodingScheme.GetByteCount(_locfile.LocKeys[locKey][language]); // loc key string
                }

                writer.Write(size);
            });
        }

        private void WriteLanguageEntries(EndiannessAwareBinaryWriter writer)
        {
            _locfile.Languages.ForEach(language =>
            {
                writer.Write(_type);
                if (_type > 0)
                    writer.Write(false);

                WriteString(writer, language);
                writer.Write(_locfile.LocKeys.Keys.Count);
                foreach(var locKey in _locfile.LocKeys.Keys)
                {
                    if (_type == 0) WriteString(writer, locKey);
                    WriteString(writer, _locfile.LocKeys[locKey][language]);
                }
            });
        }

        private void WriteString(EndiannessAwareBinaryWriter writer, string s)
        {
            writer.Write(Convert.ToUInt16(writer.EncodingScheme.GetByteCount(s)));
            writer.WriteString(s);
        }
    }
}
