using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Pck;
using OMI.Workers;

namespace OMI.Classes.Workers.PckWorker
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
                if (_propertyList.Contains(PckFile.XMLVersionString))
                    writer.Write(0x1337); // :^)


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
                        if (!_propertyList.Contains(property.property))
                            throw new Exception("Property not found in Look Up Table: " + property.property);
                        writer.Write(_propertyList.IndexOf(property.property));
                        WriteString(writer, property.value);
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
