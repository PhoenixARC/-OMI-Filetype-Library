using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.IO.Compression;
using System.Diagnostics;
using OMI.Formats.GameRule;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;

namespace OMI.Workers.GameRule
{
    public class GameRuleFileReader : IDataFormatReader<GameRuleFile>, IDataFormatReader
    {
        private IList<string> StringLookUpTable;
        private GameRuleFile _file;
        private GameRuleFile.CompressionType _compressionType;

        public GameRuleFileReader(GameRuleFile.CompressionType compressionType)
        {
            _compressionType = compressionType;
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);
        object IDataFormatReader.FromFile(string filename) => FromFile(filename);

        public GameRuleFile FromFile(string filename)
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

        public GameRuleFile FromStream(Stream stream)
        {
            using (var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, Endianness.BigEndian))
            {
                var header = ReadHeader(reader);
                ReadBody(header, reader);
            }
            return _file;
        }

        private GameRuleFileHeader ReadHeader(EndiannessAwareBinaryReader reader)
        {
            int x = reader.ReadInt16();
            if (((x >> 31) | x) == 0)
            {
                // 14 bools ?...
                reader.ReadBytes(14);
                return new GameRuleFileHeader(0xffffffff, GameRuleFile.CompressionLevel.None, new byte[4]);
            }

            var compressionLevel = (GameRuleFile.CompressionLevel)reader.ReadByte();
            uint crc = reader.ReadUInt32();
            byte[] unknownDataBuffer = new byte[4];
            reader.Read(unknownDataBuffer, 0, unknownDataBuffer.Length);
            if (unknownDataBuffer[3] > 0)
            {
                //compressionLevel = (GameRuleFile.CompressionLevel)byte4;
                throw new NotSupportedException("World grf's are not currently not supported.");
            }
            _file = new GameRuleFile(crc, unknownDataBuffer[3] > 0, compressionLevel);

            return new GameRuleFileHeader(crc, compressionLevel, unknownDataBuffer);
        }

        private void ReadBody(GameRuleFileHeader header, EndiannessAwareBinaryReader reader)
        {
            if (header.CompressionLevel != GameRuleFile.CompressionLevel.None /*||
                header.unknownData[3] != 0*/)
            {
                reader = DecompressBody(header, reader);
            }

            ReadStringLookUpTable(reader);
            string Name = GetString(reader);
            Debug.WriteLine(string.Format("Root Name: {0}", Name), category: nameof(GameRuleFileReader.ReadBody));
            ReadGameRuleHierarchy(reader, _file.Root);
        }

        private EndiannessAwareBinaryReader DecompressBody(GameRuleFileHeader fileHeader, EndiannessAwareBinaryReader reader)
        {
            int decompressedSize = reader.ReadInt32();
            int compressedSize = reader.ReadInt32();

            //if (fileHeader.unknownData[3] != 0)
            //{
            //    new_stream = new MemoryStream(reader.ReadBytes(compressedSize));
            //    compressedSize = reader.ReadInt32();
            //}

            var decpompressedReader = reader;
            if (fileHeader.CompressionLevel > GameRuleFile.CompressionLevel.Compressed)
            {
                using (var decompressedStream = DecompressStream(reader.BaseStream, decompressedSize, compressedSize))
                {
                    var rlebufffer = new byte[(int)decompressedStream.Length];
                    decompressedStream.Read(rlebufffer, 0, (int)decompressedStream.Length);
                    byte[] decodedData = RLE<byte>.Decode(rlebufffer).ToArray();
                    var stream = new MemoryStream(decodedData);
                    decpompressedReader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, Endianness.BigEndian);
                }
            }

            //if (fileHeader.unknownData[3] != 0)
            //    decompressedStream.Read(new byte[23], 0, 23);

            return decpompressedReader;
        }

        private Stream DecompressStream(Stream stream, int decompressedSize, int compressedSize)
        {
            Stream decompressedStream = _compressionType switch
            {
                GameRuleFile.CompressionType.Zlib => new InflaterInputStream(stream),
                GameRuleFile.CompressionType.Deflate => new DeflateStream(stream, CompressionLevel.Optimal),
                GameRuleFile.CompressionType.XMem => new LzxDecoderStream(stream, decompressedSize, compressedSize),
                _ => throw new NotImplementedException(),
            };
            var outputStream = new MemoryStream();
            decompressedStream.CopyTo(outputStream);
            outputStream.Position = 0;
            return outputStream;
        }

        private void ReadStringLookUpTable(EndiannessAwareBinaryReader reader)
        {
            int tableSize = reader.ReadInt32();
            StringLookUpTable = new List<string>(tableSize);
            for (int i = 0; i < tableSize; i++)
            {
                string s = ReadString(reader);
                StringLookUpTable.Add(s);
            }
        }

        private void ReadGameRuleHierarchy(EndiannessAwareBinaryReader reader, GameRuleFile.GameRule parent)
        {
            _ = parent ?? throw new ArgumentNullException(nameof(parent));
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                (string Name, int Count) parameter = (GetString(reader), reader.ReadInt32());
                var rule = parent.AddRule(parameter.Name);
                for (int j = 0; j < parameter.Count; j++)
                {
                    rule.Parameters.Add(GetString(reader), ReadString(reader));
                }
                ReadGameRuleHierarchy(reader, rule);
            }
        }

        private string GetString(EndiannessAwareBinaryReader reader) => StringLookUpTable[reader.ReadInt32()];

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            short stringLength = reader.ReadInt16();
            return reader.ReadString(stringLength);
        }

    }
}
