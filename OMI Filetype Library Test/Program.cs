using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Workers;
using OMI.Workers.Color;
using OMI.Workers.FUI;
using OMI.Workers.Language;
using OMI.Workers.Material;
using OMI.Workers.Model;
using OMI.Workers.Pck;
using OMI.Workers.MSSCMP;

namespace OMI_Filetype_Library_Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length >= 2 && args[0].Equals("--load") && File.Exists(args[1]))
            {
                IDataFormatReader reader = GetFormatReaderFromFilename(args[1]);
                object data = reader.FromFile(args[1]);
                Console.WriteLine(data);
                Console.ReadLine();
            }
        }

        private static IDataFormatReader GetFormatReaderFromFilename(string filename)
        {
            switch (Path.GetExtension(filename))
            {
                case ".fui":
                    return new FourjUIReader();
                case ".col":
                    return new COLFileReader();
                case ".loc":
                    return new LOCFileReader();
                case ".pck":
                    return new PckFileReader();
                case ".msscmp":
                    return new MSSCMPFileReader();
                case ".bin" when Path.GetFileNameWithoutExtension(filename).StartsWith("models"):
                    return new ModelFileReader();
                case ".bin" when Path.GetFileNameWithoutExtension(filename).StartsWith("materials"):
                    return new MaterialFileReader();
                default:
                    throw new NotSupportedException(Path.GetFileName(filename));
            }
        }
    }
}