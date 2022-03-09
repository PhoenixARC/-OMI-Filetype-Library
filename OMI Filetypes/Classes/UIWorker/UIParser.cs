using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UIWorker.model;

namespace UIWorker
{
    public class UIParser
    {
        private ArraySupport ArrSupport;

        public UIParser()
        {
            ArrSupport = new ArraySupport(false);
        }

        public UIContainer Parse(byte[] fuiData, string Filepath)
        {
            UIContainer UserInterfaceContainer = new UIContainer();
            MemoryStream s = new MemoryStream(fuiData);
            return Parse(UserInterfaceContainer, s, Filepath);
        }

        public UIContainer Parse(string Filepath)
        {
            UIContainer UserInterfaceContainer = new UIContainer();
            if (File.Exists(Filepath))
            {
                using (BinaryReader binaryReader = new BinaryReader(File.Open(Filepath, FileMode.Open)))
                {
                    Stream baseStream = binaryReader.BaseStream;
                    UserInterfaceContainer = Parse(UserInterfaceContainer, baseStream, Filepath);
                }
            }
            return UserInterfaceContainer;
        }

        public UIContainer Parse(UIContainer Uc, Stream s, string Filepath)
        {
            DateTime Begin = DateTime.Now;
            Uc.Header.Signature = ArrSupport.GetCharArray(s, (int)0x08);
            Uc.Header.ContentSize = ArrSupport.GetInt32(s);
            Uc.Header.SwfFileName = ArrSupport.GetCharArray(s, (int)0x40);
            Uc.Header.fuiTimelineCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiTimelineEventNameCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiTimelineActionCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiShapeCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiShapeComponentCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiVertCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiTimelineFrameCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiTimelineEventCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiReferenceCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiEdittextCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiSymbolCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiBitmapCount = ArrSupport.GetInt32(s);
            Uc.Header.imagesSize = ArrSupport.GetInt32(s);
            Uc.Header.fuiFontNameCount = ArrSupport.GetInt32(s);
            Uc.Header.fuiImportAssetCount = ArrSupport.GetInt32(s);
            Uc.Header.frameSize.MinX = ArrSupport.Getfloat(s);
            Uc.Header.frameSize.MaxX = ArrSupport.Getfloat(s);
            Uc.Header.frameSize.MinY = ArrSupport.Getfloat(s);
            Uc.Header.frameSize.MaxY = ArrSupport.Getfloat(s);
            for(int i = 0; i < Uc.Header.fuiTimelineCount; i++)
            {
                UIComponent.fuiTimeline tline = new UIComponent.fuiTimeline();
                tline.SymbolIndex = ArrSupport.GetInt32(s);
                tline.FrameIndex = ArrSupport.GetInt16(s);
                tline.FrameCount = ArrSupport.GetInt16(s);
                tline.ActionIndex = ArrSupport.GetInt16(s);
                tline.ActionCount = ArrSupport.GetInt16(s);
                tline.Rectangle.MinX = ArrSupport.GetInt32(s);
                tline.Rectangle.MaxX = ArrSupport.GetInt32(s);
                tline.Rectangle.MinY = ArrSupport.GetInt32(s);
                tline.Rectangle.MaxY = ArrSupport.GetInt32(s);
                Uc.Timelines.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiTimelineActionCount; i++)
            {
                UIComponent.fuiTimelineAction tline = new UIComponent.fuiTimelineAction();
                tline.ActionType = ArrSupport.GetInt16(s);
                tline.Unknown = ArrSupport.GetInt16(s);
                tline.StringArg0 = ArrSupport.GetCharArray(s, (int)0x40);
                tline.StringArg1 = ArrSupport.GetCharArray(s, (int)0x40);
                Uc.TimelineActions.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiShapeCount; i++)
            {
                UIComponent.fuiShape tline = new UIComponent.fuiShape();
                tline.Unknown = ArrSupport.GetInt32(s);
                tline.ShapeComponentIndex = ArrSupport.GetInt32(s);
                tline.ShapeComponentCount = ArrSupport.GetInt32(s);
                tline.Rectangle.MinX = ArrSupport.GetInt32(s);
                tline.Rectangle.MaxX = ArrSupport.GetInt32(s);
                tline.Rectangle.MinY = ArrSupport.GetInt32(s);
                tline.Rectangle.MaxY = ArrSupport.GetInt32(s);
                Uc.Shapes.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiShapeComponentCount; i++)
            {
                UIComponent.fuiShapeComponent tline = new UIComponent.fuiShapeComponent();
                tline.FillInfo.Type = ArrSupport.GetInt32(s);
                tline.FillInfo.Color.R = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.FillInfo.Color.G = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.FillInfo.Color.B = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.FillInfo.Color.A = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.FillInfo.BitmapIndex = ArrSupport.GetInt32(s);
                tline.FillInfo.Matrix.ScaleX = ArrSupport.Getfloat(s);
                tline.FillInfo.Matrix.ScaleY = ArrSupport.Getfloat(s);
                tline.FillInfo.Matrix.RotateSkew0 = ArrSupport.Getfloat(s);
                tline.FillInfo.Matrix.RotateSkew1 = ArrSupport.Getfloat(s);
                tline.FillInfo.Matrix.TranslationX = ArrSupport.Getfloat(s);
                tline.FillInfo.Matrix.TranslationY = ArrSupport.Getfloat(s);
                tline.VertIndex = ArrSupport.GetInt32(s);
                tline.VertCount = ArrSupport.GetInt32(s);
                Uc.ShapeComponents.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiVertCount; i++)
            {
                UIComponent.fuiVert tline = new UIComponent.fuiVert();
                tline.X = ArrSupport.Getfloat(s);
                tline.Y = ArrSupport.Getfloat(s);
                Uc.Verts.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiTimelineFrameCount; i++)
            {
                UIComponent.fuiTimelineFrame tline = new UIComponent.fuiTimelineFrame();
                tline.FrameName = ArrSupport.GetCharArray(s, (int)0x40);
                tline.EventIndex = ArrSupport.GetInt32(s);
                tline.EventCount = ArrSupport.GetInt32(s);
                Uc.TimelineFrames.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiTimelineEventCount; i++)
            {
                UIComponent.fuiTimelineEvent tline = new UIComponent.fuiTimelineEvent();
                tline.EventType = ArrSupport.GetInt16(s);
                tline.ObjectType = ArrSupport.GetInt16(s);
                tline.Unknown0 = ArrSupport.GetInt16(s);
                tline.Index = ArrSupport.GetInt16(s);
                tline.Unknown1 = ArrSupport.GetInt16(s);
                tline.NameIndex = ArrSupport.GetInt16(s);
                tline.matrix.ScaleX = ArrSupport.Getfloat(s);
                tline.matrix.ScaleY = ArrSupport.Getfloat(s);
                tline.matrix.RotateSkew0 = ArrSupport.Getfloat(s);
                tline.matrix.RotateSkew1 = ArrSupport.Getfloat(s);
                tline.matrix.TranslationX = ArrSupport.Getfloat(s);
                tline.matrix.TranslationY = ArrSupport.Getfloat(s);
                tline.ColorTransform.RedMultTerm = ArrSupport.Getfloat(s);
                tline.ColorTransform.GreenMultTerm = ArrSupport.Getfloat(s);
                tline.ColorTransform.BlueMultTerm = ArrSupport.Getfloat(s);
                tline.ColorTransform.AlphaMultTerm = ArrSupport.Getfloat(s);
                tline.ColorTransform.RedAddTerm = ArrSupport.Getfloat(s);
                tline.ColorTransform.GreenAddTerm = ArrSupport.Getfloat(s);
                tline.ColorTransform.BlueAddTerm = ArrSupport.Getfloat(s);
                tline.ColorTransform.AlphaAddTerm = ArrSupport.Getfloat(s);
                tline.Color.R = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.Color.G = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.Color.B = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.Color.A = ArrSupport.ReadStreamBytes(s, 1)[0];
                Uc.TimelineEvents.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiTimelineEventNameCount; i++)
            {
                UIComponent.fuiTimelineEventName tline = new UIComponent.fuiTimelineEventName();
                tline.EventName = ArrSupport.GetCharArray(s, (int)0x40);
                Uc.TimelineEventNames.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiReferenceCount; i++)
            {
                UIComponent.fuiReference tline = new UIComponent.fuiReference();
                tline.SymbolIndex = ArrSupport.GetInt32(s);
                tline.Name = ArrSupport.GetCharArray(s, (int)0x40);
                tline.Index = ArrSupport.GetInt32(s);
                Uc.References.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiEdittextCount; i++)
            {
                UIComponent.fuiEdittext tline = new UIComponent.fuiEdittext();
                tline.Unknown0 = ArrSupport.GetInt32(s);
                tline.Rectangle.MinX = ArrSupport.Getfloat(s);
                tline.Rectangle.MaxX = ArrSupport.Getfloat(s);
                tline.Rectangle.MinY = ArrSupport.Getfloat(s);
                tline.Rectangle.MaxY = ArrSupport.Getfloat(s);
                tline.FontID = ArrSupport.GetInt32(s);
                tline.Unknown1 = ArrSupport.Getfloat(s);
                tline.Color.R = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.Color.G = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.Color.B = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.Color.A = ArrSupport.ReadStreamBytes(s, 1)[0];
                tline.Unknown2 = ArrSupport.GetInt32(s);
                tline.Unknown3 = ArrSupport.GetInt32(s);
                tline.Unknown4 = ArrSupport.GetInt32(s);
                tline.Unknown5 = ArrSupport.GetInt32(s);
                tline.Unknown6 = ArrSupport.GetInt32(s);
                tline.Unknown7 = ArrSupport.GetInt32(s);
                tline.htmltextformat = ArrSupport.GetCharArray(s, (int)(0x100));
                Uc.Edittexts.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiFontNameCount; i++)
            {
                UIComponent.fuiFontName tline = new UIComponent.fuiFontName();
                tline.ID = ArrSupport.GetInt32(s);
                tline.FontName = ArrSupport.GetCharArray(s, (int)0x40);
                tline.Unknown0 = ArrSupport.GetInt32(s);
                tline.Unknown1 = ArrSupport.GetCharArray(s, (int)0x40);
                tline.Unknown2 = ArrSupport.GetInt32(s);
                tline.Unknown3 = ArrSupport.GetInt32(s);
                tline.Unknown4 = ArrSupport.GetCharArray(s, (int)0x40);
                tline.Unknown5 = ArrSupport.GetInt32(s);
                tline.Unknown6 = ArrSupport.GetInt32(s);
                tline.Unknown7 = ArrSupport.GetCharArray(s, (int)0x2c);
                Uc.FontNames.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiSymbolCount; i++)
            {
                UIComponent.fuiSymbol tline = new UIComponent.fuiSymbol();
                tline.SymbolName = ArrSupport.GetCharArray(s, (int)0x40);
                tline.ObjectType = ArrSupport.GetInt32(s);
                tline.Index = ArrSupport.GetInt32(s);
                Uc.Symbols.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiImportAssetCount; i++)
            {
                UIComponent.fuiImportAsset tline = new UIComponent.fuiImportAsset();
                tline.Name = ArrSupport.GetCharArray(s, (int)0x40);
                Uc.ImportAssets.Add(tline);
            }
            for(int i = 0; i < Uc.Header.fuiBitmapCount; i++)
            {
                UIComponent.fuiBitmap tline = new UIComponent.fuiBitmap();
                tline.Unknown0 = ArrSupport.GetInt32(s);
                tline.ImageFormat = ArrSupport.GetInt32(s);
                tline.Width = ArrSupport.GetInt32(s);
                tline.Height = ArrSupport.GetInt32(s);
                tline.Offset = ArrSupport.GetInt32(s);
                tline.Size = ArrSupport.GetInt32(s);
                tline.ZlibDataOffset = ArrSupport.GetInt32(s);
                tline.Unknown1 = ArrSupport.GetInt32(s);
                Uc.Bitmaps.Add(tline);
            }
            foreach (UIComponent.fuiBitmap bitmap in Uc.Bitmaps)
            {
                UIComponent.fuiImage img = new UIComponent.fuiImage();
                img.data = ArrSupport.ReadStreamBytes(s, bitmap.Size);
                if (bitmap.ImageFormat == 1) // PNG File
                {
                    img.Format = 1;
                }
                else
                {
                    img.Format = 0;
                }
                Uc.Images.Add(img);
            }
            TimeSpan duration = new TimeSpan(DateTime.Now.Ticks - Begin.Ticks);

            Console.WriteLine("Completed in: " + duration);
            return Uc;
        }

    }
}
