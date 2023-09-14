/*# script for QuickBMS http://quickbms.aluigi.org
 * Copyright (c) 2003-present Luigi Auriemma
 * https://www.gnu.org/licenses/old-licenses/gpl-2.0.txt
**/

using OMI.Formats.Archive;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.MilesSoundSystemCompressed
{
    public class MSSCMPFile : Dictionary<string, byte[]>
    {
        internal const uint SIGN_LE = 0x4B4E4142;
        internal const uint SIGN_BE = 0x42414E4B;

        public int Version { get; set; }

        public Dictionary<string, MilesSoundEvent> Events { get; set; }

        public Dictionary <string, MilesSoundSource> Sources { get; set; }
    }

    public class MilesSoundEvent
    {
        public bool IsCache { get; set; }
        public int unknown1 { get; set; }
        public int unknown2 { get; set; }
        public int unknown3 { get; set; }
        public Dictionary<string, int> SoundPaths { get; set; }
        public string unknown4 { get; set; }
        public string unknown5 { get; set; }
        public string unknown6 { get; set; }
        public string unknown7 { get; set; }
        public string unknown8 { get; set; }
        public string unknown9 { get; set; }
        public string unknown10 { get; set; }
        public int unknown11 { get; set; }
        public int unknown12 { get; set; }
        public int unknown13 { get; set; }
        public int unknown14 { get; set; }
        public int unknown15 { get; set; }
        public int unknown16 { get; set; }
        public int unknown17 { get; set; }
        public string unknown18 { get; set; }
        public float unknown19 { get; set; }
        public float unknown20 { get; set; }
        public float unknown21 { get; set; }
        public float unknown22 { get; set; }
        public float unknown23 { get; set; }
        public int unknown24 { get; set; }
        public int unknown25 { get; set; }
    }
    public class MilesSoundSource
    {
        public string pathName { get; set; }
        public string fileName { get; set; }
        public int Unknown1 { get; set; }
        public int PlayAction { get; set; }
        public int Unknown2 { get; set; }
        public int sampleRate { get; set; }
        public int fileSize { get; set; }
        public int Channels { get; set; }
        public int Unknown3 { get; set; }
        public int durationMilliseconds { get; set; }
        public int Unknown4 { get; set; }
        public int Unknown5 { get; set; }
        public int Unknown6 { get; set; }
        public float Unknown7 { get; set; }
        public int Unknown8 { get; set; }
        public byte[] data { get; set; }
    }
}
