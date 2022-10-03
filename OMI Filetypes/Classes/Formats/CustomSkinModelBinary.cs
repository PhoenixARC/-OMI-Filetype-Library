using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.CSMB
{
    public class CSMBFile
    {
        public int Version;
        public Dictionary<string, CSMPart> Parts = new Dictionary<string, CSMPart>();
        public Dictionary<PartOffset, float> Offsets = new Dictionary<PartOffset, float>();


        public CSMBFile()
        {

        }


        public class CSMPart
        {
            public string Name;
            public PartParent Parent;
            public float[] Position = new float[3];
            public float[] Size = new float[3];
            public int[] UV = new int[2];
            public bool HideWithArmour = false;
            public bool MirrorTexture = false;
            public float Scale = 1.0f;
        }

        public enum PartParent
        {
            HEAD = 1,
            BODY = 2,
            ARM0 = 3,
            ARM1 = 4,
            LEG0 = 5,
            LEG1 = 6,
        }
        public enum PartOffset
        {
            HEAD,
            BODY,
            ARM0,
            ARM1,
            LEG0,
            LEG1,
            HEADWEAR,
            JACKET,
            SLEEVE0,
            SLEEVE1,
            PANTS0,
            PANTS1,
            WAIST,
            LEGGING0,
            LEGGING1,
            SOCK0,
            SOCK1,
            BOOT0,
            BOOT1,
            ARMARMOR1,
            ARMARMOR0,
            BODYARMOR,
            BELT,
            TOOL0,
            TOOL1,
            HELMET,
            SHOULDER0,
            SHOULDER1,
            CHEST
        }
    }
}
