using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using OMI.Formats.Color;
using OMI.utils;
/*
 * most known Color information is the direct product of May/MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Workers.Color
{
    internal class COLFileWriter : StreamDataWriter
    {

        private ColorContainer _container;
        public COLFileWriter(ColorContainer container, bool useLittleEndian) : base(useLittleEndian)
        {
            _container = container;
        }
        public void WriteToStream(Stream stream)
        {

            WriteInt(stream, _container.ColorVersion);
            WriteInt(stream, _container.Colors.Count);
            foreach(ColorComponent.Color col in _container.Colors)
            {
                WriteString(stream, col.Name);
                WriteBytes(stream, new[] { col.SplitterByte});
                WriteBytes(stream, new[] { (byte)(col.ColorPallette.R) });
                WriteBytes(stream, new[] { (byte)(col.ColorPallette.G) });
                WriteBytes(stream, new[] { (byte)(col.ColorPallette.B) });
            }
            WriteInt(stream, _container.WaterColors.Count);
            foreach (ColorComponent.WaterColor col in _container.WaterColors)
            {
                WriteString(stream, col.Name);
                WriteBytes(stream, new[] { col.SplitterByte });

                WriteBytes(stream, new[] { (byte)(col.surfaceColor.R) });
                WriteBytes(stream, new[] { (byte)(col.surfaceColor.G) });
                WriteBytes(stream, new[] { (byte)(col.surfaceColor.B) });
                WriteBytes(stream, new[] { (byte)0x00 });
                WriteBytes(stream, new[] { (byte)(col.underwaterColor.R) });
                WriteBytes(stream, new[] { (byte)(col.underwaterColor.G) });
                WriteBytes(stream, new[] { (byte)(col.underwaterColor.B) });
                WriteBytes(stream, new[] { (byte)0x00 });
                WriteBytes(stream, new[] { (byte)(col.fogColor.R) });
                WriteBytes(stream, new[] { (byte)(col.fogColor.G) });
                WriteBytes(stream, new[] { (byte)(col.fogColor.B) });
            }
        }

        public byte[] BuildJSON(ColorContainer Cc, string OutputFile)
        {
            List<byte> OutputBytes = new List<byte>();
            StreamWriter sw = new StreamWriter(OutputFile);
            sw.AutoFlush = true;
            sw.WriteLine("{");

            OutputBytes.AddRange(BitConverter.GetBytes(Cc.ColorVersion).Reverse().ToArray());
            OutputBytes.AddRange(BitConverter.GetBytes(Cc.Colors.Count).Reverse().ToArray());

            sw.WriteLine("\t\"Version\": \"" + Cc.ColorVersion.ToString() + "\",");
            sw.WriteLine("\t\"Colors\": [{");
            foreach (ColorComponent.Color col in Cc.Colors)
            {
                sw.Write("\t\t\"" + col.Name + "\": [{");
                sw.Write("\n\t\t\t\"SeperatorByte\": \"0x");
                sw.Write(BitConverter.ToString(new byte[] { col.SplitterByte }));
                sw.Write("\",\n\t\t\t\"Color\": \"#");
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.ColorPallette.R) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.ColorPallette.G) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.ColorPallette.B) }));
                sw.Write("\"\n\t\t}]");
                if (col == Cc.Colors[Cc.Colors.Count-1])
                    sw.Write("\n");
                else
                    sw.Write(",\n");
            }
            sw.WriteLine("\t}],");
            OutputBytes.AddRange(BitConverter.GetBytes(Cc.WaterColors.Count).Reverse().ToArray());
            sw.WriteLine("\t\"WaterColors\": [{");
            foreach (ColorComponent.WaterColor col in Cc.WaterColors)
            {
                sw.Write("\t\t\""+col.Name+"\": [{");
                sw.Write("\n\t\t\t\"SeperatorByte\": \"0x");
                sw.Write(BitConverter.ToString(new byte[] { col.SplitterByte }));
                sw.Write("\",\n\t\t\t\"surfaceColor\": \"#");
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.surfaceColor.R) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.surfaceColor.G) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.surfaceColor.B) }));
                sw.Write("\",\n\t\t\t\"underwaterColor\": \"#");
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.underwaterColor.R) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.underwaterColor.G) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.underwaterColor.B) }));
                sw.Write("\",\n\t\t\t\"fogColor\": \"#");
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.fogColor.R) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.fogColor.G) }));
                sw.Write(BitConverter.ToString(new byte[] { (byte)(col.fogColor.B) }));
                sw.Write("\"\n\t\t}]");
                if (col == Cc.WaterColors[Cc.WaterColors.Count - 1])
                    sw.Write("\n");
                else
                    sw.Write(",\n");
            }
            sw.WriteLine("\t}]");

            sw.WriteLine("}");
            return OutputBytes.ToArray();
        }

        private void WriteString(Stream stream, string String)
        {
            WriteShort(stream, (short)String.Length);
            WriteString(stream, String, Encoding.UTF8);
        }
    }
}
