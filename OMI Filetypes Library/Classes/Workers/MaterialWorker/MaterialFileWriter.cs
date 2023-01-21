using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Material;
using OMI.utils;
/*
 * all known Model/Material information is the direct product of May/MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Workers.Material
{
    internal class MaterialFileWriter : StreamDataWriter
    {
        public MaterialFileWriter(bool useLittleEndian) : base(useLittleEndian)
        {
        }
        public void Build(MaterialContainer Mc, string FilePath, Stream s)
        {
            WriteInt(s, Mc.Version);
            WriteInt(s, Mc.materials.Count);
            foreach (OMI.Formats.Material.Material mat in Mc.materials)
            {
                WriteInt(s, mat.MaterialName.Length);
                WriteString(s, mat.MaterialName);
                WriteString(s, mat.MaterialType);
            }
        }
        private void WriteString(Stream stream, string String)
        {
            WriteShort(stream, (short)String.Length);
            WriteString(stream, String, Encoding.UTF8);
        }
    }
}
