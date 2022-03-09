using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIWorker.model
{
    public class UIComponent
    {
        #region Base FUI Structures

        public class fuiHeader
        {
            public char[] Signature = new char[0x08];
            public int ContentSize;
            public char[] SwfFileName = new char[0x40];
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
            public char[] StringArg0 = new char[0x40];
            public char[] StringArg1 = new char[0x40];
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
            public char[] htmltextformat = new char[(0x100)];
        }
        public class fuiFontName
        {
            public int ID;
            public char[] FontName = new char[0x40];
            public int Unknown0;
            public char[] Unknown1 = new char[0x40];
            public int Unknown2;
            public int Unknown3;
            public char[] Unknown4 = new char[0x40];
            public int Unknown5;
            public int Unknown6;
            public char[] Unknown7 = new char[0x2c];
        }
        public class fuiSymbol
        {
            public char[] SymbolName = new char[0x40];
            public int ObjectType;
            public int Index;
        }
        public class fuiImportAsset
        {
            public char[] Name = new char[0x40];
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
        }
        public class fuiRGBA
        {
            public byte R;
            public byte G;
            public byte B;
            public byte A;
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
        public class fuiObject_eFuiObjectType
        {
            public int fuiObjectType;
        }
        public class fuiImage
        {
            public byte[] data;
            public int Format;
        }

        #endregion

    }
}
