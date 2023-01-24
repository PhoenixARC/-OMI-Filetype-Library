using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public Component.fuiHeader Header;
        public List<Component.fuiTimeline> Timelines;
        public List<Component.fuiTimelineAction> TimelineActions;
        public List<Component.fuiShape> Shapes;
        public List<Component.fuiShapeComponent> ShapeComponents;
        public List<Component.fuiVert> Verts;
        public List<Component.fuiTimelineFrame> TimelineFrames;
        public List<Component.fuiTimelineEvent> TimelineEvents;
        public List<Component.fuiTimelineEventName> TimelineEventNames;
        public List<Component.fuiReference> References;
        public List<Component.fuiEdittext> Edittexts;
        public List<Component.fuiFontName> FontNames;
        public List<Component.fuiSymbol> Symbols;
        public List<Component.fuiImportAsset> ImportAssets;
        public List<Component.fuiBitmap> Bitmaps;
        public List<Component.fuiImage> Images = new List<Component.fuiImage>();

        public FourjUserInterface()
        {
            Header = new Component.fuiHeader();
            Timelines = new List<Component.fuiTimeline>();
            TimelineActions = new List<Component.fuiTimelineAction>();
            Shapes = new List<Component.fuiShape>();
            ShapeComponents = new List<Component.fuiShapeComponent>();
            Verts = new List<Component.fuiVert>();
            TimelineFrames = new List<Component.fuiTimelineFrame>();
            TimelineEvents = new List<Component.fuiTimelineEvent>();
            TimelineEventNames = new List<Component.fuiTimelineEventName>();
            References = new List<Component.fuiReference>();
            Edittexts = new List<Component.fuiEdittext>();
            FontNames = new List<Component.fuiFontName>();
            Symbols = new List<Component.fuiSymbol>();
            ImportAssets = new List<Component.fuiImportAsset>();
            Bitmaps = new List<Component.fuiBitmap>();
        }

        public override string ToString()
        {
            return string.Format("Header: {0}", Header);
        }
    }
    namespace Component
    {
        #region Base FUI Structures

        public class fuiHeader
        {
            public static readonly byte DefaultVersion = 1;
            public static readonly long DefaultSignature = DefaultVersion << 56 | 0x495546 << 32;

            public long Signature;
            public int ContentSize;
            public string SwfFileName;
            public int fuiTimelineCount;
            public int fuiTimelineEventNameCount;
            public int fuiTimelineActionCount;
            public int fuiShapeCount;
            public int fuiShapeComponentCount;
            public int fuiVertCount;
            public int fuiTimelineFrameCount;
            public int fuiTimelineEventCount;
            public int fuiReferenceCount;
            public int fuiEdittextCount;
            public int fuiSymbolCount;
            public int fuiBitmapCount;
            public int imagesSize;
            public int fuiFontNameCount;
            public int fuiImportAssetCount;
            public fuiRect frameSize = new fuiRect();

            public override string ToString()
            {
                return $"Signature: 0x{Signature.ToString("X16")}\n" +
                    $"Version: {Signature >> 56 & 0xff}\n" +
                    $"Content Size: {ContentSize}\n" +
                    $"Frame Size: {frameSize}";
            }

        }
        public class fuiTimeline
        {
            public int SymbolIndex;
            public short FrameIndex;
            public short FrameCount;
            public short ActionIndex;
            public short ActionCount;
            public fuiRect Rectangle = new fuiRect();
        }

        public class fuiTimelineAction
        {
            public short ActionType;
            public short Unknown;
            public string StringArg0;
            public string StringArg1;
        }

        public class fuiShape
        {
            public int Unknown;
            public int ShapeComponentIndex;
            public int ShapeComponentCount;
            public fuiRect Rectangle = new fuiRect();
        }

        public class fuiShapeComponent
        {
            public fuiFillStyle FillInfo = new fuiFillStyle();
            public int VertIndex;
            public int VertCount;
        }

        public class fuiVert
        {
            public float X;
            public float Y;
        }

        public class fuiTimelineFrame
        {
            public char[] FrameName = new char[0x40];
            public int EventIndex;
            public int EventCount;
        }


        public class fuiTimelineEvent
        {
            public short EventType;
            public short ObjectType;
            public short Unknown0;
            public short Index;
            public short Unknown1;
            public short NameIndex;
            public fuiMatrix matrix = new fuiMatrix();
            public fuiColorTransform ColorTransform = new fuiColorTransform();
            public fuiRGBA Color = new fuiRGBA();
        }
        public class fuiTimelineEventName
        {
            public char[] EventName = new char[0x40];
        }
        public class fuiReference
        {
            public int SymbolIndex;
            public char[] Name = new char[0x40];
            public int Index;
        }
        public class fuiEdittext
        {
            public int Unknown0;
            public fuiRect Rectangle = new fuiRect();
            public int FontID;
            public float Unknown1;
            public fuiRGBA Color = new fuiRGBA();
            public int Unknown2;
            public int Unknown3;
            public int Unknown4;
            public int Unknown5;
            public int Unknown6;
            public int Unknown7;
            public char[] htmltextformat = new char[0x100];
        }
        public class fuiFontName
        {
            public int ID;
            public char[] FontName = new char[0x40];
        }
        public class fuiSymbol
        {
            public string SymbolName;
            public int ObjectType;
            public int Index;
        }
        public class fuiImportAsset
        {
            public string Name;
        }
        public class fuiBitmap
        {
            public int Unknown0;
            public int ImageFormat;
            public int Width;
            public int Height;
            public int Offset;
            public int Size;
            public int ZlibDataOffset;
            public int Unknown1;
        }

        #endregion

        #region Used structures in an FUI element

        public class fuiRect
        {
            public float MinX;
            public float MinY;
            public float MaxX;
            public float MaxY;

            public override string ToString()
            {
                return string.Format("{0}x{1}", MaxX-MinX, MaxY-MinY);
            }
        }
        
        public class fuiRGBA
        {
            public byte R => (byte)(RGBA >> 24 & 0xff);
            public byte G => (byte)(RGBA >> 16 & 0xff);
            public byte B => (byte)(RGBA >> 08 & 0xff);
            public byte A => (byte)(RGBA >> 00 & 0xff);
            public int RGBA { get; set; }

            public override string ToString()
            {
                return string.Format("#{0}", RGBA);
            }

        }

        public class fuiMatrix
        {
            public float ScaleX;
            public float ScaleY;
            public float RotateSkew0;
            public float RotateSkew1;
            public float TranslationX;
            public float TranslationY;
        }
        
        public class fuiColorTransform
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
        
        public class fuiFillStyle
        {
            public int Type;
            public fuiRGBA Color = new fuiRGBA();
            public int BitmapIndex;
            public fuiMatrix Matrix = new fuiMatrix();
        }
        
        public enum fuiObject_eFuiObjectType
        {
            STAGE = 0,
            SHAPE = 1,
            TIMELINE = 2,
            BITMAP = 3,
            REFERENCE = 4,
            EDITTEXT = 5,
            CODEGENRECT = 6,
        }
        
        public class fuiImage
        {
            public byte[] data;
            public int Format;
        }

        #endregion

    }
}
