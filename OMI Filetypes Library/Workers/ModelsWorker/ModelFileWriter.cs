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
        private int fileVersion;
        private readonly ModelContainer container;

        public ModelFileWriter(ModelContainer container, int version)
        {
            fileVersion = version;
            this.container = container;
        }

        public void WriteToFile(string filename)
        {
            using (FileStream fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream, Endianness.BigEndian))
            {
                writer.Write(fileVersion);
                writer.Write(container.ModelCount);

                foreach (Formats.Model.Model model in container)
                {
                    WriteString(writer, model.Name);
                    writer.Write(model.TextureSize.Width);
                    writer.Write(model.TextureSize.Height);
                    writer.Write(model.Parts.Count);
                    foreach (ModelPart part in model.Parts.Values)
                    {
                        WriteString(writer, part.Name);
                        if (fileVersion > 1)
                        {
                            // in case part doesn't have parent
                            WriteString(writer, part.ParentName ?? string.Empty);
                        }
                        writer.Write(part.Translation.X);
                        writer.Write(part.Translation.Y);
                        writer.Write(part.Translation.Z);
                        writer.Write(part.Rotation.X);
                        writer.Write(part.Rotation.Y);
                        writer.Write(part.Rotation.Z);

                        if (fileVersion > 0)
                        {
                            writer.Write(part.AdditionalRotation.X);
                            writer.Write(part.AdditionalRotation.Y);
                            writer.Write(part.AdditionalRotation.Z);
                        }
                        writer.Write(part.Boxes.Count);
                        foreach (ModelBox box in part.Boxes)
                        {
                            writer.Write(box.Position.X);
                            writer.Write(box.Position.Y);
                            writer.Write(box.Position.Z);
                            writer.Write(box.Size.X);
                            writer.Write(box.Size.Y);
                            writer.Write(box.Size.Z);
                            writer.Write(box.Uv.X);
                            writer.Write(box.Uv.Y);
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
            writer.WriteString(s);
        }
    }
}