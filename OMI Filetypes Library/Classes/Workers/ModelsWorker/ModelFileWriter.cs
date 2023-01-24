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

        public void WriteToStream(Stream s)
        {
            using (var writer = new EndiannessAwareBinaryWriter(s, Endianness.BigEndian))
            {
                writer.Write(fileVersion);
                writer.Write(container.models.Count);

                foreach (KeyValuePair<string, Formats.Model.Model> model in container.models)
                {
                    WriteString(writer, model.Key);
                    writer.Write(model.Value.TextureHeight);
                    writer.Write(model.Value.TextureWidth);
                    writer.Write(model.Value.Parts.Count);
                    foreach (KeyValuePair<string, ModelPart> part in model.Value.Parts)
                    {
                        WriteString(writer, part.Key);
                        writer.Write(part.Value.TranslationX);
                        writer.Write(part.Value.TranslationY);
                        writer.Write(part.Value.TranslationZ);
                        writer.Write(part.Value.UnknownFloat);
                        writer.Write(part.Value.TextureOffsetX);
                        writer.Write(part.Value.TextureOffsetY);
                        writer.Write(part.Value.RotationX);
                        writer.Write(part.Value.RotationY);
                        writer.Write(part.Value.RotationZ);
                        writer.Write(part.Value.Boxes.Count);
                        foreach (var box in part.Value.Boxes)
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