using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ColorWorker.model;

namespace ColorWorker
{
    public class ColorParser
    {
        private ArraySupport ArrSupport;

        public ColorParser()
        {
            ArrSupport = new ArraySupport();
        }

        public ColorContainer Parse(byte[] locData, string Filepath)
        {
            ColorContainer modelContainer = new ColorContainer();
            MemoryStream s = new MemoryStream(locData);
            return Parse(modelContainer, s, Filepath);
        }

        public ColorContainer Parse(string Filepath)
        {
            ColorContainer modelContainer = new ColorContainer();
            if (File.Exists(Filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(Filepath, FileMode.Open)))
                {
                    Stream baseStream = binaryReader.BaseStream;
                    modelContainer = Parse(modelContainer, baseStream, Filepath);
                }
            }
            return modelContainer;
        }


        public ColorContainer Parse(ColorContainer Mc, Stream s, string Filepath)
        {
            DateTime Begin = DateTime.Now;
            Mc.ColorVersion = ArrSupport.GetInt32(s);
            int NumOfColors = ArrSupport.GetInt32(s);
            for (int i = 0; i < NumOfColors; i++)
            {
                ColorComponent.Color col = new ColorComponent.Color();
                col.Name = ArrSupport.GetString(s);
                col.SplitterByte = (byte)s.ReadByte();
                col.ColorPallette.R = s.ReadByte();
                col.ColorPallette.G = s.ReadByte();
                col.ColorPallette.B = s.ReadByte();
                Mc.Colors.Add(col);
            }
            int NumOfWaterColors = ArrSupport.GetInt32(s);
            for (int i = 0; i < NumOfWaterColors; i++)
            {
                ColorComponent.WaterColor col = new ColorComponent.WaterColor();
                col.Name = ArrSupport.GetString(s);
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
    }
}
