using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.Material
{
    public class MaterialContainer
    {
        public int Version;
        public List<Material> materials = new List<Material>();
    }
    public class Material
    {
        public string MaterialName;
        public string MaterialType;
    }
}
