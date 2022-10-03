using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI_Filetypes.Classes.Formats;
using OMI_Filetypes.Classes.Workers.CSMBinaryWorker;

namespace OMI_Filetype_Library
{
    internal class CSMtoCSMB
    {


//Part Name
//Part Parent(HEAD, BODY, LEG0, LEG1, ARM0, ARM1)
//Part Name
//Position-X
        //Position-Y
        //Position-Z
        //Size-X
        //Size-Y
        //Size-Z
        //UV-Y
        //UV-X

        public  List<string> CSMBlock = new List<string>();

        public  CSMBFile TryParse(string CSM)
        {
            CSMBFile NewFile = new CSMBFile();
            int y = 0;
                int i = 0;
                string[] CSMLines = CSM.Split(new[] { "\n", "\r\n" }, StringSplitOptions.None);
                foreach (string line in CSMLines)
                {
                    if (i > 10)
                    {
                        GetModelPartFromCSM(CSMBlock, NewFile, y);
                        CSMBlock.Clear();
                        i = 0;
                    y++;
                    }
                    CSMBlock.Add(line + "\n");
                    i++;
                }

            return NewFile;
        }

        public  void GetModelPartFromCSM(List<string> CSM, CSMBFile csmb, int Instance)
        {
            Random rnd = new Random();
            CSMBFile.CSMPart part = new CSMBFile.CSMPart();
            string PartName = CSM[0].Replace("\n","");
            Console.WriteLine(PartName+Instance);
            string PartParent = CSM[1];
            switch (PartParent)
            {
                case "HEAD":
                    part.Parent = CSMBFile.PartParent.HEAD;
                    break;
                case "BODY":
                    part.Parent = CSMBFile.PartParent.BODY;
                    break;
                case "ARM0":
                    part.Parent = CSMBFile.PartParent.ARM0;
                    break;
                case "ARM1":
                    part.Parent = CSMBFile.PartParent.ARM1;
                    break;
                case "LEG0":
                    part.Parent = CSMBFile.PartParent.LEG0;
                    break;
                case "LEG1":
                    part.Parent = CSMBFile.PartParent.LEG1;
                    break;
            }
            try
            {
                part.Position[0] = float.Parse(CSM[3]);
            }
            catch
            {
                part.Position[0] = 0.0f;
            }
            part.Position[1] = float.Parse(CSM[4]);
            part.Position[2] = float.Parse(CSM[5]);
            part.Size[0] = float.Parse(CSM[6]);
            part.Size[1] = float.Parse(CSM[7]);
            part.Size[2] = float.Parse(CSM[8]);
            part.UV[0] = int.Parse(CSM[9]);
            part.UV[1] = int.Parse(CSM[10]);
            csmb.Parts.Add(PartName + Instance, part);
        }
    }
}
