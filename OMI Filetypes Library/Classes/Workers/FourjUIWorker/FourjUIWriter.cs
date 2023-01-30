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

        }

        public void WriteToStream(Stream s)
        {
            using (var writer = new EndiannessAwareBinaryWriter(s))
            {
                writer.Write(_UIContainer.Header.Signature);

                writer.Write(_UIContainer.Header.ContentSize);
                writer.WriteString(_UIContainer.Header.SwfFileName, 0x40, Encoding.ASCII);
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

                foreach (FuiTimeline tl in _UIContainer.Timelines)
                {
                    writer.Write(tl.SymbolIndex);
                    writer.Write(tl.FrameIndex);
                    writer.Write(tl.FrameCount);
                    writer.Write(tl.ActionIndex);
                    writer.Write(tl.ActionCount);
                    writer.Write(tl.Rectangle.MinX);
                    writer.Write(tl.Rectangle.MaxX);
                    writer.Write(tl.Rectangle.MinY);
                    writer.Write(tl.Rectangle.MaxY);
                }
                foreach (FuiTimelineAction tl in _UIContainer.TimelineActions)
                {
                    writer.Write(tl.ActionType);
                    writer.Write(tl.Unknown);
                    writer.WriteString(tl.StringArg0, 0x40, Encoding.ASCII);
                    writer.WriteString(tl.StringArg1, 0x40, Encoding.ASCII);
                }
                foreach (FuiShape tl in _UIContainer.Shapes)
                {
                    writer.Write(tl.Unknown);
                    writer.Write(tl.ShapeComponentIndex);
                    writer.Write(tl.ShapeComponentCount);
                    writer.Write(tl.Rectangle.MinX);
                    writer.Write(tl.Rectangle.MaxX);
                    writer.Write(tl.Rectangle.MinY);
                    writer.Write(tl.Rectangle.MaxY);
                }
                foreach (FuiShapeComponent tl in _UIContainer.ShapeComponents)
                {
                    writer.Write(tl.FillInfo.Type);
                    WriteByte(s, tl.FillInfo.Color.R);
                    WriteByte(s, tl.FillInfo.Color.G);
                    WriteByte(s, tl.FillInfo.Color.B);
                    WriteByte(s, tl.FillInfo.Color.A);
                    writer.Write(tl.FillInfo.BitmapIndex);
                    writer.Write(tl.FillInfo.Matrix.ScaleX);
                    writer.Write(tl.FillInfo.Matrix.ScaleY);
                    writer.Write(tl.FillInfo.Matrix.RotateSkew0);
                    writer.Write(tl.FillInfo.Matrix.RotateSkew1);
                    writer.Write(tl.FillInfo.Matrix.TranslationX);
                    writer.Write(tl.FillInfo.Matrix.TranslationY);
                    writer.Write(tl.VertIndex);
                    writer.Write(tl.VertCount);
                }
                foreach (FuiVert tl in _UIContainer.Verts)
                {
                    writer.Write(tl.X);
                    writer.Write(tl.Y);
                }
                foreach (FuiTimelineFrame tl in _UIContainer.TimelineFrames)
                {
                    writer.WriteString(tl.FrameName, 0x40);
                    writer.Write(tl.EventIndex);
                    writer.Write(tl.EventCount);
                }
                foreach (FuiTimelineEvent tl in _UIContainer.TimelineEvents)
                {
                    writer.Write(tl.EventType);
                    writer.Write(tl.ObjectType);
                    writer.Write(tl.Unknown0);
                    writer.Write(tl.Index);
                    writer.Write(tl.Unknown1);
                    writer.Write(tl.NameIndex);
                    writer.Write(tl.matrix.ScaleX);
                    writer.Write(tl.matrix.ScaleY);
                    writer.Write(tl.matrix.RotateSkew0);
                    writer.Write(tl.matrix.RotateSkew1);
                    writer.Write(tl.matrix.TranslationX);
                    writer.Write(tl.matrix.TranslationY);
                    writer.Write(tl.ColorTransform.RedMultTerm);
                    writer.Write(tl.ColorTransform.GreenMultTerm);
                    writer.Write(tl.ColorTransform.BlueMultTerm);
                    writer.Write(tl.ColorTransform.AlphaMultTerm);
                    writer.Write(tl.ColorTransform.RedAddTerm);
                    writer.Write(tl.ColorTransform.GreenAddTerm);
                    writer.Write(tl.ColorTransform.BlueAddTerm);
                    writer.Write(tl.ColorTransform.AlphaAddTerm);
                    //OutputBytes.AddRange(new byte[] { 0xff, 0xff, 0xff, 0xff});
                    WriteByte(s, tl.Color.R);
                    WriteByte(s, tl.Color.G);
                    WriteByte(s, tl.Color.B);
                    WriteByte(s, tl.Color.A);
                }
                foreach (FuiTimelineEventName tl in _UIContainer.TimelineEventNames)
                {
                    writer.WriteString(tl.EventName, 0x40);
                }
                foreach (FuiReference tl in _UIContainer.References)
                {
                    writer.Write(tl.SymbolIndex);
                    writer.WriteString(tl.Name, 0x40);
                    writer.Write(tl.Index);
                }
                foreach (FuiEdittext tl in _UIContainer.Edittexts)
                {
                    writer.Write(tl.Unknown0);
                    writer.Write(tl.Rectangle.MinX);
                    writer.Write(tl.Rectangle.MaxX);
                    writer.Write(tl.Rectangle.MinY);
                    writer.Write(tl.Rectangle.MaxY);
                    writer.Write(tl.FontID);
                    writer.Write(tl.Unknown1);
                    writer.Write(tl.Color.RGBA);
                    writer.Write(tl.Unknown2);
                    writer.Write(tl.Unknown3);
                    writer.Write(tl.Unknown4);
                    writer.Write(tl.Unknown5);
                    writer.Write(tl.Unknown6);
                    writer.Write(tl.Unknown7);
                    writer.WriteString(tl.htmltextformat, 0x100);
                }
                foreach (FuiFontName tl in _UIContainer.FontNames)
                {
                    writer.Write(tl.ID);
                    writer.WriteString(tl.FontName, 0x40);
                    writer.Write(new byte[0xC0]);
                }
                foreach (FuiSymbol tl in _UIContainer.Symbols)
                {
                    writer.WriteString(tl.SymbolName, 0x40, Encoding.ASCII);
                    writer.Write(tl.ObjectType);
                    writer.Write(tl.Index);
                }
                foreach (FuiImportAsset importAsset in _UIContainer.ImportAssets)
                {
                    writer.WriteString(importAsset.Name, 0x40, Encoding.ASCII);
                }
                foreach (FuiBitmap tl in _UIContainer.Bitmaps)
                {
                    writer.Write(tl.Unknown0);
                    writer.Write(tl.ImageFormat);
                    writer.Write(tl.Width);
                    writer.Write(tl.Height);
                    writer.Write(tl.Offset);
                    writer.Write(tl.Size);
                    writer.Write(tl.ZlibDataOffset);
                    writer.Write(tl.Unknown1);
                }
                foreach (fuiImage img in _UIContainer.Images)
                {
                    writer.Write(img.data);
                }
            }
        }

        private void WriteByte(Stream stream, byte b)
        {
            stream.WriteByte(b);
        }
    }
}
