using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
    public class FourjUserInterface
    {
        public Components.FuiHeader Header;
        public List<Components.FuiTimeline> Timelines;
        public List<Components.FuiTimelineAction> TimelineActions;
        public List<Components.FuiShape> Shapes;
        public List<Components.FuiShapeComponent> ShapeComponents;
        public List<Components.FuiVert> Verts;
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
        public class FuiHeader
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

        public class FuiTimeline
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

        public class FuiVert
        {
            public float X;
            public float Y;
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
            public FuiMatrix Matrix = new FuiMatrix();
            public FuiColorTransform ColorTransform = new FuiColorTransform();
            public System.Drawing.Color Color;
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
            public int FontID;
            public float Unknown1;
            public System.Drawing.Color Color;
            public int Unknown2;
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
            public int SymbolIndex;
            public int ImageFormat;
            public int Width;
            public int Height;
            public int Offset;
            public int Size;
            public int ZlibDataOffset;
            public int Unknown1;
        }

        public class FuiRect
        {
            public float MinX;
            public float MinY;
            public float MaxX;
            public float MaxY;

            public float Width => MaxX - MinX;
            public float Height => MaxY - MinY;

            public override string ToString()
            {
                return string.Format("{0}x{1}", Width, Height);
            }
        }
        
        public static class FuiRGBA
        {
            public static System.Drawing.Color GetColor(int rgba)
            {
                return System.Drawing.Color.FromArgb(rgba & 0xff | rgba >> 8 & 0xffffff);
            }
            
            public static int GetColor(System.Drawing.Color color)
            {
                int argb = color.ToArgb();
                return (argb & 0xffffff) << 8 | argb >> 24 & 0xff;
            }
        }

        public class FuiMatrix
        {
            public float ScaleX;
            public float ScaleY;
            public float RotateSkew0;
            public float RotateSkew1;
            public float TranslationX;
            public float TranslationY;
        }
        
        public class FuiColorTransform
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
        
        public class FuiFillStyle
        {
            public int Type;
            public System.Drawing.Color Color;
            public int BitmapIndex;
            public FuiMatrix Matrix = new FuiMatrix();
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
