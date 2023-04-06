using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Workers
{
    internal interface IDataFileFormat
    {
        string FileExtention { get; }
        IDataFormatReader Reader { get; }
        IDataFormatWriter Writer { get; }
    }
}