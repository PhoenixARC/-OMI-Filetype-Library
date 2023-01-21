using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Color;
using OMI.utils;
using System.Diagnostics;
using System.Drawing;
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

        public ColorContainer Read(byte[] data)
        {
            ColorContainer modelContainer = null;
            using (var ms = new MemoryStream(data))
            {
                modelContainer = Read(ms);
            }
            return modelContainer;
        }

        public ColorContainer Read(string filename)
        {
            ColorContainer modelContainer = null;
            if (File.Exists(filename))
            {
                using (var fs = File.OpenRead(filename))
                {
                    modelContainer = Read(fs);
                }
            }
            return modelContainer;
        }

        public ColorContainer Read(Stream s)
        {
            var colorContainer = new ColorContainer();
            Stopwatch stopwatch = Stopwatch.StartNew();
            colorContainer.ColorVersion = ReadInt(s);
            int NumOfColors = ReadInt(s);
            for (int i = 0; i < NumOfColors; i++)
            {
                var col = new ColorContainer.Color();
                col.Name = ReadString(s);
                col.ColorPallette = System.Drawing.Color.FromArgb(ReadInt(s));
                colorContainer.Colors.Add(col);
            }
            int NumOfWaterColors = ReadInt(s);
            for (int i = 0; i < NumOfWaterColors; i++)
            {
                var col = new ColorContainer.WaterColor();
                col.Name = ReadString(s);
                col.SurfaceColor = System.Drawing.Color.FromArgb(ReadInt(s));
                col.UnderwaterColor = System.Drawing.Color.FromArgb(ReadInt(s));
                col.FogColor = System.Drawing.Color.FromArgb(ReadInt(s));
                colorContainer.WaterColors.Add(col);
            }
            stopwatch.Stop();
            Debug.WriteLine("Completed in: " + stopwatch.Elapsed, category: nameof(COLFileReader.Read));
            return colorContainer;
        }

        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.ASCII);
        }
    }
}
