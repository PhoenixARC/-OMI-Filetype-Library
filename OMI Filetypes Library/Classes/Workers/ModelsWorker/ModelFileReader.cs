using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using OMI.Formats.Model;
using OMI.utils;
/*
 * all known Model/Material information is the direct product of May/MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Workers.Model
{
    internal class ModelFileReader : StreamDataReader
    {

        public ModelFileReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }
        public ModelContainer Read(byte[] locData)
        {
            ModelContainer modelContainer = new ModelContainer();
            MemoryStream s = new MemoryStream(locData);
            return Read(modelContainer, s);
        }

        public ModelContainer Parse(string Filepath)
        {
            ModelContainer modelContainer = new ModelContainer();
            if (File.Exists(Filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(Filepath, FileMode.Open)))
                {
                    Stream baseStream = binaryReader.BaseStream;
                    modelContainer = Read(modelContainer, baseStream);
                }
            }
            return modelContainer;
        }

        public ModelContainer Read(ModelContainer Mc, Stream s)
        {
            ReadInt(s); // Get Models.bin version
            int NumOfModels = ReadInt(s); // Get Number Of Models

            for (int i = 0; i < NumOfModels; i++)
            {
                string Modelname = ReadString(s); // get model name
                ModelPiece mp = new ModelPiece();
                mp.TextureHeight = ReadInt(s); // get texture height
                mp.TextureWidth = ReadInt(s); // get texture width
                int NumOfParts = ReadInt(s); // get number of parts in model

                for (int j = 0; j < NumOfParts; j++)
                {
                    ModelPart mpart = new ModelPart();
                    string ModelPartname = ReadString(s); // get model part name
                    mpart.TranslationX = ReadFloat(s); // get model part translation in X dimension
                    mpart.TranslationY = ReadFloat(s); // get model part translation in Y dimension
                    mpart.TranslationZ = ReadFloat(s); // get model part translation in Z dimension
                    mpart.UnknownFloat = ReadFloat(s); // get Unknown Floating Point number
                    mpart.TextureOffsetX = ReadFloat(s); // get model part Texture Offset in X dimension
                    mpart.TextureOffsetY = ReadFloat(s); // get model part Texture Offset in Y dimension
                    mpart.RotationX = ReadFloat(s); // get model part Rotation in X dimension
                    mpart.RotationY = ReadFloat(s); // get model part Rotation in Y dimension
                    mpart.RotationZ = ReadFloat(s); // get model part Rotation in Z dimension
                    int NumOfBoxes = ReadInt(s); // get number of boxes in model part

                    for (int x = 0; x < NumOfBoxes; x++)
                    {
                        ModelBox mb = new ModelBox();
                        mb.PositionX = ReadFloat(s); // get part box position in X dimension
                        mb.PositionY = ReadFloat(s); // get part box position in Y dimension
                        mb.PositionZ = ReadFloat(s); // get part box position in Z dimension
                        mb.Length = ReadInt(s); // get part box Length
                        mb.Height = ReadInt(s); // get part box Height
                        mb.Width = ReadInt(s); // get part box Width
                        mb.UvX = ReadFloat(s); // get part box Texture UV in X dimension
                        mb.UvY = ReadFloat(s); // get part box Texture UV in Y dimension
                        mb.Scale = ReadFloat(s); // get part box Scale
                        mb.Mirror = ReadBool(s);
                        mpart.Boxes.Add(x.ToString(), mb);
                    }
                    mp.Parts.Add(ModelPartname, mpart);

                }
                Mc.models.Add(Modelname, mp);
            }

            return Mc;
        }
        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.UTF8);
        }

    }
}
