using System.Collections.Generic;
using System.IO;
using System.Text;
using OMI.Formats.Pck;

namespace OMI.Workers.Pck
{
    public class PckFileWriter : IDataFormatWriter
    {
        private readonly PckFile _pckFile;
        private readonly Endianness _endianness;
        private readonly IList<string> _propertyList;

        public PckFileWriter(PckFile pckFile, Endianness endianness)
        {
            _pckFile = pckFile;
            _endianness = endianness;
            _propertyList = pckFile.GetPropertyList();
        }

        public void WriteToFile(string filename)
        {
            using (var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream,
                _endianness == Endianness.LittleEndian ? Encoding.Unicode : Encoding.BigEndianUnicode, true, _endianness))
            {
                writer.Write(_pckFile.type);

                writer.Write(_propertyList.Count);
                foreach (var entry in _propertyList)
                {
                    writer.Write(_propertyList.IndexOf(entry));
                    WriteString(writer, entry);
                };
                if (_pckFile.HasVerionString)
                    writer.Write(1);

                writer.Write(_pckFile.Files.Count);
                foreach (var file in _pckFile.Files)
                {
                    writer.Write(file.Size);
                    writer.Write((int)file.Filetype);
                    WriteString(writer, file.Filename);
                }

                foreach (var file in _pckFile.Files)
                {
                    writer.Write(file.Properties.Count);
                    foreach (var property in file.Properties)
                    {
                        if (!_propertyList.Contains(property.Key))
                            throw new KeyNotFoundException("Property not found in Look Up Table: " + property.Key);
                        writer.Write(_propertyList.IndexOf(property.Key));
                        WriteString(writer, property.Value);
                    }
                    writer.Write(file.Data);
                }
            }
        }

        private void WriteString(EndiannessAwareBinaryWriter writer, string s)
        {
            writer.Write(s.Length);
            writer.WriteString(s);
            writer.Write(0);
        }
    }
}
