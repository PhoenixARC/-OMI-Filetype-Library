using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialWorker.model;

namespace MaterialWorker
{
    public class MaterialParser
    {

        private ArraySupport ArrSupport;

        public MaterialParser()
        {
            ArrSupport = new ArraySupport();
        }

        public MaterialContainer Parse(byte[] locData, string Filepath)
        {
            MaterialContainer modelContainer = new MaterialContainer();
            MemoryStream s = new MemoryStream(locData);
            return Parse(modelContainer, s, Filepath);
        }

        public MaterialContainer Parse(string Filepath)
        {
            MaterialContainer modelContainer = new MaterialContainer();
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

        public MaterialContainer Parse(MaterialContainer Mc, Stream s, string Filepath)
        {
            DateTime Begin = DateTime.Now;
            Mc.Version = ArrSupport.GetInt32(s);
            int NumOfMaterials = ArrSupport.GetInt32(s);
            for (int i = 0; i < NumOfMaterials; i++)
            {
                Material mat = new Material();
                mat.MaterialName = ArrSupport.GetString(s);
                mat.MaterialType = ArrSupport.GetString(s);
                Mc.materials.Add(mat);
            }
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

            Console.WriteLine("Completed in: " + duration);
            return Mc;
        }
    }
}
