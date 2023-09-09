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
        public long version;
    }
}
