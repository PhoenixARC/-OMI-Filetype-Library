﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OMI.Formats.FUI;
using OMI.Formats.FUI.Component;
using OMI.utils;
/*
 * all known FourJUserInterface information is the direct product of Miku-666(NessieHax)'s work! check em out! 
 * https://github.com/NessieHax
*/
namespace OMI.Workers.FUI
{
    internal class FourjUIWriter : StreamDataWriter
    {
        FourjUserInterface _UIContainer;
        public FourjUIWriter(FourjUserInterface container, bool useLittleEndian) : base(useLittleEndian)
        {
            _UIContainer = container;
        }

        public void Write(Stream s)
        {
            WriteLong(s, _UIContainer.Header.Signature);
            WriteInt(s, _UIContainer.Header.ContentSize);
            WriteString(s, _UIContainer.Header.SwfFileName, 0x40, Encoding.ASCII);
            WriteInt(s, _UIContainer.Timelines.Count);
            WriteInt(s, _UIContainer.TimelineEventNames.Count);
            WriteInt(s, _UIContainer.TimelineActions.Count);
            WriteInt(s, _UIContainer.Shapes.Count);
            WriteInt(s, _UIContainer.ShapeComponents.Count);
            WriteInt(s, _UIContainer.Verts.Count);
            WriteInt(s, _UIContainer.TimelineFrames.Count);
            WriteInt(s, _UIContainer.TimelineEvents.Count);
            WriteInt(s, _UIContainer.References.Count);
            WriteInt(s, _UIContainer.Edittexts.Count);
            WriteInt(s, _UIContainer.Symbols.Count);
            WriteInt(s, _UIContainer.Bitmaps.Count);
            WriteInt(s, _UIContainer.Header.imagesSize);
            WriteInt(s, _UIContainer.FontNames.Count);
            WriteInt(s, _UIContainer.ImportAssets.Count);
            WriteFloat(s, _UIContainer.Header.frameSize.MinX);
            WriteFloat(s, _UIContainer.Header.frameSize.MaxX);
            WriteFloat(s, _UIContainer.Header.frameSize.MinY);
            WriteFloat(s, _UIContainer.Header.frameSize.MaxY);

            foreach (fuiTimeline tl in _UIContainer.Timelines)
            {
                WriteInt(s, tl.SymbolIndex);
                WriteShort(s, tl.FrameIndex);
                WriteShort(s, tl.FrameCount);
                WriteShort(s, tl.ActionIndex);
                WriteShort(s, tl.ActionCount);
                WriteFloat(s, tl.Rectangle.MinX);
                WriteFloat(s, tl.Rectangle.MaxX);
                WriteFloat(s, tl.Rectangle.MinY);
                WriteFloat(s, tl.Rectangle.MaxY);
            }
            foreach (fuiTimelineAction tl in _UIContainer.TimelineActions)
            {
                WriteShort(s, tl.ActionType);
                WriteShort(s, tl.Unknown);
                WriteString(s, tl.StringArg0, 0x40, Encoding.ASCII);
                WriteString(s, tl.StringArg1, 0x40, Encoding.ASCII);
            }
            foreach (fuiShape tl in _UIContainer.Shapes)
            {
                WriteInt(s, tl.Unknown);
                WriteInt(s, tl.ShapeComponentIndex);
                WriteInt(s, tl.ShapeComponentCount);
                WriteFloat(s, tl.Rectangle.MinX);
                WriteFloat(s, tl.Rectangle.MaxX);
                WriteFloat(s, tl.Rectangle.MinY);
                WriteFloat(s, tl.Rectangle.MaxY);
            }
            foreach (fuiShapeComponent tl in _UIContainer.ShapeComponents)
            {
                WriteInt(s, tl.FillInfo.Type);
                WriteByte(s, tl.FillInfo.Color.R);
                WriteByte(s, tl.FillInfo.Color.G);
                WriteByte(s, tl.FillInfo.Color.B);
                WriteByte(s, tl.FillInfo.Color.A);
                WriteInt(s, tl.FillInfo.BitmapIndex);
                WriteFloat(s, tl.FillInfo.Matrix.ScaleX);
                WriteFloat(s, tl.FillInfo.Matrix.ScaleY);
                WriteFloat(s, tl.FillInfo.Matrix.RotateSkew0);
                WriteFloat(s, tl.FillInfo.Matrix.RotateSkew1);
                WriteFloat(s, tl.FillInfo.Matrix.TranslationX);
                WriteFloat(s, tl.FillInfo.Matrix.TranslationY);
                WriteInt(s, tl.VertIndex);
                WriteInt(s, tl.VertCount);
            }
            foreach (fuiVert tl in _UIContainer.Verts)
            {
                WriteFloat(s, tl.X);
                WriteFloat(s, tl.Y);
            }
            foreach (fuiTimelineFrame tl in _UIContainer.TimelineFrames)
            {
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.FrameName), 0x40);
                WriteInt(s, tl.EventIndex);
                WriteInt(s, tl.EventCount);
            }
            foreach (fuiTimelineEvent tl in _UIContainer.TimelineEvents)
            {
                WriteShort(s, tl.EventType);
                WriteShort(s, tl.ObjectType);
                WriteShort(s, tl.Unknown0);
                WriteShort(s, tl.Index);
                WriteShort(s, tl.Unknown1);
                WriteShort(s, tl.NameIndex);
                WriteFloat(s, tl.matrix.ScaleX);
                WriteFloat(s, tl.matrix.ScaleY);
                WriteFloat(s, tl.matrix.RotateSkew0);
                WriteFloat(s, tl.matrix.RotateSkew1);
                WriteFloat(s, tl.matrix.TranslationX);
                WriteFloat(s, tl.matrix.TranslationY);
                WriteFloat(s, tl.ColorTransform.RedMultTerm);
                WriteFloat(s, tl.ColorTransform.GreenMultTerm);
                WriteFloat(s, tl.ColorTransform.BlueMultTerm);
                WriteFloat(s, tl.ColorTransform.AlphaMultTerm);
                WriteFloat(s, tl.ColorTransform.RedAddTerm);
                WriteFloat(s, tl.ColorTransform.GreenAddTerm);
                WriteFloat(s, tl.ColorTransform.BlueAddTerm);
                WriteFloat(s, tl.ColorTransform.AlphaAddTerm);
                //OutputBytes.AddRange(new byte[] { 0xff, 0xff, 0xff, 0xff});
                WriteByte(s, tl.Color.R);
                WriteByte(s, tl.Color.G);
                WriteByte(s, tl.Color.B);
                WriteByte(s, tl.Color.A);
            }
            foreach (fuiTimelineEventName tl in _UIContainer.TimelineEventNames)
            {
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.EventName), 0x40);
            }
            foreach (fuiReference tl in _UIContainer.References)
            {
                WriteInt(s, tl.SymbolIndex);
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.Name), 0x40);
                WriteInt(s, tl.Index);
            }
            foreach (fuiEdittext tl in _UIContainer.Edittexts)
            {
                WriteInt(s, tl.Unknown0);
                WriteFloat(s, tl.Rectangle.MinX);
                WriteFloat(s, tl.Rectangle.MaxX);
                WriteFloat(s, tl.Rectangle.MinY);
                WriteFloat(s, tl.Rectangle.MaxY);
                WriteInt(s, tl.FontID);
                WriteFloat(s, tl.Unknown1);
                WriteByte(s, tl.Color.R);
                WriteByte(s, tl.Color.G);
                WriteByte(s, tl.Color.B);
                WriteByte(s, tl.Color.A);
                WriteInt(s, tl.Unknown2);
                WriteInt(s, tl.Unknown3);
                WriteInt(s, tl.Unknown4);
                WriteInt(s, tl.Unknown5);
                WriteInt(s, tl.Unknown6);
                WriteInt(s, tl.Unknown7);
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.htmltextformat), 0x100);
            }
            foreach (fuiFontName tl in _UIContainer.FontNames)
            {
                WriteInt(s, tl.ID);
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.FontName));
                WriteInt(s, tl.Unknown0);
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.Unknown1));
                WriteInt(s, tl.Unknown2);
                WriteInt(s, tl.Unknown3);
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.Unknown4));
                WriteInt(s, tl.Unknown5);
                WriteInt(s, tl.Unknown6);
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.Unknown7));
            }
            foreach (fuiSymbol tl in _UIContainer.Symbols)
            {
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.SymbolName), 0x40);
                WriteInt(s, tl.ObjectType);
                WriteInt(s, tl.Index);
            }
            foreach (fuiImportAsset tl in _UIContainer.ImportAssets)
            {
                WriteBytes(s, Encoding.ASCII.GetBytes(tl.Name));
            }
            foreach (fuiBitmap tl in _UIContainer.Bitmaps)
            {
                WriteInt(s, tl.Unknown0);
                WriteInt(s, tl.ImageFormat);
                WriteInt(s, tl.Width);
                WriteInt(s, tl.Height);
                WriteInt(s, tl.Offset);
                WriteInt(s, tl.Size);
                WriteInt(s, tl.ZlibDataOffset);
                WriteInt(s, tl.Unknown1);
            }
            foreach (fuiImage img in _UIContainer.Images)
            {
                WriteBytes(s, img.data);
            }
        }

        private void WriteByte(Stream stream, byte b)
        {
            stream.WriteByte(b);
        }
    }
}