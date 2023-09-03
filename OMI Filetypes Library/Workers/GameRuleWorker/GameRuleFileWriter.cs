using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using System.IO.Compression;
using OMI.Formats.GameRule;
using OMI.Workers;

namespace OMI.Workers.GameRule
{
    public class GameRuleFileWriter : IDataFormatWriter
    {
        private readonly GameRuleFile _grfFile;
        private List<string> StringLookUpTable;

        private GameRuleFile.CompressionLevel _compressionLvl;
        private GameRuleFile.CompressionType _compressionType;

        /// <summary>
        /// Initializes new <see cref="GameRuleFileReader"/> with the <see cref="GameRuleFile.CompressionLevel"/> set to <see cref="GameRuleFile.CompressionLevel.None"/>
        /// </summary>
        /// <param name="grf"></param>
        public GameRuleFileWriter(GameRuleFile grf)
            : this(grf, GameRuleFile.CompressionLevel.None, GameRuleFile.CompressionType.Zlib)
        {

        }

        public GameRuleFileWriter(GameRuleFile grf, GameRuleFile.CompressionLevel compressionLvl, GameRuleFile.CompressionType compressionType)
        {
            _compressionLvl = compressionLvl;
            _compressionType = compressionType;
            if (grf.Header.unknownData[3] != 0)
                throw new NotImplementedException("World grf saving is currently unsupported");
            _grfFile = grf;
            StringLookUpTable = new List<string>();
            PrepareLookUpTable(_grfFile.Root, StringLookUpTable);
        }

        private void PrepareLookUpTable(GameRuleFile.GameRule rule, List<string> LUT)
        {
            if (!LUT.Contains(rule.Name)) LUT.Add(rule.Name);
            rule.ChildRules.ForEach(subRule => PrepareLookUpTable(subRule, LUT));
            foreach (var param in rule.Parameters)
                if (!LUT.Contains(param.Key)) LUT.Add(param.Key);
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
            var writer = new EndiannessAwareBinaryWriter(stream, Encoding.ASCII, leaveOpen: true, Endianness.BigEndian);
            WriteHeader(writer);
            using (var uncompressed_stream = new MemoryStream())
            {
                var decompressed_writer = new EndiannessAwareBinaryWriter(uncompressed_stream, Encoding.ASCII, leaveOpen: true, Endianness.BigEndian);
                WriteBody(decompressed_writer);
                HandleCompression(writer, uncompressed_stream);
            }
        }

        private void HandleCompression(EndiannessAwareBinaryWriter destination, MemoryStream sourceStream)
        {
            byte[] _buffer = sourceStream.ToArray();
            int _original_length = _buffer.Length;

            if (_compressionLvl >= GameRuleFile.CompressionLevel.CompressedRle)
                _buffer = CompressRLE(_buffer);
            if (_compressionLvl >= GameRuleFile.CompressionLevel.Compressed)
            {
                var compressed_buffer = Compress(_buffer);
                destination.Write(_original_length);
                destination.Write(compressed_buffer.Length);
            }
            if (_compressionLvl >= GameRuleFile.CompressionLevel.CompressedRleCrc)
                MakeAndWriteCrc(destination, _buffer);
            destination.Write(_buffer);
        }

        private byte[] Compress(byte[] data)
        {
            var outputStream = new MemoryStream(); // Stream gets Disposed in DeflaterOutputStream

            using (Stream deflateStream = _compressionType switch
            {
                GameRuleFile.CompressionType.Zlib => new DeflaterOutputStream(outputStream),
                GameRuleFile.CompressionType.Deflate => new DeflateStream(outputStream, CompressionLevel.Optimal),
                GameRuleFile.CompressionType.XMem => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            })
            {
                deflateStream.Write(data, 0, data.Length);
                deflateStream.Flush();
                outputStream.Position = 0;
            }
            return outputStream.ToArray();
        }

        private byte[] CompressRLE(byte[] data) => RLE<byte>.Encode(data).ToArray();

        private void MakeAndWriteCrc(EndiannessAwareBinaryWriter writer, byte[] data)
        {
            uint crc = CRC32.CRC(data);
            if (crc != _grfFile.Header.Crc)
            {
                writer.BaseStream.Position = 3;
                writer.Write(crc);
                writer.BaseStream.Seek(0, SeekOrigin.End);
            }
        }

        private void WriteHeader(EndiannessAwareBinaryWriter writer)
        {
            writer.Write((short)1);
            if (_compressionLvl < GameRuleFile.CompressionLevel.None ||
                _compressionLvl > GameRuleFile.CompressionLevel.CompressedRleCrc)
                throw new ArgumentOutOfRangeException(nameof(_compressionLvl));
            writer.Write((byte)_compressionLvl);
            writer.Write(_grfFile.Header.Crc);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0);
            writer.Write((byte)0); // <- used in world grf
        }

        private void WriteBody(EndiannessAwareBinaryWriter writer)
        {
            WriteTagLookUpTable(writer);
            SetString(writer, _grfFile.Root.Name);
            writer.Write(_grfFile.Root.ChildRules.Count);
            WriteGameRuleHierarchy(writer, _grfFile.Root);
        }

        private void WriteTagLookUpTable(EndiannessAwareBinaryWriter writer)
        {
            writer.Write(StringLookUpTable.Count);
            StringLookUpTable.ForEach( s => WriteString(writer, s) );
        }

        private void WriteGameRuleHierarchy(EndiannessAwareBinaryWriter writer, GameRuleFile.GameRule rule)
        {
            foreach (var subRule in rule.ChildRules)
            {
                SetString(writer, subRule.Name);
                writer.Write(subRule.Parameters.Count);
                foreach (var param in subRule.Parameters) WriteParameter(writer, param);
                writer.Write(subRule.ChildRules.Count);
                WriteGameRuleHierarchy(writer, subRule);
            }
        }

        private void WriteParameter(EndiannessAwareBinaryWriter writer, KeyValuePair<string, string> param)
        {
            SetString(writer, param.Key);
            WriteString(writer, param.Value);
        }

        private void SetString(EndiannessAwareBinaryWriter writer, string s)
        {
            int i = StringLookUpTable.IndexOf(s);
            if (i == -1) throw new Exception(nameof(s));
            writer.Write(i);
        }

        private void WriteString(EndiannessAwareBinaryWriter writer, string s)
        {
            writer.Write((short)s.Length);
            writer.WriteString(s);
        }
    }
}
