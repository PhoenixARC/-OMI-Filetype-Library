using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * all known Model/Material information is the direct product of May/MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespa
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
