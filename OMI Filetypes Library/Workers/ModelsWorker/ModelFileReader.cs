using System;
using System.Text;
using System.IO;
using OMI.Formats.Model;
using System.Diagnostics;

/*
* all known Model/Material information is the direct product of MattNL's work! check em out! 
* https://github.com/MattN-L
*/
namespace OMI.Workers.Model
{
    public class ModelFileReader : IDataFormatReader<ModelContainer>, IDataFormatReader
    {
        public ModelContainer Read(byte[] data)
        {
            MemoryStream s = new MemoryStream(data);
            return FromStream(s);
        }

        public ModelContainer FromFile(string filename)
        {
            if (File.Exists(filename))
            {
                ModelContainer modelContainer = null;
                using (var fs = File.OpenRead(filename))
                {
                    modelContainer = FromStream(fs);
                }
                return modelContainer;
            }
            throw new FileNotFoundException(filename);
        }

        public ModelContainer FromStream(Stream stream)
        {
            var container = new ModelContainer();
            using (var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, true, Endianness.BigEndian))
            {
                container.Version = reader.ReadInt32();

                int numOfModels = reader.ReadInt32();

                for (int i = 0; i < numOfModels; i++)
                {
                    string modelName = ReadString(reader);
                    int textureWidth = reader.ReadInt32();
                    int textureHeight = reader.ReadInt32();
                    Formats.Model.Model model = new Formats.Model.Model
                    {
                        Name = modelName,
                        TextureSize = new System.Drawing.Size(textureWidth, textureHeight)
                    };
                    int numOfParts = reader.ReadInt32();

                    for (int j = 0; j < numOfParts; j++)
                    {
                        ModelPart part = new ModelPart();
                        part.Name = ReadString(reader);

                        if(container.Version > 1)
                        {
                            part.ParentName = ReadString(reader);
                        }

                        part.Translation.X = reader.ReadSingle();
                        part.Translation.Y = reader.ReadSingle();
                        part.Translation.Z = reader.ReadSingle();
                        
                        part.Rotation.X = reader.ReadSingle();
                        part.Rotation.Y = reader.ReadSingle();
                        part.Rotation.Z = reader.ReadSingle();

                        if (container.Version > 0)
                        {
                            part.AdditionalRotation.X = reader.ReadSingle();
                            part.AdditionalRotation.Y = reader.ReadSingle();
                            part.AdditionalRotation.Z = reader.ReadSingle();
                        }

                        int numOfBoxes = reader.ReadInt32();

                        for (int x = 0; x < numOfBoxes; x++)
                        {
                            ModelBox box = new ModelBox();
                            box.Position.X = reader.ReadSingle();
                            box.Position.Y = reader.ReadSingle();
                            box.Position.Z = reader.ReadSingle();
                            box.Size.X = reader.ReadInt32();
                            box.Size.Y = reader.ReadInt32();
                            box.Size.Z = reader.ReadInt32();
                            box.Uv.X = reader.ReadSingle();
                            box.Uv.Y = reader.ReadSingle();
                            box.Scale = reader.ReadSingle();
                            box.Mirror = reader.ReadBoolean();
                            part.Boxes.Add(box);
                        }
                        model.Parts.Add(part.Name, part);
                    }
                    container.Add(model);
                }
            }
            return container;
        }

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            short length = reader.ReadInt16();
            return reader.ReadString(length);
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
