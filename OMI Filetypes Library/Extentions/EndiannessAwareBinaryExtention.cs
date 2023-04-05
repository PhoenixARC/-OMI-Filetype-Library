using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMI.Extentions
{
    internal static class EndiannessAwareBinaryExtention
    {
        internal static void Empty<T>(this EndiannessAwareBinaryWriter writer, List<T> list, Action<EndiannessAwareBinaryWriter, T> writeItem)
        {
            foreach (var item in list)
            {
                writeItem.Invoke(writer, item);
            }
        }

        internal static void Fill<T>(this EndiannessAwareBinaryReader reader, List<T> list, Func<EndiannessAwareBinaryReader, T> readItemFunc)
        {
            for (int i = 0; i < list.Capacity; i++)
            {
                list.Add(readItemFunc(reader));
            }
        }
    }
}
