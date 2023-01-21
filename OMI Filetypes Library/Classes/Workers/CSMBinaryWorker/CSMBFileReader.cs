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
    internal class CSMBFileReader : StreamDataReader
    {

        public CSMBFileReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }


        public CSMBFile Read(string Filepath)
        {
            CSMBFile CSMB = new CSMBFile();
            if (File.Exists(Filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(Filepath, FileMode.Open)))
                {
                    Stream baseStream = binaryReader.BaseStream;
                    CSMB = Read(CSMB, baseStream, Filepath);
                }
            }
            return CSMB;
        }
        public CSMBFile Read(CSMBFile Mc, Stream s, string Filepath)
        {
            DateTime Begin = DateTime.Now;
            Mc.Version = ReadInt(s);
            int NumOfParts = ReadInt(s);
            for(int i = 0; i < NumOfParts; i++)
            {
                string PartName = ReadString(s);
                CSMBFile.CSMPart part = new CSMBFile.CSMPart();
                part.Parent = (CSMBFile.PartParent)ReadInt(s);
                part.Position[0] = ReadFloat(s);
                part.Position[1] = ReadFloat(s);
                part.Position[2] = ReadFloat(s);
                part.Size[0] = ReadFloat(s);
                part.Size[1] = ReadFloat(s);
                part.Size[2] = ReadFloat(s);
                part.UV[0] = ReadInt(s);
                part.UV[1] = ReadInt(s);
                part.HideWithArmour = ReadBool(s);
                part.MirrorTexture = ReadBool(s);
                part.Scale = ReadFloat(s);
                Mc.Parts.Add(PartName, part);
            }
            int NumOfOffsets = ReadInt(s);
            for (int i = 0; i < NumOfOffsets; i++)
            {
                Mc.Offsets.Add((CSMBFile.PartOffset)ReadInt(s), ReadFloat(s));
            }
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

            Console.WriteLine("Completed in: " + duration);
            return Mc;
        }
        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.UTF8);
        }
    }
}
