using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OMI.Formats.Model;
using OMI.Workers;
/*
* all known Model/Material information is the direct product of May/MattNL's work! check em out! 
* https://github.com/MattN-L
*/

namespace OMI.Workers.Model
{
    public class ModelFileWriter : IDataFormatWriter
    {
        //! TODO: accept version in the constructor
        private const int fileVersion = 1;
        private readonly ModelContainer container;

        public ModelFileWriter(ModelContainer container)
        {
            this.container = container;
        }

        public void WriteToFile(string filename)
        {
            using (var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream, Endianness.BigEndian))
            {
                writer.Write(fileVersion);
                writer.Write(container.Models.Count);

                foreach (Formats.Model.Model model in container.Models.Values)
                {
                    WriteString(writer, model.Name);
                    writer.Write(model.TextureSize.Width);
                    writer.Write(model.TextureSize.Height);
                    writer.Write(model.Parts.Count);
                    foreach (ModelPart part in model.Parts.Values)
                    {
                        WriteString(writer, part.Name);
                        writer.Write(part.TranslationX);
                        writer.Write(part.TranslationY);
                        writer.Write(part.TranslationZ);
                        writer.Write(part.UnknownFloat);
                        writer.Write(part.TextureOffsetX);
                        writer.Write(part.TextureOffsetY);
                        writer.Write(part.RotationX);
                        writer.Write(part.RotationY);
                        writer.Write(part.RotationZ);
                        writer.Write(part.Boxes.Count);
                        foreach (var box in part.Boxes)
                        {
                            writer.Write(box.PositionX);
                            writer.Write(box.PositionY);
                            writer.Write(box.PositionZ);
                            writer.Write(box.Length);
                            writer.Write(box.Height);
                            writer.Write(box.Width);
                            writer.Write(box.UvX);
                            writer.Write(box.UvY);
                            writer.Write(box.Scale);
                            writer.Write(box.Mirror);

                        }
                    }
                }
            }
        }

        private void WriteString(EndiannessAwareBinaryWriter writer, string s)
        {
            writer.Write((short)s.Length);
            writer.WriteString(s, Encoding.ASCII);
        }
    }
}