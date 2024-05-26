using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.GameRule;

namespace OMI.Workers.GameRule
{
    public class GameRuleFileHeader
    {
        public uint Crc;
        public GameRuleFile.CompressionLevel CompressionLevel;
        public GameRuleFile.CompressionType CompressionType;
        public byte[] unknownData;

        public GameRuleFileHeader(uint crc, GameRuleFile.CompressionLevel compressionLevel, byte[] bytes)
            : this(crc, compressionLevel)
        {
            unknownData = bytes;
        }

        public GameRuleFileHeader(uint crc, GameRuleFile.CompressionLevel compressionLevel, GameRuleFile.CompressionType compressionType, byte[] bytes)
            : this(crc, compressionLevel, compressionType)
        {
            unknownData = bytes;
        }

        public GameRuleFileHeader(uint crc, GameRuleFile.CompressionLevel compressionLevel)
            : this(crc, compressionLevel, GameRuleFile.CompressionType.Unknown)
        {
        }

        public GameRuleFileHeader(uint crc, GameRuleFile.CompressionLevel compressionLevel, GameRuleFile.CompressionType compressionType)
        {
            Crc = crc;
            CompressionLevel = compressionLevel;
            CompressionType = compressionType;
            unknownData = new byte[4];
        }
    }
}
