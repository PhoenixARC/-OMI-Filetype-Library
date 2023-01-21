using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Workers.FUI;

namespace OMI_Filetype_Library_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0].Equals("--load") && args.Length >= 2 && File.Exists(args[1]))
            {
                switch (Path.GetExtension(args[1]))
                {
                    case ".fui":
                        Console.WriteLine(new FourjUIReader().Read(args[1]).Header);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
