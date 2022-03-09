using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIWorker.model;

namespace UIWorker
{
    public class UIBuilder
    {
        public byte[] Build(UIContainer uc)
        {
            List<byte> OutputBytes = new List<byte>();
            OutputBytes.AddRange(Encoding.UTF8.GetBytes(uc.Header.Signature));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Header.ContentSize));
            OutputBytes.AddRange(Encoding.UTF8.GetBytes(uc.Header.SwfFileName));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Timelines.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.TimelineEventNames.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.TimelineActions.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Shapes.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.ShapeComponents.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Verts.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.TimelineFrames.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.TimelineEvents.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.References.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Edittexts.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Symbols.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Bitmaps.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Header.imagesSize));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.FontNames.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.ImportAssets.Count));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Header.frameSize.MinX));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Header.frameSize.MaxX));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Header.frameSize.MinY));
            OutputBytes.AddRange(BitConverter.GetBytes(uc.Header.frameSize.MaxY));
            foreach(UIComponent.fuiTimeline tl in uc.Timelines)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.SymbolIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FrameIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FrameCount));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ActionIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ActionCount));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MinX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MaxX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MinY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MaxY));
            }
            foreach(UIComponent.fuiTimelineAction tl in uc.TimelineActions)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ActionType));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.StringArg0));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.StringArg1));
            }
            foreach(UIComponent.fuiShape tl in uc.Shapes)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ShapeComponentIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ShapeComponentCount));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MinX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MaxX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MinY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MaxY));
            }
            foreach(UIComponent.fuiShapeComponent tl in uc.ShapeComponents)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.Type));
                OutputBytes.Add((tl.FillInfo.Color.R));
                OutputBytes.Add((tl.FillInfo.Color.G));
                OutputBytes.Add((tl.FillInfo.Color.B));
                OutputBytes.Add((tl.FillInfo.Color.A));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.BitmapIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.Matrix.ScaleX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.Matrix.ScaleY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.Matrix.RotateSkew0));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.Matrix.RotateSkew1));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.Matrix.TranslationX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FillInfo.Matrix.TranslationY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.VertIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.VertCount));
            }
            foreach(UIComponent.fuiVert tl in uc.Verts)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.X));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Y));
            }
            foreach(UIComponent.fuiTimelineFrame tl in uc.TimelineFrames)
            {
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.FrameName));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.EventIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.EventCount));
            }
            foreach(UIComponent.fuiTimelineEvent tl in uc.TimelineEvents)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.EventType));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ObjectType));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown0));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Index));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown1));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.NameIndex));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.matrix.ScaleX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.matrix.ScaleY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.matrix.RotateSkew0));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.matrix.RotateSkew1));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.matrix.TranslationX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.matrix.TranslationY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.RedMultTerm));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.GreenMultTerm));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.BlueMultTerm));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.AlphaMultTerm));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.RedAddTerm));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.GreenAddTerm));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.BlueAddTerm));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ColorTransform.AlphaAddTerm));
                //OutputBytes.AddRange(new byte[] { 0xff, 0xff, 0xff, 0xff});
                OutputBytes.Add((tl.Color.R));
                OutputBytes.Add((tl.Color.G));
                OutputBytes.Add((tl.Color.B));
                OutputBytes.Add((tl.Color.A));
            }
            foreach(UIComponent.fuiTimelineEventName tl in uc.TimelineEventNames)
            {
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.EventName));
            }
            foreach(UIComponent.fuiReference tl in uc.References)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.SymbolIndex));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.Name));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Index));
            }
            foreach(UIComponent.fuiEdittext tl in uc.Edittexts)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown0));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MinX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MaxX));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MinY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Rectangle.MaxY));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.FontID));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown1));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Color.R));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Color.G));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Color.B));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Color.A));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown2));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown3));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown4));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown5));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown6));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown7));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.htmltextformat));
            }
            foreach(UIComponent.fuiFontName tl in uc.FontNames)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ID));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.FontName));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown0));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.Unknown1));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown2));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown3));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.Unknown4));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown5));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown6));
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.Unknown7));
            }
            foreach(UIComponent.fuiSymbol tl in uc.Symbols)
            {
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.SymbolName));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ObjectType));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Index));
            }
            foreach(UIComponent.fuiImportAsset tl in uc.ImportAssets)
            {
                OutputBytes.AddRange(Encoding.UTF8.GetBytes(tl.Name));
            }
            foreach(UIComponent.fuiBitmap tl in uc.Bitmaps)
            {
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown0));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ImageFormat));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Width));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Height));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Offset));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Size));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.ZlibDataOffset));
                OutputBytes.AddRange(BitConverter.GetBytes(tl.Unknown1));
            }
            foreach(UIComponent.fuiImage img in uc.Images)
            {
                OutputBytes.AddRange(img.data);
            }
            /*
            */
            return OutputBytes.ToArray();
        }
    }
}
