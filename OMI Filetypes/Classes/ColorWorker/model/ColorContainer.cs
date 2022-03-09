using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorWorker.model
{
    public class ColorContainer
    {
        public int ColorVersion;
        public List<ColorComponent.Color> Colors = new List<ColorComponent.Color>();
        public List<ColorComponent.WaterColor> WaterColors = new List<ColorComponent.WaterColor>();
    }
}
