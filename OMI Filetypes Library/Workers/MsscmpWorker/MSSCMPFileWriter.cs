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


            var endianness =  Endianness.BigEndian;

            using (EndiannessAwareBinaryWriter writer = new EndiannessAwareBinaryWriter(stream, Encoding.ASCII, endianness))
            {
                writer.Write(MSSCMPFile.SIGN_BE);
                writer.Write(_archive.Version);
                writer.Write(0); // unsure how to calculate Memory usage?
                writer.Write(0);

                if(_archive.Version >= 8)
                    writer.Write(0);

                writer.Write(0); // calculated Events offset
                writer.Write(0);
                writer.Write(0);
                writer.Write(0); // calculated sources offset

                if (_archive.Version >= 8)
                    writer.Write(0);

                writer.Write(_archive.Events.Count);
                writer.Write(0);
                writer.Write(0);
                writer.Write(_archive.Sources.Count);

                //TODO: Reconstruct events and sources, & calculate their respective offsets

            }


        }
    }
}
