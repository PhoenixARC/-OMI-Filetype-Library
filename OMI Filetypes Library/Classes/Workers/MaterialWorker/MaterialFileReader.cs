using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using OMI.Formats.Material;
using OMI.Workers;
/*
* all known Model/Material information is the direct product of May/MattNL's work! check em out! 
* https://github.com/MattN-L
*/
namespace OMI.Workers.Material
{
    public class MaterialFileReader : IDataFormatReader<MaterialContainer>, IDataFormatReader
    {
        public MaterialContainer FromFile(string filename)
        {
            if (File.Exists(filename))
            {
                MaterialContainer modelContainer = null;
                using (var fs = File.OpenRead(filename))
                {
                    modelContainer = FromStream(fs);
                }
                return modelContainer;
            }
            throw new FileNotFoundException(filename);
        }

        public MaterialContainer FromStream(Stream stream)
        {
            var container = new MaterialContainer();
            using (var reader = new EndiannessAwareBinaryReader(stream, encoding: Encoding.ASCII, leaveOpen: true))
            {
                Stopwatch stopwatch = Stopwatch.StartNew();
                container.Version = reader.ReadInt32();
                int NumOfMaterials = reader.ReadInt32();
                for (int i = 0; i < NumOfMaterials; i++)
                {
                    Formats.Material.Material mat = new Formats.Material.Material();
                    mat.Name = reader.ReadString(reader.ReadInt16());
                    mat.Type = reader.ReadString(reader.ReadInt16());
                    container.materials.Add(mat);
                }
                stopwatch.Stop();
                Debug.WriteLine("Completed in: " + stopwatch.Elapsed, category: nameof(MaterialFileReader.FromStream));
            }
            return container;
        }

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);
    }
}
