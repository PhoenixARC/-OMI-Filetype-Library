using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OMI.Formats.Model;
using OMI.utils;

namespace OMI.Workers.Model
{
    internal class ModelFileWriter : StreamDataWriter
    {

        public ModelFileWriter(bool useLittleEndian) : base(useLittleEndian)
        {
        }
        public void Build(ModelContainer Mc, Stream s)
        {
            WriteInt(s, 1);
            WriteInt(s, Mc.models.Count);

            foreach (KeyValuePair<string, ModelPiece> Model in Mc.models)
            {
                WriteString(s, Model.Key);
                WriteInt(s, Model.Value.TextureHeight);
                WriteInt(s, Model.Value.TextureWidth);
                WriteInt(s, Model.Value.Parts.Count);
                foreach (KeyValuePair<string, ModelPart> Part in Model.Value.Parts)
                {
                    WriteString(s, Part.Key);
                    WriteFloat(s, Part.Value.TranslationX);
                    WriteFloat(s, Part.Value.TranslationY);
                    WriteFloat(s, Part.Value.TranslationZ);
                    WriteFloat(s, Part.Value.UnknownFloat);
                    WriteFloat(s, Part.Value.TextureOffsetX);
                    WriteFloat(s, Part.Value.TextureOffsetY);
                    WriteFloat(s, Part.Value.RotationX);
                    WriteFloat(s, Part.Value.RotationY);
                    WriteFloat(s, Part.Value.RotationZ);
                    WriteInt(s, Part.Value.Boxes.Count);
                    foreach (KeyValuePair<string, ModelBox> box in Part.Value.Boxes)
                    {
                        WriteFloat(s, box.Value.PositionX);
                        WriteFloat(s, box.Value.PositionY);
                        WriteFloat(s, box.Value.PositionZ);
                        WriteInt(s, box.Value.Length);
                        WriteInt(s, box.Value.Height);
                        WriteInt(s, box.Value.Width);
                        WriteFloat(s, box.Value.UvX);
                        WriteFloat(s, box.Value.UvY);
                        WriteFloat(s, box.Value.Scale);
                        WriteBool(s, box.Value.Mirror);

                    }
                }
            }
        }
        private void WriteString(Stream stream, string String)
        {
            WriteShort(stream, (short)String.Length);
            WriteString(stream, String, Encoding.UTF8);
        }
    }
}
