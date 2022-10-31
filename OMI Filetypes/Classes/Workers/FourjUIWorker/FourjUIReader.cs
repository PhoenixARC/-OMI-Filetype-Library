using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.FUI;
using OMI.utils;
/*
 * all known FourJUserInterface information is the direct product of Miku-666(NessieHax)'s work! check em out! 
 * https://github.com/NessieHax
*/
namespace OMI.Workers.FUI
{
    internal class FourjUIReader : StreamDataReader
    {
        public FourjUIReader(bool useLittleEndian) : base(useLittleEndian)
        {
        }

        public FourjUserInterface Read(byte[] FileData)
        {
            FourjUserInterface UserInterfaceContainer = new FourjUserInterface();
            MemoryStream s = new MemoryStream(FileData);
            return Read(UserInterfaceContainer, s);
        }

        public FourjUserInterface Read(string Filepath)
        {
            FourjUserInterface UserInterfaceContainer = new FourjUserInterface();
            if (File.Exists(Filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(Filepath, FileMode.Open)))
                {
                    Stream baseStream = binaryReader.BaseStream;
                    UserInterfaceContainer = Read(UserInterfaceContainer, baseStream);
                }
            }
            return UserInterfaceContainer;
        }

        public FourjUserInterface Read(FourjUserInterface UIContainer, Stream s)
        {
            DateTime Begin = DateTime.Now;
            UIContainer.Header.Signature = ReadCharArray(s, (int)0x08);
            UIContainer.Header.ContentSize = ReadInt(s);
            UIContainer.Header.SwfFileName = ReadCharArray(s, (int)0x40);
            UIContainer.Header.fuiTimelineCount = ReadInt(s);
            UIContainer.Header.fuiTimelineEventNameCount = ReadInt(s);
            UIContainer.Header.fuiTimelineActionCount = ReadInt(s);
            UIContainer.Header.fuiShapeCount = ReadInt(s);
            UIContainer.Header.fuiShapeComponentCount = ReadInt(s);
            UIContainer.Header.fuiVertCount = ReadInt(s);
            UIContainer.Header.fuiTimelineFrameCount = ReadInt(s);
            UIContainer.Header.fuiTimelineEventCount = ReadInt(s);
            UIContainer.Header.fuiReferenceCount = ReadInt(s);
            UIContainer.Header.fuiEdittextCount = ReadInt(s);
            UIContainer.Header.fuiSymbolCount = ReadInt(s);
            UIContainer.Header.fuiBitmapCount = ReadInt(s);
            UIContainer.Header.imagesSize = ReadInt(s);
            UIContainer.Header.fuiFontNameCount = ReadInt(s);
            UIContainer.Header.fuiImportAssetCount = ReadInt(s);
            UIContainer.Header.frameSize.MinX = ReadFloat(s);
            UIContainer.Header.frameSize.MaxX = ReadFloat(s);
            UIContainer.Header.frameSize.MinY = ReadFloat(s);
            UIContainer.Header.frameSize.MaxY = ReadFloat(s);
            for (int i = 0; i < UIContainer.Header.fuiTimelineCount; i++)
            {
                UIComponent.fuiTimeline tline = new UIComponent.fuiTimeline();
                tline.SymbolIndex = ReadInt(s);
                tline.FrameIndex = ReadShort(s);
                tline.FrameCount = ReadShort(s);
                tline.ActionIndex = ReadShort(s);
                tline.ActionCount = ReadShort(s);
                tline.Rectangle.MinX = ReadInt(s);
                tline.Rectangle.MaxX = ReadInt(s);
                tline.Rectangle.MinY = ReadInt(s);
                tline.Rectangle.MaxY = ReadInt(s);
                UIContainer.Timelines.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiTimelineActionCount; i++)
            {
                UIComponent.fuiTimelineAction tline = new UIComponent.fuiTimelineAction();
                tline.ActionType = ReadShort(s);
                tline.Unknown = ReadShort(s);
                tline.StringArg0 = ReadCharArray(s, (int)0x40);
                tline.StringArg1 = ReadCharArray(s, (int)0x40);
                UIContainer.TimelineActions.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiShapeCount; i++)
            {
                UIComponent.fuiShape tline = new UIComponent.fuiShape();
                tline.Unknown = ReadInt(s);
                tline.ShapeComponentIndex = ReadInt(s);
                tline.ShapeComponentCount = ReadInt(s);
                tline.Rectangle.MinX = ReadInt(s);
                tline.Rectangle.MaxX = ReadInt(s);
                tline.Rectangle.MinY = ReadInt(s);
                tline.Rectangle.MaxY = ReadInt(s);
                UIContainer.Shapes.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiShapeComponentCount; i++)
            {
                UIComponent.fuiShapeComponent tline = new UIComponent.fuiShapeComponent();
                tline.FillInfo.Type = ReadInt(s);
                tline.FillInfo.Color.R = ReadBytes(s, 1)[0];
                tline.FillInfo.Color.G = ReadBytes(s, 1)[0];
                tline.FillInfo.Color.B = ReadBytes(s, 1)[0];
                tline.FillInfo.Color.A = ReadBytes(s, 1)[0];
                tline.FillInfo.BitmapIndex = ReadInt(s);
                tline.FillInfo.Matrix.ScaleX = ReadFloat(s);
                tline.FillInfo.Matrix.ScaleY = ReadFloat(s);
                tline.FillInfo.Matrix.RotateSkew0 = ReadFloat(s);
                tline.FillInfo.Matrix.RotateSkew1 = ReadFloat(s);
                tline.FillInfo.Matrix.TranslationX = ReadFloat(s);
                tline.FillInfo.Matrix.TranslationY = ReadFloat(s);
                tline.VertIndex = ReadInt(s);
                tline.VertCount = ReadInt(s);
                UIContainer.ShapeComponents.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiVertCount; i++)
            {
                UIComponent.fuiVert tline = new UIComponent.fuiVert();
                tline.X = ReadFloat(s);
                tline.Y = ReadFloat(s);
                UIContainer.Verts.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiTimelineFrameCount; i++)
            {
                UIComponent.fuiTimelineFrame tline = new UIComponent.fuiTimelineFrame();
                tline.FrameName = ReadCharArray(s, (int)0x40);
                tline.EventIndex = ReadInt(s);
                tline.EventCount = ReadInt(s);
                UIContainer.TimelineFrames.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiTimelineEventCount; i++)
            {
                UIComponent.fuiTimelineEvent tline = new UIComponent.fuiTimelineEvent();
                tline.EventType = ReadShort(s);
                tline.ObjectType = ReadShort(s);
                tline.Unknown0 = ReadShort(s);
                tline.Index = ReadShort(s);
                tline.Unknown1 = ReadShort(s);
                tline.NameIndex = ReadShort(s);
                tline.matrix.ScaleX = ReadFloat(s);
                tline.matrix.ScaleY = ReadFloat(s);
                tline.matrix.RotateSkew0 = ReadFloat(s);
                tline.matrix.RotateSkew1 = ReadFloat(s);
                tline.matrix.TranslationX = ReadFloat(s);
                tline.matrix.TranslationY = ReadFloat(s);
                tline.ColorTransform.RedMultTerm = ReadFloat(s);
                tline.ColorTransform.GreenMultTerm = ReadFloat(s);
                tline.ColorTransform.BlueMultTerm = ReadFloat(s);
                tline.ColorTransform.AlphaMultTerm = ReadFloat(s);
                tline.ColorTransform.RedAddTerm = ReadFloat(s);
                tline.ColorTransform.GreenAddTerm = ReadFloat(s);
                tline.ColorTransform.BlueAddTerm = ReadFloat(s);
                tline.ColorTransform.AlphaAddTerm = ReadFloat(s);
                tline.Color.R = ReadBytes(s, 1)[0];
                tline.Color.G = ReadBytes(s, 1)[0];
                tline.Color.B = ReadBytes(s, 1)[0];
                tline.Color.A = ReadBytes(s, 1)[0];
                UIContainer.TimelineEvents.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiTimelineEventNameCount; i++)
            {
                UIComponent.fuiTimelineEventName tline = new UIComponent.fuiTimelineEventName();
                tline.EventName = ReadCharArray(s, (int)0x40);
                UIContainer.TimelineEventNames.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiReferenceCount; i++)
            {
                UIComponent.fuiReference tline = new UIComponent.fuiReference();
                tline.SymbolIndex = ReadInt(s);
                tline.Name = ReadCharArray(s, (int)0x40);
                tline.Index = ReadInt(s);
                UIContainer.References.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiEdittextCount; i++)
            {
                UIComponent.fuiEdittext tline = new UIComponent.fuiEdittext();
                tline.Unknown0 = ReadInt(s);
                tline.Rectangle.MinX = ReadFloat(s);
                tline.Rectangle.MaxX = ReadFloat(s);
                tline.Rectangle.MinY = ReadFloat(s);
                tline.Rectangle.MaxY = ReadFloat(s);
                tline.FontID = ReadInt(s);
                tline.Unknown1 = ReadFloat(s);
                tline.Color.R = ReadBytes(s, 1)[0];
                tline.Color.G = ReadBytes(s, 1)[0];
                tline.Color.B = ReadBytes(s, 1)[0];
                tline.Color.A = ReadBytes(s, 1)[0];
                tline.Unknown2 = ReadInt(s);
                tline.Unknown3 = ReadInt(s);
                tline.Unknown4 = ReadInt(s);
                tline.Unknown5 = ReadInt(s);
                tline.Unknown6 = ReadInt(s);
                tline.Unknown7 = ReadInt(s);
                tline.htmltextformat = ReadCharArray(s, (int)(0x100));
                UIContainer.Edittexts.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiFontNameCount; i++)
            {
                UIComponent.fuiFontName tline = new UIComponent.fuiFontName();
                tline.ID = ReadInt(s);
                tline.FontName = ReadCharArray(s, (int)0x40);
                tline.Unknown0 = ReadInt(s);
                tline.Unknown1 = ReadCharArray(s, (int)0x40);
                tline.Unknown2 = ReadInt(s);
                tline.Unknown3 = ReadInt(s);
                tline.Unknown4 = ReadCharArray(s, (int)0x40);
                tline.Unknown5 = ReadInt(s);
                tline.Unknown6 = ReadInt(s);
                tline.Unknown7 = ReadCharArray(s, (int)0x2c);
                UIContainer.FontNames.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiSymbolCount; i++)
            {
                UIComponent.fuiSymbol tline = new UIComponent.fuiSymbol();
                tline.SymbolName = ReadCharArray(s, (int)0x40);
                tline.ObjectType = ReadInt(s);
                tline.Index = ReadInt(s);
                UIContainer.Symbols.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiImportAssetCount; i++)
            {
                UIComponent.fuiImportAsset tline = new UIComponent.fuiImportAsset();
                tline.Name = ReadCharArray(s, (int)0x40);
                UIContainer.ImportAssets.Add(tline);
            }
            for (int i = 0; i < UIContainer.Header.fuiBitmapCount; i++)
            {
                UIComponent.fuiBitmap tline = new UIComponent.fuiBitmap();
                tline.Unknown0 = ReadInt(s);
                tline.ImageFormat = ReadInt(s);
                tline.Width = ReadInt(s);
                tline.Height = ReadInt(s);
                tline.Offset = ReadInt(s);
                tline.Size = ReadInt(s);
                tline.ZlibDataOffset = ReadInt(s);
                tline.Unknown1 = ReadInt(s);
                UIContainer.Bitmaps.Add(tline);
            }
            foreach (UIComponent.fuiBitmap bitmap in UIContainer.Bitmaps)
            {
                UIComponent.fuiImage img = new UIComponent.fuiImage();
                img.data = ReadBytes(s, bitmap.Size);
                if (bitmap.ImageFormat == 1) // PNG File
                {
                    img.Format = 1;
                }
                else
                {
                    img.Format = 0;
                }
                UIContainer.Images.Add(img);
            }
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

            Console.WriteLine("Completed in: " + duration);
            return UIContainer;
        }
        private char[] ReadCharArray(Stream stream, int length)
        {
            return ReadString(stream, length, Encoding.UTF8).ToCharArray();
        }
    }
}
