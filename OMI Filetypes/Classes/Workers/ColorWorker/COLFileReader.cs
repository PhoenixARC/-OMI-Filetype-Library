using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Color;
using OMI.utils;
/*
 * most known Color information is the direct product of May/MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Workers.Color
{
    internal class COLFileReader : StreamDataReader
    {

        public COLFileReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }

        public ColorContainer Read(byte[] locData, string Filepath)
        {
            ColorContainer modelContainer = new ColorContainer();
            MemoryStream s = new MemoryStream(locData);
            return Read(modelContainer, s, Filepath);
        }

        public ColorContainer Read(string Filepath)
        {
            ColorContainer modelContainer = new ColorContainer();
            if (File.Exists(Filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(Filepath, FileMode.Open)))
                {
                    Stream baseStream = binaryReader.BaseStream;
                    modelContainer = Read(modelContainer, baseStream, Filepath);
                }
            }
            return modelContainer;
        }


        public ColorContainer Read(ColorContainer Mc, Stream s, string Filepath)
        {
            DateTime Begin = DateTime.Now;
            Mc.ColorVersion = ReadInt(s);
            int NumOfColors = ReadInt(s);
            for (int i = 0; i < NumOfColors; i++)
            {
                ColorComponent.Color col = new ColorComponent.Color();
                col.Name = ReadString(s);
                col.SplitterByte = (byte)s.ReadByte();
                col.ColorPallette.R = s.ReadByte();
                col.ColorPallette.G = s.ReadByte();
                col.ColorPallette.B = s.ReadByte();
                Mc.Colors.Add(col);
            }
            int NumOfWaterColors = ReadInt(s);
            for (int i = 0; i < NumOfWaterColors; i++)
            {
                ColorComponent.WaterColor col = new ColorComponent.WaterColor();
                col.Name = ReadString(s);
                col.SplitterByte = (byte)s.ReadByte();
                col.surfaceColor.R = s.ReadByte();
                col.surfaceColor.G = s.ReadByte();
                col.surfaceColor.B = s.ReadByte();
                s.ReadByte();
                col.underwaterColor.R = s.ReadByte();
                col.underwaterColor.G = s.ReadByte();
                col.underwaterColor.B = s.ReadByte();
                s.ReadByte();
                col.fogColor.R = s.ReadByte();
                col.fogColor.G = s.ReadByte();
                col.fogColor.B = s.ReadByte();
                Mc.WaterColors.Add(col);
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
