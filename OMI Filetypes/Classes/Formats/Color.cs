using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.Color
{
    public class ColorContainer
    {
        public int ColorVersion;
        public List<ColorComponent.Color> Colors = new List<ColorComponent.Color>();
        public List<ColorComponent.WaterColor> WaterColors = new List<ColorComponent.WaterColor>();
    }
    public class ColorComponent
    {
        public class Color
        {
            public string Name;
            public byte SplitterByte;
            public RGB ColorPallette = new RGB();
        }
        public class WaterColor
        {
            public string Name;
            public byte SplitterByte;
            public RGB surfaceColor = new RGB();
            public RGB underwaterColor = new RGB();
            public RGB fogColor = new RGB();
        }
    }
    public class RGB
    {
        public int R;
        public int G;
        public int B;
    }
}
