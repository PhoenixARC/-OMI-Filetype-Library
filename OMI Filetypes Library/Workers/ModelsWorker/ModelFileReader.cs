using System.Text;
using System.IO;
using OMI.Formats.Model;
using System.Diagnostics;
/*
* all known Model/Material information is the direct product of May/MattNL's work! check em out! 
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
                int version = reader.ReadInt32();
                int NumOfModels = reader.ReadInt32();

                for (int i = 0; i < NumOfModels; i++)
                {
                    string modelName = ReadString(reader);
                    Formats.Model.Model model = new Formats.Model.Model
                    {
                        Name = modelName,
                        TextureSize = new System.Drawing.Size(reader.ReadInt32(), reader.ReadInt32())
                    };
                    int NumOfParts = reader.ReadInt32();

                    for (int j = 0; j < NumOfParts; j++)
                    {
                        ModelPart part = new ModelPart();
                        string partName = ReadString(reader);

                        if (version > 1)
                        {
                            string partParentName = ReadString(reader);
                            Debug.WriteLineIf(partParentName.Length > 0, partParentName, category: nameof(ModelFileReader.FromStream));
                        }

                        part.TranslationX = reader.ReadSingle();
                        part.TranslationY = reader.ReadSingle();
                        part.TranslationZ = reader.ReadSingle();
                        part.UnknownFloat = reader.ReadSingle();
                        part.TextureOffsetX = reader.ReadSingle();
                        part.TextureOffsetY = reader.ReadSingle();
                        part.RotationX = reader.ReadSingle();
                        part.RotationY = reader.ReadSingle();
                        part.RotationZ = reader.ReadSingle();
                        int NumOfBoxes = reader.ReadInt32();

                        for (int x = 0; x < NumOfBoxes; x++)
                        {
                            ModelBox box = new ModelBox();
                            box.PositionX = reader.ReadSingle();
                            box.PositionY = reader.ReadSingle();
                            box.PositionZ = reader.ReadSingle();
                            box.Length = reader.ReadInt32();
                            box.Height = reader.ReadInt32();
                            box.Width = reader.ReadInt32();
                            box.UvX = reader.ReadSingle();
                            box.UvY = reader.ReadSingle();
                            box.Scale = reader.ReadSingle();
                            box.Mirror = reader.ReadBoolean();
                            part.Boxes.Add(box);
                        }
                        model.Parts.Add(partName, part);

                    }
                    container.Models.Add(modelName, model);
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
