using System;
using System.Collections.Generic;
using System.Globalization;

///! Credits
///! <see cref="https://nicoschertler.wordpress.com/2014/06/04/generic-run-length-encoding-rle-for-c/"/>

namespace OMI.Workers.GameRule
{
    /// <summary>
    /// Provides the RLE codec for any integer data type.
    /// </summary>
    /// <typeparam name="T">The data's type. Must be an integer type or an ArgumentException will be thrown</typeparam>
    static class RLE
    {
        /// <summary>
        /// This is the marker that identifies a compressed run
        /// </summary>
        private static byte rleMarker = byte.MaxValue;

        /// <summary>
        /// A run can be at most as long as the marker - 1
        /// </summary>
        private static byte maxLength = byte.MaxValue - 1;

        /// <summary>
        /// RLE-Encodes a data set.
        /// </summary>
        /// <param name="data">The data to encode</param>
        /// <returns>Encoded data</returns>
        public static IEnumerable<byte> Encode(IEnumerable<byte> data)
        {
            var enumerator = data.GetEnumerator();

            if (!enumerator.MoveNext())
                yield break;

            var firstRunValue = enumerator.Current;
            byte runLength = 1;
            while (enumerator.MoveNext())
            {
                var currentValue = enumerator.Current;
                // if the current value is the value of the current run, don't yield anything, 
                // just extend the run
                if (currentValue.Equals(firstRunValue))
                    runLength++;
                else
                {
                    // the current value is different from the current run
                    // yield what we have so far
                    foreach (var item in MakeRun(firstRunValue, runLength))
                        yield return item;

                    // and reset the run
                    firstRunValue = currentValue;
                    runLength = 1;
                }
                // if there are very many identical values, don't exceed the max length
                if (runLength > maxLength)
                {
                    foreach (var item in MakeRun(firstRunValue, maxLength))
                        yield return item;
                    runLength -= maxLength;
                }
            }
            //yield everything that has been buffered
            foreach (var item in MakeRun(firstRunValue, runLength))
                yield return item;
        }

        /// <summary>
        /// Decodes RLE-encoded data
        /// </summary>
        /// <param name="data">RLE-encoded data</param>
        /// <returns>The original data</returns>
        public static IEnumerable<byte> Decode(IEnumerable<byte> data)
        {
            var enumerator = data.GetEnumerator();
            if (!enumerator.MoveNext())
                yield break;

            do
            {
                var value = enumerator.Current;
                if (!value.Equals(rleMarker))
                {
                    //an ordinary value
                    yield return value;
                }
                else
                {
                    //might be flag or escape
                    //examine the next value
                    if (!enumerator.MoveNext())
                        throw new ArgumentException("The provided data is not properly encoded.");
                    if (enumerator.Current.Equals(rleMarker))
                    {
                        //escaped value
                        yield return value;
                    }
                    else
                    {
                        //rle marker
                        var length = enumerator.Current;
                        if (length > 2 && length <= maxLength)
                        {
                            if (!enumerator.MoveNext())
                                throw new ArgumentException("The provided data is not properly encoded.");
                            var val = enumerator.Current;
                            for (var j = 0; j < length + 1; ++j)
                                yield return val;
                        }
                        else
                        {
                            yield return length;
                        }
                    }
                }
            }
            while (enumerator.MoveNext());
        }

        private static IEnumerable<byte> MakeRun(byte value, byte length)
        {
            if ((length <= 3 && !value.Equals(rleMarker)) || length <= 1)
            {
                //don't compress this run, it is just too small
                for (ulong i = 0; i < length; i++)
                {
                    yield return value.Equals(rleMarker) ? rleMarker : value;
                }
            }
            else
            {
                //compressed run
                yield return rleMarker;
                yield return (byte)(length - 1);
                yield return value;
            }
        }
    }
}