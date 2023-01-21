using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Material;
using OMI.utils;
/*
 * all known Model/Material information is the direct product of May/MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Workers.Material
{
    public class MaterialFileReader : StreamDataReader
    {
        public MaterialFileReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }


        public MaterialContainer Read(byte[] locData, string Filepath)
        {
            MaterialContainer modelContainer = new MaterialContainer();
            MemoryStream s = new MemoryStream(locData);
            return Read(modelContainer, s);
        }

        public MaterialContainer PaReadrse(string Filepath)
        {
            MaterialContainer modelContainer = new MaterialContainer();
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

        public MaterialContainer Read(MaterialContainer Mc, Stream s)
        {
            DateTime Begin = DateTime.Now;
            Mc.Version = ReadInt(s);
            int NumOfMaterials = ReadInt(s);
            for (int i = 0; i < NumOfMaterials; i++)
            {
                OMI.Formats.Material.Material mat = new OMI.Formats.Material.Material();
                mat.MaterialName = ReadString(s);
                mat.MaterialType = ReadString(s);
                Mc.materials.Add(mat);
            }
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

            Console.WriteLine("Completed in: " + duration);
            return Mc;
        }
        private string ReadString(Stream stream)
        {
            short length = ReadShort(stream);
            return ReadString(stream, length, Encoding.UTF8);
        }
    }
}
