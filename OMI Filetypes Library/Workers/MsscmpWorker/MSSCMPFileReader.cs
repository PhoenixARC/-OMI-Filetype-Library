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

        private Dictionary<string, string[]> ReadEvents(EndiannessAwareBinaryReader reader, int eventOffset, int eventCount)
        {
            Dictionary<string, string[]> Events = new Dictionary<string, string[]>();

            reader.BaseStream.Position = eventOffset;
            Debug.WriteLine("Events: " + eventCount);
            for (int i = 0; i < eventCount; i++)
            {
                long eventNameOffset = reader.ReadInt32();
                long eventDetailsOffset = reader.ReadInt32();

                Events.Add(ReadStringAt(reader, eventNameOffset), ReadStringAt(reader, eventDetailsOffset).Split(';'));

                Debug.WriteLine($" ├─[Name](Offset:{eventNameOffset:x}):        {ReadStringAt(reader, eventNameOffset)}");
                Debug.WriteLine($" ├─[Properties](Offset:{eventDetailsOffset:x}):  {ReadStringAt(reader, eventDetailsOffset)}");
                Debug.WriteLine(" │");
            }
            return Events;
        }

        public Dictionary<string, Dictionary<string, object>> ReadSources(EndiannessAwareBinaryReader reader, int sourceOffset, int sourceCount)
        {

            Dictionary<string, Dictionary<string, object>> Sources = new Dictionary<string, Dictionary<string, object>>();
            reader.BaseStream.Position = sourceOffset;
            Debug.WriteLine("Sources: " + sourceCount);
            for (int i = 0; i < sourceCount; i++)
            {
                int sourcePathOffset = reader.ReadInt32();
                int infoOffset = reader.ReadInt32();
                Debug.WriteLine(" ├─PathOffset:        " + sourcePathOffset);
                Debug.WriteLine(" ├─InfoOffset:        " + infoOffset);
                Dictionary<string, object> source = ReadBankAt(reader, infoOffset, sourcePathOffset);
                Sources.Add((string)source["path"], source);
            }
            return Sources;
        }

        private Dictionary<string, object> ReadBankAt(EndiannessAwareBinaryReader reader, long offset, int sourceOffset)
        {
            Dictionary<string, object> Source = new Dictionary<string, object>();
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
            

            Source.Add("path", pathName);
            Source.Add("file", fileName);
            Source.Add("0x8", reader.ReadInt32());
            Source.Add("PlayAction", reader.ReadInt32());
            Source.Add("0x10", reader.ReadInt32());
            Source.Add("sampleRate", reader.ReadInt32());
            Source.Add("fileSize", reader.ReadInt32());
            Source.Add("Channels", reader.ReadInt32());
            Source.Add("0x20", reader.ReadInt32());
            Source.Add("durationMilliseconds", reader.ReadInt32());
            Source.Add("0x28", reader.ReadInt32());
            Source.Add("0x2C", reader.ReadInt32());
            Source.Add("0x30", reader.ReadInt32());
            Source.Add("0x34", reader.ReadSingle());
            Source.Add("0x38", reader.ReadInt32());

            int DataOffset = int.Parse(((string)Source["file"]).Split('*')[2].Split('.')[0]);


            Source.Add("data", ReadBytesAt(reader, DataOffset, (int)Source["fileSize"]));



            Debug.WriteLine(" │ ├─StartOffset: " + offset);
            Debug.WriteLine(" │ ├─filenameRelativeOffset: " + filenameRelativeOffset);
            Debug.WriteLine(" │ ├─Path: " + Source["path"]);
            Debug.WriteLine(" │ ├─Filename: " + Source["file"]);
            Debug.WriteLine(" │ ├─SourceName: " + receivedSourceNameOffset);
            Debug.WriteLine(" │ ├─0x8: " + Source["0x8"]);
            Debug.WriteLine(" │ ├─PlayAction: " + Source["PlayAction"]);
            Debug.WriteLine(" │ ├─0x10: " + Source["0x10"]);
            Debug.WriteLine(" │ ├─sampleRate: " + Source["sampleRate"]);
            Debug.WriteLine(" │ ├─fileSize: " + Source["fileSize"]);
            Debug.WriteLine(" │ ├─Channels: " + Source["Channels"]);
            Debug.WriteLine(" │ ├─0x20: " + Source["0x20"]);
            Debug.WriteLine(" │ ├─durationMilliseconds: " + Source["durationMilliseconds"]);
            Debug.WriteLine(" │ ├─0x28: " + Source["0x28"]);
            Debug.WriteLine(" │ ├─0x2C: " + Source["0x2C"]);
            Debug.WriteLine(" │ ├─0x30: " + Source["0x30"]);
            Debug.WriteLine(" │ ├─0x34: " + Source["0x34"]);
            Debug.WriteLine(" │ ├─0x38: " + Source["0x38"]);

            reader.BaseStream.Position = origin;

            return Source;
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
