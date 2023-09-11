/*# script for QuickBMS http://quickbms.aluigi.org
 * Copyright (c) 2003-present Luigi Auriemma
 * https://www.gnu.org/licenses/old-licenses/gpl-2.0.txt
**/

using OMI.Formats.Archive;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Formats.MilesSoundSystemCompressed
{
    public class MSSCMPFile : ConsoleArchive
    {
        internal const uint SIGN_LE = 0x4B4E4142;
        internal const uint SIGN_BE = 0x42414E4B;

        public int Version { get; set; }
    }
}
