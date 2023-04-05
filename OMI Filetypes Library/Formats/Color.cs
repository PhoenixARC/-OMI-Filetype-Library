using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
/*
 * most known Color information is the direct product of May/MattNL's work! check em out! 
 * https://github.com/MattN-L
*/
namespace OMI.Formats.Color
{
    public class ColorContainer
    {
        public int Version;
        public List<Color> Colors = new List<Color>();
        public List<WaterColor> WaterColors = new List<WaterColor>();

        public class Color
        {
            public string Name;
            public System.Drawing.Color ColorPallette;
        }

        public class WaterColor
        {
            public string Name;
            public System.Drawing.Color SurfaceColor;
            public System.Drawing.Color UnderwaterColor;
            public System.Drawing.Color FogColor;
        }
    }
}
