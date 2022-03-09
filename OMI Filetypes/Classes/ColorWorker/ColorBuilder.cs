using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using ColorWorker.model;

namespace ColorWorker
{
    public class ColorBuilder
    {

        public byte[] Build(ColorContainer Cc)
        {
            List<byte> OutputBytes = new List<byte>();

            OutputBytes.AddRange(BitConverter.GetBytes(Cc.ColorVersion).Reverse().ToArray());
            OutputBytes.AddRange(BitConverter.GetBytes(Cc.Colors.Count).Reverse().ToArray());
            foreach(ColorComponent.Color col in Cc.Colors)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(Int16.Parse(col.Name.Length.ToString())).Reverse().ToArray());
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(col.Name));
                OutputBytes.Add(col.SplitterByte);
                OutputBytes.Add((byte)(col.ColorPallette.R));
                OutputBytes.Add((byte)(col.ColorPallette.G));
                OutputBytes.Add((byte)(col.ColorPallette.B));
            }
            OutputBytes.AddRange(BitConverter.GetBytes(Cc.WaterColors.Count).Reverse().ToArray());
            foreach (ColorComponent.WaterColor col in Cc.WaterColors)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(Int16.Parse(col.Name.Length.ToString())).Reverse().ToArray());
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(col.Name));
                OutputBytes.Add(col.SplitterByte);
                OutputBytes.Add((byte)(col.surfaceColor.R));
                OutputBytes.Add((byte)(col.surfaceColor.G));
                OutputBytes.Add((byte)(col.surfaceColor.B));
                OutputBytes.Add(0x00);
                OutputBytes.Add((byte)(col.underwaterColor.R));
                OutputBytes.Add((byte)(col.underwaterColor.G));
                OutputBytes.Add((byte)(col.underwaterColor.B));
                OutputBytes.Add(0x00);
                OutputBytes.Add((byte)(col.fogColor.R));
                OutputBytes.Add((byte)(col.fogColor.G));
                OutputBytes.Add((byte)(col.fogColor.B));
            }

            return OutputBytes.ToArray();
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

    }
}
