using System.Text;
using System.IO;
using OMI.Formats.Model;
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
                    string Modelname = ReadString(reader);
                    Formats.Model.Model model = new Formats.Model.Model();
                    model.TextureHeight = reader.ReadInt32();
                    model.TextureWidth = reader.ReadInt32(); 
                    int NumOfParts = reader.ReadInt32();

                    for (int j = 0; j < NumOfParts; j++)
                    {
                        ModelPart mpart = new ModelPart();
                        string ModelPartname = ReadString(reader);
                        mpart.TranslationX = reader.ReadSingle();
                        mpart.TranslationY = reader.ReadSingle();
                        mpart.TranslationZ = reader.ReadSingle();
                        mpart.UnknownFloat = reader.ReadSingle();
                        mpart.TextureOffsetX = reader.ReadSingle();
                        mpart.TextureOffsetY = reader.ReadSingle();
                        mpart.RotationX = reader.ReadSingle();
                        mpart.RotationY = reader.ReadSingle();
                        mpart.RotationZ = reader.ReadSingle();
                        int NumOfBoxes = reader.ReadInt32();

                        for (int x = 0; x < NumOfBoxes; x++)
                        {
                            ModelBox mb = new ModelBox();
                            mb.PositionX = reader.ReadSingle();
                            mb.PositionY = reader.ReadSingle();
                            mb.PositionZ = reader.ReadSingle();
                            mb.Length = reader.ReadInt32();
                            mb.Height = reader.ReadInt32();
                            mb.Width = reader.ReadInt32();
                            mb.UvX = reader.ReadSingle();
                            mb.UvY = reader.ReadSingle();
                            mb.Scale = reader.ReadSingle();
                            mb.Mirror = reader.ReadBoolean();
                            mpart.Boxes.Add(x.ToString(), mb);
                        }
                        model.Parts.Add(ModelPartname, mpart);

                    }
                    container.models.Add(Modelname, model);
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
