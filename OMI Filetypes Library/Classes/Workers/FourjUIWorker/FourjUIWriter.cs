/* Copyright (c) 2022-present miku-666
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
**/
using System.IO;
using System.Text;
using OMI.Formats.FUI;
using OMI.Formats.FUI.Components;
using OMI.Workers;
/*
* all known FourJUserInterface information is the direct product of Miku-666(NessieHax)'s work! check em out! 
* https://github.com/NessieHax
*/
namespace OMI.Workers.FUI
{
    public class FourjUIWriter : IDataFormatWriter
    {
        private FourjUserInterface _UIContainer;
        public FourjUIWriter(FourjUserInterface container)
        {
            _UIContainer = container;
        }

        public void WriteToFile(string fileName)
        {
            using (var fs = File.OpenWrite(fileName))
            {
                WriteToStream(fs);
            }
        }

        public void WriteToStream(Stream stream)
        {
            using (var writer = new EndiannessAwareBinaryWriter(stream, Encoding.ASCII, leaveOpen: true, Endianness.LittleEndian))
            {
                writer.Write(_UIContainer.Header.Signature);

                writer.Write(_UIContainer.Header.ContentSize);
                writer.WriteString(_UIContainer.Header.SwfFileName, 0x40);
                writer.Write(_UIContainer.Timelines.Count);
                writer.Write(_UIContainer.TimelineEventNames.Count);
                writer.Write(_UIContainer.TimelineActions.Count);
                writer.Write(_UIContainer.Shapes.Count);
                writer.Write(_UIContainer.ShapeComponents.Count);
                writer.Write(_UIContainer.Verts.Count);
                writer.Write(_UIContainer.TimelineFrames.Count);
                writer.Write(_UIContainer.TimelineEvents.Count);
                writer.Write(_UIContainer.References.Count);
                writer.Write(_UIContainer.Edittexts.Count);
                writer.Write(_UIContainer.Symbols.Count);
                writer.Write(_UIContainer.Bitmaps.Count);
                writer.Write(_UIContainer.Header.imagesSize);
                writer.Write(_UIContainer.FontNames.Count);
                writer.Write(_UIContainer.ImportAssets.Count);
                writer.Write(_UIContainer.Header.frameSize.MinX);
                writer.Write(_UIContainer.Header.frameSize.MaxX);
                writer.Write(_UIContainer.Header.frameSize.MinY);
                writer.Write(_UIContainer.Header.frameSize.MaxY);

                foreach (FuiTimeline timeline in _UIContainer.Timelines)
                {
                    writer.Write(timeline.SymbolIndex);
                    writer.Write(timeline.FrameIndex);
                    writer.Write(timeline.FrameCount);
                    writer.Write(timeline.ActionIndex);
                    writer.Write(timeline.ActionCount);
                    writer.Write(timeline.Rectangle.MinX);
                    writer.Write(timeline.Rectangle.MaxX);
                    writer.Write(timeline.Rectangle.MinY);
                    writer.Write(timeline.Rectangle.MaxY);
                }
                foreach (FuiTimelineAction timelineAction in _UIContainer.TimelineActions)
                {
                    writer.Write(timelineAction.ActionType);
                    writer.Write(timelineAction.Unknown);
                    writer.Write(timelineAction.FrameIndex);
                    writer.WriteString(timelineAction.StringArg0, 0x40);
                    writer.WriteString(timelineAction.StringArg1, 0x40);
                }
                foreach (FuiShape shape in _UIContainer.Shapes)
                {
                    writer.Write(shape.Unknown);
                    writer.Write(shape.ShapeComponentIndex);
                    writer.Write(shape.ShapeComponentCount);
                    writer.Write(shape.Rectangle.MinX);
                    writer.Write(shape.Rectangle.MaxX);
                    writer.Write(shape.Rectangle.MinY);
                    writer.Write(shape.Rectangle.MaxY);
                }
                foreach (FuiShapeComponent shapeComponent in _UIContainer.ShapeComponents)
                {
                    writer.Write(shapeComponent.FillInfo.Type);
                    writer.Write(shapeComponent.FillInfo.Color.RGBA);
                    writer.Write(shapeComponent.FillInfo.BitmapIndex);
                    writer.Write(shapeComponent.FillInfo.Matrix.ScaleX);
                    writer.Write(shapeComponent.FillInfo.Matrix.ScaleY);
                    writer.Write(shapeComponent.FillInfo.Matrix.RotateSkew0);
                    writer.Write(shapeComponent.FillInfo.Matrix.RotateSkew1);
                    writer.Write(shapeComponent.FillInfo.Matrix.TranslationX);
                    writer.Write(shapeComponent.FillInfo.Matrix.TranslationY);
                    writer.Write(shapeComponent.VertIndex);
                    writer.Write(shapeComponent.VertCount);
                }
                foreach (FuiVert vert in _UIContainer.Verts)
                {
                    writer.Write(vert.X);
                    writer.Write(vert.Y);
                }
                foreach (FuiTimelineFrame timelineFrame in _UIContainer.TimelineFrames)
                {
                    writer.WriteString(timelineFrame.FrameName, 0x40);
                    writer.Write(timelineFrame.EventIndex);
                    writer.Write(timelineFrame.EventCount);
                }
                foreach (FuiTimelineEvent timelineEvent in _UIContainer.TimelineEvents)
                {
                    writer.Write(timelineEvent.EventType);
                    writer.Write(timelineEvent.ObjectType);
                    writer.Write(timelineEvent.Unknown0);
                    writer.Write(timelineEvent.Index);
                    writer.Write(timelineEvent.Unknown1);
                    writer.Write(timelineEvent.NameIndex);
                    writer.Write(timelineEvent.matrix.ScaleX);
                    writer.Write(timelineEvent.matrix.ScaleY);
                    writer.Write(timelineEvent.matrix.RotateSkew0);
                    writer.Write(timelineEvent.matrix.RotateSkew1);
                    writer.Write(timelineEvent.matrix.TranslationX);
                    writer.Write(timelineEvent.matrix.TranslationY);
                    writer.Write(timelineEvent.ColorTransform.RedMultTerm);
                    writer.Write(timelineEvent.ColorTransform.GreenMultTerm);
                    writer.Write(timelineEvent.ColorTransform.BlueMultTerm);
                    writer.Write(timelineEvent.ColorTransform.AlphaMultTerm);
                    writer.Write(timelineEvent.ColorTransform.RedAddTerm);
                    writer.Write(timelineEvent.ColorTransform.GreenAddTerm);
                    writer.Write(timelineEvent.ColorTransform.BlueAddTerm);
                    writer.Write(timelineEvent.ColorTransform.AlphaAddTerm);
                    writer.Write(timelineEvent.Color.RGBA);
                }
                foreach (FuiTimelineEventName eventName in _UIContainer.TimelineEventNames)
                {
                    writer.WriteString(eventName.Name, 0x40);
                }
                foreach (FuiReference reference in _UIContainer.References)
                {
                    writer.Write(reference.SymbolIndex);
                    writer.WriteString(reference.Name, 0x40);
                    writer.Write(reference.Index);
                }
                foreach (FuiEdittext edittext in _UIContainer.Edittexts)
                {
                    writer.Write(edittext.Unknown0);
                    writer.Write(edittext.Rectangle.MinX);
                    writer.Write(edittext.Rectangle.MaxX);
                    writer.Write(edittext.Rectangle.MinY);
                    writer.Write(edittext.Rectangle.MaxY);
                    writer.Write(edittext.FontID);
                    writer.Write(edittext.Unknown1);
                    writer.Write(edittext.Color.RGBA);
                    writer.Write(edittext.Unknown2);
                    writer.Write(edittext.Unknown3);
                    writer.Write(edittext.Unknown4);
                    writer.Write(edittext.Unknown5);
                    writer.Write(edittext.Unknown6);
                    writer.Write(edittext.Unknown7);
                    writer.WriteString(edittext.htmlSource, 0x100);
                }
                foreach (FuiFontName fontName in _UIContainer.FontNames)
                {
                    writer.Write(fontName.ID);
                    writer.WriteString(fontName.Name, 0x40);
                    writer.Write(new byte[0xC0]);
                }
                foreach (FuiSymbol symbol in _UIContainer.Symbols)
                {
                    writer.WriteString(symbol.SymbolName, 0x40);
                    writer.Write(symbol.ObjectType);
                    writer.Write(symbol.Index);
                }
                foreach (FuiImportAsset importAsset in _UIContainer.ImportAssets)
                {
                    writer.WriteString(importAsset.Name, 0x40);
                }
                foreach (FuiBitmap bitmap in _UIContainer.Bitmaps)
                {
                    writer.Write(bitmap.Unknown0);
                    writer.Write(bitmap.ImageFormat);
                    writer.Write(bitmap.Width);
                    writer.Write(bitmap.Height);
                    writer.Write(bitmap.Offset);
                    writer.Write(bitmap.Size);
                    writer.Write(bitmap.ZlibDataOffset);
                    writer.Write(bitmap.Unknown1);
                }
                foreach (byte[] imgData in _UIContainer.ImagesData)
                {
                    writer.Write(imgData);
                }
            }
        }
    }
}
