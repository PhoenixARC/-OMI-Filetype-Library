using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Material;
using OMI.Workers;
/*
* all known Model/Material information is the direct product of May/MattNL's work! check em out! 
* https://github.com/MattN-L
*/
namespace OMI.Workers.Material
{
    internal class MaterialFileWriter : IDataFormatWriter
    {
        private readonly MaterialContainer container;

        public MaterialFileWriter(MaterialContainer container)
        {
            this.container = container;
        }

        public void WriteToStream(Stream s)
        {
            using (var writer = new EndiannessAwareBinaryWriter(s))
            {
                writer.Write(container.Version);
                writer.Write(container.materials.Count);
                foreach (Formats.Material.Material material in container.materials)
                {
                    writer.Write((short)material.Name.Length);
                    writer.WriteString(material.Name, Encoding.ASCII);
                    writer.Write((short)material.Type.Length);
                    writer.WriteString(material.Type, Encoding.ASCII);
                }
            }
        }

        public void WriteToFile(string filename)
        {
            using (var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }
    }
}
