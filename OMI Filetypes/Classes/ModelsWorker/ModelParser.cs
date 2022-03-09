using System;
using System.Threading;
using System.Collections.Generic;
using System.IO;
using ModelsWorker.model;

namespace ModelsWorker
{
    public class ModelParser
    {

        private ArraySupport ArrSupport;

        public ModelParser()
        {
            ArrSupport = new ArraySupport();
        }

        public ModelContainer Parse(byte[] locData, string Filepath)
        {
            ModelContainer modelContainer = new ModelContainer();
            MemoryStream s = new MemoryStream(locData);
            return Parse(modelContainer, s, Filepath);
        }

        public ModelContainer Parse(string Filepath)
        {
            ModelContainer modelContainer = new ModelContainer();
            if (File.Exists(Filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(Filepath, FileMode.Open)))
                {
                    Stream baseStream = binaryReader.BaseStream;
                    modelContainer = Parse(modelContainer, baseStream, Filepath);
                }
            }
            return modelContainer;
        }

        public ModelContainer Parse(ModelContainer Mc, Stream s, string Filepath)
        {
            ArrSupport.GetInt32(s); // Get Models.bin version
            int NumOfModels = ArrSupport.GetInt32(s); // Get Number Of Models

            for (int i = 0; i < NumOfModels; i++)
            {
                string Modelname = ArrSupport.GetString(s); // get model name
                ModelPiece mp = new ModelPiece();
                mp.TextureHeight = ArrSupport.GetInt32(s); // get texture height
                mp.TextureWidth = ArrSupport.GetInt32(s); // get texture width
                int NumOfParts = ArrSupport.GetInt32(s); // get number of parts in model

                for (int j = 0; j < NumOfParts; j++)
                {
                    ModelPart mpart = new ModelPart();
                    string ModelPartname = ArrSupport.GetString(s); // get model part name
                    ArrSupport.GetInt32(s); // skip over 4 byte buffer space
                    mpart.TranslationX = ArrSupport.Getfloat(s); // get model part translation in X dimension
                    mpart.TranslationY = ArrSupport.Getfloat(s); // get model part translation in Y dimension
                    mpart.TranslationZ = ArrSupport.Getfloat(s); // get model part translation in Z dimension
                    mpart.TextureOffsetX = ArrSupport.Getfloat(s); // get model part Texture Offset in X dimension
                    mpart.TextureOffsetY = ArrSupport.Getfloat(s); // get model part Texture Offset in Y dimension
                    mpart.RotationX = ArrSupport.Getfloat(s); // get model part Rotation in X dimension
                    mpart.RotationY = ArrSupport.Getfloat(s); // get model part Rotation in Y dimension
                    mpart.RotationZ = ArrSupport.Getfloat(s); // get model part Rotation in Z dimension
                    int NumOfBoxes = ArrSupport.GetInt32(s); // get number of boxes in model part

                    for (int x = 0; x < NumOfBoxes; x++)
                    {
                        ModelBox mb = new ModelBox();
                        mb.PositionX = ArrSupport.Getfloat(s); // get part box position in X dimension
                        mb.PositionY = ArrSupport.Getfloat(s); // get part box position in Y dimension
                        mb.PositionZ = ArrSupport.Getfloat(s); // get part box position in Z dimension
                        mb.Length = ArrSupport.GetInt32(s); // get part box Length
                        mb.Height = ArrSupport.GetInt32(s); // get part box Height
                        mb.Width = ArrSupport.GetInt32(s); // get part box Width
                        mb.UvX = ArrSupport.Getfloat(s); // get part box Texture UV in X dimension
                        mb.UvY = ArrSupport.Getfloat(s); // get part box Texture UV in Y dimension
                        mb.Scale = ArrSupport.Getfloat(s); // get part box Scale
                        s.ReadByte();
                        mpart.Boxes.Add(x.ToString(), mb);
                    }
                    mp.Parts.Add(ModelPartname, mpart);

                }
                Mc.models.Add(Modelname, mp);
            }

            return Mc;
        }

    }
}
