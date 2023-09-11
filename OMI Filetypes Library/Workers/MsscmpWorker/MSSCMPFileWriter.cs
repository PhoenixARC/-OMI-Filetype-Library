using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.Archive;
using OMI.Formats.MilesSoundSystemCompressed;
using OMI.Workers;

namespace OMI.Workers.MSSCMP
{
    public class MSSCMPFileWriter : IDataFormatWriter
    {
        private MSSCMPFile _archive;

        public MSSCMPFileWriter(MSSCMPFile archive)
        {
            _archive = archive;
        }

        public void WriteToFile(string filename)
        {
            using (var fs = File.OpenWrite(filename))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            throw new NotImplementedException();
        }
    }
}
