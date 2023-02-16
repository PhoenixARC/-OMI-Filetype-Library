using System.Collections.Generic;


/*
 * all known FourJUserInterface information is the direct product of Miku-666(NessieHax)'s work! check em out! 
 * https://github.com/NessieHax
*/
namespace OMI.Formats.FUI
{
    public class FourjUserInterface
    {
        public Components.FuiHeader Header;
        public List<Components.FuiTimeline> Timelines { get; }
        public List<Components.FuiTimelineAction> TimelineActions { get; }
        public List<Components.FuiShape> Shapes { get; }
        public List<Components.FuiShapeComponent> ShapeComponents { get; }
        public List<Components.FuiVert> Verts { get; }
        public List<Components.FuiTimelineFrame> TimelineFrames { get; }
        public List<Components.FuiTimelineEvent> TimelineEvents { get; }
        public List<Components.FuiTimelineEventName> TimelineEventNames { get; }
        public List<Components.FuiReference> References { get; }
        public List<Components.FuiEdittext> Edittexts { get; }
        public List<Components.FuiFontName> FontNames { get; }
        public List<Components.FuiSymbol> Symbols { get; }
        public List<Components.FuiImportAsset> ImportAssets { get; }
        public List<Components.FuiBitmap> Bitmaps { get; }
        public List<byte[]> ImagesData { get; }

        public FourjUserInterface()
        {
            Header = new Components.FuiHeader();
            Timelines = new List<Components.FuiTimeline>();
            TimelineActions = new List<Components.FuiTimelineAction>();
            Shapes = new List<Components.FuiShape>();
            ShapeComponents = new List<Components.FuiShapeComponent>();
            Verts = new List<Components.FuiVert>();
            TimelineFrames = new List<Components.FuiTimelineFrame>();
            TimelineEvents = new List<Components.FuiTimelineEvent>();
            TimelineEventNames = new List<Components.FuiTimelineEventName>();
            References = new List<Components.FuiReference>();
            Edittexts = new List<Components.FuiEdittext>();
            FontNames = new List<Components.FuiFontName>();
            Symbols = new List<Components.FuiSymbol>();
            ImportAssets = new List<Components.FuiImportAsset>();
            Bitmaps = new List<Components.FuiBitmap>();
            ImagesData = new List<byte[]>();
        }
    }

    namespace Components
    {
        public class FuiHeader
        {
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
            public FuiRect frameSize = new FuiRect();
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
            public FuiMatrix matrix = new FuiMatrix();
            public FuiColorTransform ColorTransform = new FuiColorTransform();
            public FuiRGBA Color = new FuiRGBA();
        }

        public class FuiTimelineEventName
        {
            /// <summary>
            /// Max size: 0x40
            /// </summary>
            public string Name;
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
            public FuiRGBA Color = new FuiRGBA();
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
            public string SymbolName;
            public int ObjectType;
            public int Index;
        }
        
        public class FuiImportAsset
        {
            /// <summary>
            /// Max size: 0x40
            /// </summary>
            public string Name;
        }

        public class FuiBitmap
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
        
        public class FuiRGBA
        {
            public byte R => (byte)(RGBA >> 0x18 & 0xff);
            public byte G => (byte)(RGBA >> 0x10 & 0xff);
            public byte B => (byte)(RGBA >> 0x08 & 0xff);
            public byte A => (byte)(RGBA >> 0x00 & 0xff);
            public int RGBA { get; set; }

            public override string ToString()
            {
                return string.Format("#{0}", RGBA);
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
            public FuiRGBA Color = new FuiRGBA();
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
