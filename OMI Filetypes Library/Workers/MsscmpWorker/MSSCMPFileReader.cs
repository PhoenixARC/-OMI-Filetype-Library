using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using OMI.Formats.Archive;
using OMI.Formats.MilesSoundSystemCompressed;
using OMI.Workers;

namespace OMI.Workers.MSSCMP
{
    public class MSSCMPFileReader : IDataFormatReader<MSSCMPFile>, IDataFormatReader
    {
        public MSSCMPFile FromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            using (var fs = File.OpenRead(filename))
            {
                return FromStream(fs);
            }
        }

        public MSSCMPFile FromStream(Stream stream)
        {
            MSSCMPFile _archive = new MSSCMPFile();
            using (EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, Endianness.BigEndian))
            {
                reader.ReadInt32();
                _archive.version = reader.ReadInt32();
                int InfoSize = reader.ReadInt32();
                reader.BaseStream.Position += 8;
                long InfoOffset = reader.ReadInt32();
                reader.BaseStream.Position += 12;
                
                if (_archive.version >= 8)
                    reader.BaseStream.Position += 4;
                
                Console.WriteLine("Version:     " + _archive.version);
                Console.WriteLine("Info Offset: " + InfoOffset);
                Console.WriteLine("Info Size:   " + InfoSize);

                long Events = reader.ReadInt32();
                long FilesOffset2 = reader.ReadInt32();
                long FilesOffset3 = reader.ReadInt32();
                long Sources = reader.ReadInt32();

                reader.BaseStream.Position = InfoOffset;

                Console.WriteLine("Events:  " + Events);
                for(int i = 0; i < Events; i++)
                {
                    KeyValuePair<int, int>  kvp = GetFields(reader, InfoOffset);

                    /*Console.WriteLine(" ├─Name:        " + ReadStringUntil(reader, kvp.Value, 0x00));
                    Console.WriteLine(" ├─Properties:  " + ReadStringUntil(reader, kvp.Key, 0x00));
                    Console.WriteLine(" ├─NameOffset:  " + kvp.Value);
                    Console.WriteLine(" ├─PropertyOffset:  " + kvp.Key);
                    Console.WriteLine(" │");*/
                }
                Console.WriteLine("Sources:  " + Sources);
                for (int i = 0; i < Sources; i++)
                {
                    Console.WriteLine(" ├─Reading From:      " + reader.BaseStream.Position);
                    int PathOff = reader.ReadInt32();
                    int InfoOff = reader.ReadInt32();
                    Console.WriteLine(" ├─PathOffset:        " + PathOff);
                    Console.WriteLine(" ├─InfoOffset:        " + InfoOff);
                    ReadSource(reader, PathOff);
                }
            }
            return _archive;
        }

        public void ReadSource(EndiannessAwareBinaryReader reader, long offset1)
        {
            long currentOffset = reader.BaseStream.Position;
            int receivedSourceNameOffset = reader.ReadInt32();
            int AfterNameOffset = reader.ReadInt32();

            if (offset1 != receivedSourceNameOffset)
            {
                //throw new Exception($"Unexpected offset difference: Expected {offset1}(0x{offset1:x}) got {receivedSourceNameOffset}(0x{receivedSourceNameOffset:x})");
            }

            string Pathname = ReadStringUntil(reader, offset1, 0x00);
            string FileName = ReadStringUntil(reader, AfterNameOffset + currentOffset, 0x00);

            Console.WriteLine(" │ ├─StartOffset:  " + currentOffset);
            Console.WriteLine(" │ ├─afternameOffset:  " + AfterNameOffset);
            Console.WriteLine(" │ ├─PathName:  " + Pathname);
            Console.WriteLine(" │ ├─FileName:  " + FileName);
            Console.WriteLine(" │ ├─SourceName:  " + receivedSourceNameOffset);
            Console.WriteLine(" │ ├─0x8:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─PlayAction:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─0x10:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─sampleRate:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─fileSize:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─Channels:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─0x20:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─durationMilliseconds:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─0x28:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─0x2C:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─0x30:  " + reader.ReadInt32());
            Console.WriteLine(" │ ├─0x34:  " + reader.ReadSingle());
            Console.WriteLine(" │ ├─0x38:  " + reader.ReadInt32());
            Console.ReadLine();
        }
        public KeyValuePair<int, int> GetFields(EndiannessAwareBinaryReader reader, long offset)
        {
            long NameOffset = reader.ReadInt32();
            long SizeOffset = reader.ReadInt32();



            return new KeyValuePair<int, int>((int)SizeOffset, (int)NameOffset);
        }

        public string ReadStringUntil(EndiannessAwareBinaryReader reader, long offset, byte seperator)
        {
            List<byte> bits = new List<byte>();
            long currentPosition = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;
            while (reader.PeekChar() != seperator)
            {
                    bits.Add(reader.ReadByte());
            }
            reader.BaseStream.Position = currentPosition;
            string result = Encoding.UTF8.GetString(bits.ToArray());
            return result;
        }

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            short length = reader.ReadInt16();
            return reader.ReadString(length, Encoding.ASCII);
        }

        private byte[] ReadBytesFromPosition(Stream stream, int position, int size)
        {
            long origin = stream.Position;
            if (stream.Seek(position, SeekOrigin.Begin) != position) throw new Exception();
            byte[] bytes = new byte[size];
            stream.Read(bytes, 0, size);
            if (stream.Seek(origin, SeekOrigin.Begin) != origin) throw new Exception();
            return bytes;
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
