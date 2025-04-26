﻿using System;
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
            using (var fs = File.Create(filename))
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

                writer.Write(_propertyList.Count + Convert.ToInt32(_pckFile.HasVerionString));
                if(_pckFile.HasVerionString)
                    _propertyList.Insert(0, PckFile.XMLVersionString);
                foreach (var entry in _propertyList)
                {
                        writer.Write(_propertyList.IndexOf(entry));
                        WriteString(writer, entry);
                };
                if (_pckFile.HasVerionString)
                {
                    writer.Write(1);
                }

                writer.Write(_pckFile.AssetCount);
                var assets = _pckFile.GetAssets();
                foreach (var asset in assets)
                {
                    writer.Write(asset.Size);
                    writer.Write((int)asset.Type);
                    WriteString(writer, asset.Filename);
                }

                foreach (var asset in assets)
                {
                    writer.Write(asset.Properties.Count);
                    foreach (var property in asset.Properties)
                    {
                        if (!_propertyList.Contains(property.Key))
                            throw new KeyNotFoundException("Property not found in Look Up Table: " + property.Key);
                        writer.Write(_propertyList.IndexOf(property.Key));
                        WriteString(writer, property.Value);
                    }
                    writer.Write(asset.Data);
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
