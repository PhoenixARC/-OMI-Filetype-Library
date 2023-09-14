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

        public void DumpMSSCMP(string directory)
        {
            foreach (KeyValuePair<string, MilesSoundSource> source in _archive.Sources)
            {
                string FullFilePath = directory + "\\" + source.Key;

                if (!Directory.Exists(Path.GetDirectoryName(FullFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FullFilePath));
                }
                File.WriteAllBytes(FullFilePath + ".binka", source.Value.data);
            }
            foreach (KeyValuePair<string, MilesSoundEvent> Event in _archive.Events)
            {
                string FullFilePath = directory + "\\" + Event.Key;

                if (!Directory.Exists(Path.GetDirectoryName(FullFilePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(FullFilePath));
                }

                StringBuilder sb = new StringBuilder();

                sb.Append(Event.Value.unknown1 + ";");
                sb.Append(Event.Value.unknown2 + ";");
                sb.Append(Event.Value.unknown3 + ";");
                if(Event.Value.IsCache)
                    sb.Append(Event.Value.unknown4 + ";");
                foreach(KeyValuePair<string, int> Sounds in Event.Value.SoundPaths)
                {
                    if(!Event.Value.IsCache)
                        sb.Append(Sounds.Key + ":" + Sounds.Value + ":");
                    else
                        sb.Append(Sounds.Key + ":");
                }
                if (!Event.Value.IsCache)
                    sb.Append(Event.Value.unknown4 + ";");
                sb.Append(Event.Value.unknown5 + ";");
                if (!Event.Value.IsCache)
                {
                    sb.Append(Event.Value.unknown6 + ";");
                    sb.Append(Event.Value.unknown7 + ";");
                    sb.Append(Event.Value.unknown8 + ";");
                    sb.Append(Event.Value.unknown9 + ";");
                    sb.Append(Event.Value.unknown10 + ";");
                    sb.Append(Event.Value.unknown11 + ";");
                    sb.Append(Event.Value.unknown12 + ";");
                    sb.Append(Event.Value.unknown13 + ";");
                    sb.Append(Event.Value.unknown14 + ";");
                    sb.Append(Event.Value.unknown15 + ";");
                    sb.Append(Event.Value.unknown16 + ";");
                    sb.Append(Event.Value.unknown17 + ";");
                    sb.Append(Event.Value.unknown18 + ";");
                    sb.Append(Event.Value.unknown19 + ";");
                    sb.Append(Event.Value.unknown20 + ";");
                    sb.Append(Event.Value.unknown21 + ";");
                    sb.Append(Event.Value.unknown22 + ";");
                    sb.Append(Event.Value.unknown23 + ";");
                    sb.Append(Event.Value.unknown24 + ";");
                    sb.Append(Event.Value.unknown25 + ";");
                }
                File.WriteAllText(FullFilePath, sb.ToString());
            }
        }
    }
}
