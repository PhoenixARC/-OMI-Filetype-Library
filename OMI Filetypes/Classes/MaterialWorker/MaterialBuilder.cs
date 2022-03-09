using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MaterialWorker.model;

namespace MaterialWorker
{
    public class MaterialBuilder
    {
        public void Build(MaterialContainer Mc, string FilePath)
        {
            int OSet = 0;
            FileStream fs = new FileStream(FilePath, FileMode.Create, FileAccess.Write);
            fs.Write(BitConverter.GetBytes(Mc.Version), OSet, 1);
            OSet += 3;
            fs.Write(BitConverter.GetBytes(Mc.materials.Count), OSet, 1);
            OSet += 3;
            foreach (Material mat in Mc.materials)
            {
                fs.Write(BitConverter.GetBytes((Int16)(mat.MaterialName.Length)), OSet, 1);
                OSet += 1;
                fs.Write(Encoding.Default.GetBytes(mat.MaterialName), OSet, 1);
                OSet += mat.MaterialName.Length-1;
                fs.Write(BitConverter.GetBytes((Int16)(mat.MaterialType.Length)), OSet, 1);
                OSet += 1;
                fs.Write(Encoding.Default.GetBytes(mat.MaterialType), OSet, 1);
                OSet += mat.MaterialType.Length-1;
            }
        }
    }
}
