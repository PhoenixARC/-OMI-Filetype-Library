using System;
using System.IO;
using System.Text;

namespace OMI_Filetypes
{
    internal static class StreamUtil
    {
        public enum Endianness
        {
            BigEndian,
            LittleEndian
        }

        private static bool _isBigEndian = false;

        public static void SetEndianness(Endianness endianness) => _isBigEndian = endianness == Endianness.BigEndian;

        #region Read
        public static byte[] ReadBytes(Stream stream, int count)
        {
            byte[] bytes = new byte[count];
            stream.Read(bytes, 0, count);
            return bytes;
        }

        public static short ReadShort(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 2);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }

        public static int ReadInt(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        public static long ReadLong(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 8);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        public static float ReadFloat(Stream stream)
        {
            byte[] buffer = ReadBytes(stream, 4);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        public static bool ReadBool(Stream stream)
        {
            int val = stream.ReadByte();
            return val != -1 && val > 0;
        }

        public static string ReadString(Stream stream, int length, Encoding encoding)
        {
            byte[] buffer = ReadBytes(stream, length << Convert.ToInt32(encoding is UnicodeEncoding));
            return encoding.GetString(buffer).Trim('\0');
        }
        #endregion

        #region Write
        public static void WriteBytes(Stream stream, byte[] bytes, int count)
        {
            stream.Write(bytes, 0, count);
        }

        public static void WriteShort(Stream stream, short value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            WriteBytes(stream, buffer, 2);
        }

        public static void WriteInt(Stream stream, int value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            WriteBytes(stream, buffer, 4);
        }

        public static void WriteLong(Stream stream, long value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            WriteBytes(stream, buffer, 8);
        }

        public static void WriteFloat(Stream stream, float value)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            if (!BitConverter.IsLittleEndian || _isBigEndian)
                Array.Reverse(buffer);
            WriteBytes(stream, buffer, 4);
        }

        public static void WriteByte(Stream stream, byte value)
        {
            stream.WriteByte(value);
        }

        public static void WriteBool(Stream stream, bool value)
        {
            stream.WriteByte(Convert.ToByte(value));
        }

        public static void WriteString(Stream stream, string s, int maxCapacity, Encoding encoding)
        {
            byte[] buffer = new byte[maxCapacity];
            encoding.GetBytes(s, 0, maxCapacity, buffer, 0);
            WriteBytes(stream, buffer, maxCapacity);
        }

        public static void WriteString(Stream stream, string s, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(s);
            WriteBytes(stream, buffer, buffer.Length);
        }
        #endregion
    }
}
