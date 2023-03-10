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

        public GameRuleFileHeader(uint crc, GameRuleFile.CompressionLevel compressionLevel, byte[] unknownData)
        {
            Crc = crc;
            CompressionLevel = compressionLevel;
            this.unknownData = unknownData;
        }
    }
}
