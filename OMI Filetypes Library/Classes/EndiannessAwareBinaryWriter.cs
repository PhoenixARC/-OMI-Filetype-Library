/* Copyright (c) 2023-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System;
using System.IO;
using System.Text;

namespace OMI
{
    internal class EndiannessAwareBinaryWriter : BinaryWriter
    {
        private readonly Endianness _endianness = Endianness.LittleEndian;
        private readonly Encoding _encoding = Encoding.UTF8;
        public Encoding EncodingScheme => _encoding;

        public EndiannessAwareBinaryWriter(Stream output) : base(output)
        {
        }

        public EndiannessAwareBinaryWriter(Stream output, Encoding encoding) : base(output, encoding)
        {
            _encoding = encoding;
        }

        public EndiannessAwareBinaryWriter(Stream output, Encoding encoding, bool leaveOpen) : base(output, encoding, leaveOpen)
        {
            _encoding = encoding;
        }

        public EndiannessAwareBinaryWriter(Stream output, Endianness endianness) : base(output)
        {
            _endianness = endianness;
        }

        public EndiannessAwareBinaryWriter(Stream output, Encoding encoding, Endianness endianness) : base(output, encoding)
        {
            _endianness = endianness;
            _encoding = encoding;
        }

        public EndiannessAwareBinaryWriter(Stream output, Encoding encoding, bool leaveOpen, Endianness endianness) : base(output, encoding, leaveOpen)
        {
            _endianness = endianness;
            _encoding = encoding;
        }

        private static void CheckEndiannessAndSwapBuffer(ref byte[] buffer, Endianness endianness)
        {
            if (!BitConverter.IsLittleEndian ||
                endianness == Endianness.BigEndian)
                Array.Reverse(buffer);
        }

        public override void Write(short value) => Write(value, _endianness);
        public override void Write(int value) => Write(value, _endianness);
        public override void Write(long value) => Write(value, _endianness);
        public override void Write(float value) => Write(value, _endianness);

        public void Write(short value, Endianness endianness)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            CheckEndiannessAndSwapBuffer(ref buffer, endianness);
            Write(buffer);
        }

        public void Write(int value, Endianness endianness)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            CheckEndiannessAndSwapBuffer(ref buffer, endianness);
            Write(buffer);
        }

        public void Write(long value, Endianness endianness)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            CheckEndiannessAndSwapBuffer(ref buffer, endianness);
            Write(buffer);
        }

        public void Write(float value, Endianness endianness)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            CheckEndiannessAndSwapBuffer(ref buffer, endianness);
            Write(buffer);
        }

        /// <summary>
        /// Writes a string to the given <see cref="BinaryWriter.BaseStream"/> using the provided <see cref="EncodingScheme"/>
        /// </summary>
        /// <param name="s">String to write</param>
        /// <param name="maxCapacity">Maximum capacity the string can use</param>
        public void WriteString(string s, int maxCapacity) => WriteString(s, maxCapacity, _encoding);

        /// <summary>
        /// Writes a string to the given <see cref="BinaryWriter.BaseStream"/> using the provided <see cref="EncodingScheme"/>
        /// </summary>
        /// <param name="s">String to write</param>
        public void WriteString(string s) => WriteString(s, _encoding);

        public void WriteString(string s, int maxCapacity, Encoding encoding)
        {
            byte[] buffer = new byte[maxCapacity];
            encoding.GetBytes(s, 0, maxCapacity, buffer, 0);
            Write(buffer);
        }

        public void WriteString(string s, Encoding encoding)
        {
            byte[] buffer = encoding.GetBytes(s);
            Write(buffer);
        }
    }
}
