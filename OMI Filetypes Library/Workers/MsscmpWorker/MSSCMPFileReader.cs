using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using OMI.Formats.MilesSoundSystemCompressed;

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
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, 4);
            int signature = buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3];

            if (signature != MSSCMPFile.SIGN_BE && signature != MSSCMPFile.SIGN_LE)
                throw new Exception("Not a sound bank file.", new Exception($"{signature:x}"));

            var endianness = signature == MSSCMPFile.SIGN_BE ? Endianness.BigEndian : Endianness.LittleEndian;

            using (EndiannessAwareBinaryReader reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, endianness))
            {
                // Header
                _archive.Version = reader.ReadInt32();
                int memoryUsage = reader.ReadInt32();
                _ = reader.ReadInt32(); // ignore

                string filename = ReadString(reader); // ignore

                if (_archive.Version >= 8)
                    reader.BaseStream.Position += 4;

                int eventOffset = reader.ReadInt32();
                _ = reader.ReadInt32(); // unknown
                _ = reader.ReadInt32(); // unknown
                int sourceOffset = reader.ReadInt32();

                if (_archive.Version >= 8)
                    reader.BaseStream.Position += 4;

                int eventCount = reader.ReadInt32();
                _ = reader.ReadInt32(); // unknown
                _ = reader.ReadInt32(); // unknown
                int sourceCount = reader.ReadInt32();

                Debug.WriteLine($"Version: {_archive.Version}");
                Debug.WriteLine($"Memory usage: {memoryUsage:x}");
                Debug.WriteLine($"Event offset: {eventOffset:x}");
                Debug.WriteLine($"Source offset: {sourceOffset:x}");
                // Header End

                _archive.Events = ReadEvents(reader, eventOffset, eventCount);

                _archive.Sources = ReadSources(reader, sourceOffset, sourceCount);
            }
            System.GC.Collect();

            return _archive;
        }

        private Dictionary<string, MilesSoundEvent> ReadEvents(EndiannessAwareBinaryReader reader, int eventOffset, int eventCount)
        {
            Dictionary<string, MilesSoundEvent> Events = new Dictionary<string, MilesSoundEvent>();


            reader.BaseStream.Position = eventOffset;
            Debug.WriteLine("Events: " + eventCount);
            for (int i = 0; i < eventCount; i++)
            {
                MilesSoundEvent Event = new MilesSoundEvent();

                long eventNameOffset = reader.ReadInt32();
                long eventDetailsOffset = reader.ReadInt32();

                string[] eventInfo = ReadStringAt(reader, eventDetailsOffset).Split(';');

                if(eventInfo.Length < 27 && eventInfo.Length != 6)
                {
                    throw new Exception($"Unexpected EventInfo size: Expected 27 or 6, but got {eventInfo.Length}.");
                }

                Dictionary<string, int> SoundPaths = new Dictionary<string, int>();

                string[] Sounds = eventInfo[3].Split(':');
                if (eventInfo.Length == 6)
                {
                    Sounds = eventInfo[4].Split(':');
                    for (int y = 0; y < (Sounds.Length); y ++)
                    {
                        if(!SoundPaths.ContainsKey(Sounds[y]))
                            SoundPaths.Add(Sounds[y], 0);
                    }
                }
                else
                {
                    for (int y = 0; y <= (Sounds.Length / 2); y += 2)
                    {
                        SoundPaths.Add(Sounds[y], int.Parse(Sounds[y + 1]));
                    }
                }


                Event.unknown1 = int.Parse(eventInfo[0]);
                Event.unknown2 = int.Parse(eventInfo[1]);
                Event.unknown3 = int.Parse(eventInfo[2]);
                if (eventInfo.Length != 6)
                {
                    Event.IsCache = false;
                    Event.SoundPaths = SoundPaths;
                    Event.unknown4 = eventInfo[4];
                    Event.unknown5 = eventInfo[5];
                    Event.unknown6 = eventInfo[6];
                    Event.unknown7 = eventInfo[7];
                    Event.unknown8 = eventInfo[8];
                    Event.unknown9 = eventInfo[9];
                    Event.unknown10 = eventInfo[10];
                    Event.unknown11 = int.Parse(eventInfo[11]);
                    Event.unknown12 = int.Parse(eventInfo[12]);
                    Event.unknown13 = int.Parse(eventInfo[13]);
                    Event.unknown14 = int.Parse(eventInfo[14]);
                    Event.unknown15 = int.Parse(eventInfo[15]);
                    Event.unknown16 = int.Parse(eventInfo[16]);
                    Event.unknown17 = int.Parse(eventInfo[17]);
                    Event.unknown18 = eventInfo[18];
                    Event.unknown19 = float.Parse(eventInfo[19]);
                    Event.unknown20 = float.Parse(eventInfo[20]);
                    Event.unknown21 = float.Parse(eventInfo[21]);
                    Event.unknown22 = float.Parse(eventInfo[22]);
                    Event.unknown23 = float.Parse(eventInfo[23]);
                    Event.unknown24 = int.Parse(eventInfo[24]);
                    Event.unknown25 = int.Parse(eventInfo[25]);
                }
                else
                {
                    Event.IsCache = true;
                    Event.SoundPaths = SoundPaths;
                    Event.unknown4 = eventInfo[3];
                    Event.unknown5 = eventInfo[5];
                }

                Events.Add(ReadStringAt(reader, eventNameOffset), Event);

                Debug.WriteLine($" ├─[Name](Offset:{eventNameOffset:x}):        {ReadStringAt(reader, eventNameOffset)}");
                Debug.WriteLine($" ├─[Properties](Offset:{eventDetailsOffset:x}):  {ReadStringAt(reader, eventDetailsOffset)}");
                Debug.WriteLine(" │");
            }
            return Events;
        }

        public Dictionary<string, MilesSoundSource> ReadSources(EndiannessAwareBinaryReader reader, int sourceOffset, int sourceCount)
        {
            Dictionary<string, MilesSoundSource> Sources = new Dictionary<string, MilesSoundSource>();
            reader.BaseStream.Position = sourceOffset;
            Debug.WriteLine("Sources: " + sourceCount);
            for (int i = 0; i < sourceCount; i++)
            {
                int sourcePathOffset = reader.ReadInt32();
                int infoOffset = reader.ReadInt32();
                Debug.WriteLine(" ├─PathOffset:        " + sourcePathOffset);
                Debug.WriteLine(" ├─InfoOffset:        " + infoOffset);
                MilesSoundSource source = ReadBankAt(reader, infoOffset, sourcePathOffset);
                Sources.Add(source.pathName, source);
            }
            return Sources;
        }

        private MilesSoundSource ReadBankAt(EndiannessAwareBinaryReader reader, long offset, int sourceOffset)
        {
            MilesSoundSource source = new MilesSoundSource();

            long origin = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;

            Debug.WriteLine(" ├─Reading From:      " + offset);

            int receivedSourceNameOffset = reader.ReadInt32();
            int filenameRelativeOffset = reader.ReadInt32();

            if (sourceOffset != receivedSourceNameOffset)
            {
                throw new Exception($"Unexpected offset difference: Expected {sourceOffset}(0x{sourceOffset:x}) but got {receivedSourceNameOffset}(0x{receivedSourceNameOffset:x})");
            }

            string pathName = ReadStringAt(reader, sourceOffset);
            string fileName = ReadStringAt(reader, filenameRelativeOffset + offset);
            

            source.pathName = pathName;
            source.fileName = fileName;
            source.Unknown1 = reader.ReadInt32();
            source.PlayAction = reader.ReadInt32();
            source.Unknown2 = reader.ReadInt32();
            source.sampleRate = reader.ReadInt32();
            source.fileSize = reader.ReadInt32();
            source.Channels = reader.ReadInt32();
            source.Unknown3 = reader.ReadInt32();
            source.durationMilliseconds = reader.ReadInt32();
            source.Unknown4 = reader.ReadInt32();
            source.Unknown5 = reader.ReadInt32();
            source.Unknown6 = reader.ReadInt32();
            source.Unknown7 = reader.ReadSingle();
            source.Unknown8 = reader.ReadInt32();


            int DataOffset = int.Parse(((string)source.fileName).Split('*')[2].Split('.')[0]);


            source.data = ReadBytesAt(reader, DataOffset, (int)source.fileSize);



            Debug.WriteLine(" │ ├─StartOffset: " + offset);
            Debug.WriteLine(" │ ├─filenameRelativeOffset: " + filenameRelativeOffset);
            Debug.WriteLine(" │ ├─Path: " + source.pathName);
            Debug.WriteLine(" │ ├─Filename: " + source.fileName);
            Debug.WriteLine(" │ ├─SourceName: " + receivedSourceNameOffset);
            Debug.WriteLine(" │ ├─0x8: " + source.Unknown1);
            Debug.WriteLine(" │ ├─PlayAction: " + source.PlayAction);
            Debug.WriteLine(" │ ├─0x10: " + source.Unknown2);
            Debug.WriteLine(" │ ├─sampleRate: " + source.sampleRate);
            Debug.WriteLine(" │ ├─fileSize: " + source.fileSize);
            Debug.WriteLine(" │ ├─Channels: " + source.Channels);
            Debug.WriteLine(" │ ├─0x20: " + source.Unknown3);
            Debug.WriteLine(" │ ├─durationMilliseconds: " + source.durationMilliseconds);
            Debug.WriteLine(" │ ├─0x28: " + source.Unknown4);
            Debug.WriteLine(" │ ├─0x2C: " + source.Unknown5);
            Debug.WriteLine(" │ ├─0x30: " + source.Unknown6);
            Debug.WriteLine(" │ ├─0x34: " + source.Unknown7);
            Debug.WriteLine(" │ ├─0x38: " + source.Unknown8);

            reader.BaseStream.Position = origin;

            return source;
        }

        // TODO: check encoding for 'IsSingleByte'
        private int GetStringLength(Stream stream)
        {
            long begin = stream.Position;

            if (stream.ReadByte() == '\0')
            {
                stream.Position -= 1;
                return 0;
            }

            int next;
            while ((next = stream.ReadByte()) != -1 && next != '\0')
            { }

            int length = (int)(stream.Position - begin);
            stream.Position = begin;
            return length;
        }

        internal string ReadString(EndiannessAwareBinaryReader reader)
        {
            int length = GetStringLength(reader.BaseStream);
            return reader.ReadString(length);
        }

        internal string ReadStringAt(EndiannessAwareBinaryReader reader, long offset)
        {
            long origin = reader.BaseStream.Position;
            reader.BaseStream.Position = offset;

            string result = ReadString(reader);

            reader.BaseStream.Position = origin;
            return result;
        }

        private byte[] ReadBytesAt(EndiannessAwareBinaryReader reader, long offset, int size)
        {
            long origin = reader.BaseStream.Position;

            byte[] bytes = new byte[size];

            reader.BaseStream.Position = offset;

            for(int i = 0; i < size; i++)
                bytes[i] = reader.ReadByte();

            reader.BaseStream.Position = origin;
            return bytes;
        }
        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
