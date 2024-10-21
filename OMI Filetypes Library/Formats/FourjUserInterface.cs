using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;


/*
 * all known FourJUserInterface information is the direct product of Miku-666(NessieHax)'s work! check em out! 
 * https://github.com/NessieHax
*/
namespace OMI.Formats.FUI
{
    public sealed class FourjUserInterface
    {
        public Components.FuiHeader Header;
        public List<Components.FuiTimeline> Timelines;
        public List<Components.FuiTimelineAction> TimelineActions;
        public List<Components.FuiShape> Shapes;
        public List<Components.FuiShapeComponent> ShapeComponents;
        public List<PointF> Verts;
        public List<Components.FuiTimelineFrame> TimelineFrames;
        public List<Components.FuiTimelineEvent> TimelineEvents;
        public List<string> TimelineEventNames;
        public List<Components.FuiReference> References;
        public List<Components.FuiEdittext> Edittexts;
        public List<Components.FuiFontName> FontNames;
        public List<Components.FuiSymbol> Symbols;
        public List<string> ImportAssets;
        public List<Components.FuiBitmap> Bitmaps;
        public List<byte[]> ImagesData = new List<byte[]>();

        public FourjUserInterface()
        {
            Header = new Components.FuiHeader();
        }
    }

    namespace Components
    {
        public sealed class FuiHeader
        {
            public static readonly byte DefaultVersion = 1;
            public static readonly long DefaultSignature = DefaultVersion << 56 | 0x495546 << 32;

            public byte Version => (byte)(Signature >> 56 & 0xff);

            public long Signature;
            public int ContentSize;
            public string SwfFileName;
            public FuiRect FrameSize = new FuiRect();

            public override string ToString()
            {
                return $"Signature: 0x{Signature.ToString("X16")}\n" +
                    $"Version: {Version}\n" +
                    $"Content Size: {ContentSize}\n" +
                    $"Frame Size: {FrameSize}";
            }
        }

        public sealed class FuiTimeline
        {
            public int SymbolIndex;
            public short FrameIndex;
            public short FrameCount;
            public short ActionIndex;
            public short ActionCount;
            public FuiRect Rectangle = new FuiRect();
        }

        public class FuiTimelineAction
        {
            public byte ActionType;
            public byte Unknown;
            public short FrameIndex;
            public string StringArg0;
            public string StringArg1;
        }

        public class FuiShape
        {
            public int Unknown;
            public int ShapeComponentIndex;
            public int ShapeComponentCount;
            public FuiRect Rectangle = new FuiRect();
        }

        public class FuiShapeComponent
        {
            public FuiFillStyle FillInfo = new FuiFillStyle();
            public int VertIndex;
            public int VertCount;
        }

        public class FuiTimelineFrame
        {
            /// <summary>
            /// Max size: 0x40
            /// </summary>
            public string FrameName;
            public int EventIndex;
            public int EventCount;
        }

        public class FuiTimelineEvent
        {
            public short EventType;
            public short ObjectType;
            public short Unknown0;
            public short Index;
            public short Unknown1;
            public short NameIndex;
            public FuiMatrix Matrix;
            public FuiColorTransform ColorTransform = new FuiColorTransform();
            public UInt32 Color;
        }

        public class FuiReference
        {
            public int SymbolIndex;
            /// <summary>
            /// Max size: 0x40
            /// </summary>
            public string Name;
            public int Index;
        }

        public class FuiEdittext
        {
            public int Unknown0;
            public FuiRect Rectangle = new FuiRect();
            public int FontId;
            public float FontScale;
            public UInt32 Color;
            public int Alignment; // 0 - 3
            public int Unknown3;
            public int Unknown4;
            public int Unknown5;
            public int Unknown6;
            public int Unknown7;
            /// <summary>
            /// Max size: 0x100
            /// </summary>
            public string htmlSource;
        }
        
        public class FuiFontName
        {
            public int ID;
            /// <summary>
            /// Max size: 0x40
            /// </summary>
            public string Name;

            public byte[] UnknownData;
        }
        
        public class FuiSymbol
        {
            /// <summary>
            /// Max size: 0x40
            /// </summary>
            public string Name;
            public int ObjectType;
            public int Index;
        }

        public class FuiBitmap
        {
            public enum FuiImageFormat
            {
                PNG_WITH_ALPHA_DATA = 1,
                PNG_NO_ALPHA_DATA = 3,
                JPEG_NO_ALPHA_DATA = 6,
                JPEG_UNKNOWN = 7,
                /// <summary>
                /// <see cref="ZlibDataOffset"/> has to be set!
                /// </summary>
                JPEG_WITH_ALPHA_DATA = 8
            }

            public int SymbolIndex;
            public FuiImageFormat ImageFormat;
            public Size ImageSize;
            public int Offset;
            public int Size;
            public int ZlibDataOffset;
            /// <summary>
            /// Preserved
            /// </summary>
            public readonly int BindHandle = 0;

            public byte[] ImageData;

            public Image image;

            public void ReverseRGB(Bitmap bmp)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    for (int x = 0; x < bmp.Width; x++)
                    {
                        System.Drawing.Color col = bmp.GetPixel(x, y);
                        bmp.SetPixel(x, y, System.Drawing.Color.FromArgb(col.A, col.B, col.G, col.R));
                    }
                }
            }
        }

        public struct FuiRect
        {
            public Vector2 Min;
            public Vector2 Max;

            public float Width => Max.X - Min.X;
            public float Height => Max.Y - Min.Y;

            public SizeF Size => new SizeF(Width, Height);

            public override string ToString()
            {
                return Size.ToString();
            }
        }

        public struct FuiMatrix
        {
            public SizeF Scale;
            public float RotateSkew0;
            public float RotateSkew1;
            public PointF Translation;
        }
        
        public struct FuiColorTransform
        {
            public float RedMultTerm;
            public float GreenMultTerm;
            public float BlueMultTerm;
            public float AlphaMultTerm;
            public float RedAddTerm;
            public float GreenAddTerm;
            public float BlueAddTerm;
            public float AlphaAddTerm;
        }
        
        public struct FuiFillStyle
        {
            public enum FillType
            {
                Color = 1,
                //Unknown = 3,
                Image = 5,
            }

            public FillType Type;
            public UInt32 Color;
            public int BitmapIndex;
            public FuiMatrix Matrix;
        }

        
        internal enum fuiObject_eFuiObjectType
        {
            STAGE = 0,
            SHAPE = 1,
            TIMELINE = 2,
            BITMAP = 3,
            REFERENCE = 4,
            EDITTEXT = 5,
            CODEGENRECT = 6,
        }

    }
}
