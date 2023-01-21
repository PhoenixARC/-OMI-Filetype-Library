using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.CSMB;
using OMI.utils;

namespace OMI.Workers.CSMB
{
    internal class CSMBBuilder : StreamDataWriter
    {
        CSMBFile _file;
        public CSMBBuilder(CSMBFile Mc, bool useLittleEndian) : base(useLittleEndian)
        {
            _file = Mc;
        }
        public void Write(Stream s)
        {
            WriteInt(s, _file.Version);
            WriteInt(s, _file.Parts.Count);
            foreach(KeyValuePair<string, CSMBFile.CSMPart> pair in _file.Parts)
            {
                WriteString(s, pair.Key);
                Console.WriteLine(pair.Key);
                WriteInt(s, (int)pair.Value.Parent);
                WriteFloat(s, pair.Value.Position[0]);
                WriteFloat(s, pair.Value.Position[1]);
                WriteFloat(s, pair.Value.Position[2]);
                WriteFloat(s, pair.Value.Size[0]);
                WriteFloat(s, pair.Value.Size[1]);
                WriteFloat(s, pair.Value.Size[2]);
                WriteInt(s, pair.Value.UV[0]);
                WriteInt(s, pair.Value.UV[1]);
                WriteBool(s, pair.Value.HideWithArmour);
                WriteBool(s, pair.Value.MirrorTexture);
                WriteFloat(s, pair.Value.Scale);
            }
            WriteInt(s, _file.Offsets.Count);
            foreach (KeyValuePair<CSMBFile.PartOffset, float> pair in _file.Offsets)
            {
                WriteInt(s, (int)pair.Key);
                WriteFloat(s, pair.Value);
            }
        }
        private void WriteString(Stream stream, string String)
        {
            WriteShort(stream, (short)String.Length);
            WriteString(stream, String, Encoding.UTF8);
        }

    }
}
