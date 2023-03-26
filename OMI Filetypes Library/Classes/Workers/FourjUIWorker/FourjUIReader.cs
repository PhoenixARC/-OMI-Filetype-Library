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
using System.Diagnostics;
using OMI.Formats.FUI;
using OMI.Formats.FUI.Components;
using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
/*
* all known FourJUserInterface information is the direct product of Miku-666(NessieHax)'s work! check em out! 
* https://github.com/NessieHax
*/
namespace OMI.Workers.FUI
{
    public class FourjUIReader : IDataFormatReader<FourjUserInterface>, IDataFormatReader
    {
        public FourjUIReader()
        {
        }

        public FourjUserInterface FromFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename);
            FourjUserInterface UserInterfaceContainer;
            using (var fs = File.OpenRead(filename))
            {
                UserInterfaceContainer = FromStream(fs);
            }
            return UserInterfaceContainer;
        }

        public FourjUserInterface FromStream(Stream stream)
        {
            FourjUserInterface UIContainer = new FourjUserInterface();
            using (var reader = new EndiannessAwareBinaryReader(stream, Encoding.ASCII, Endianness.LittleEndian))
            {
                UIContainer.Header.Signature = reader.ReadInt64(Endianness.BigEndian);
                UIContainer.Header.ContentSize = reader.ReadInt32();
                UIContainer.Header.SwfFileName = reader.ReadString(0x40);

                UIContainer.Timelines = new List<FuiTimeline>(reader.ReadInt32());
                UIContainer.TimelineEventNames = new List<string>(reader.ReadInt32());
                UIContainer.TimelineActions = new List<FuiTimelineAction>(reader.ReadInt32());
                UIContainer.Shapes = new List<FuiShape>(reader.ReadInt32());
                UIContainer.ShapeComponents = new List<FuiShapeComponent>(reader.ReadInt32());
                UIContainer.Verts = new List<FuiVert>(reader.ReadInt32());
                UIContainer.TimelineFrames = new List<FuiTimelineFrame>(reader.ReadInt32());
                UIContainer.TimelineEvents = new List<FuiTimelineEvent>(reader.ReadInt32());
                UIContainer.References = new List<FuiReference>(reader.ReadInt32());
                UIContainer.Edittexts = new List<FuiEdittext>(reader.ReadInt32());
                UIContainer.Symbols = new List<FuiSymbol>(reader.ReadInt32());
                UIContainer.Bitmaps = new List<FuiBitmap>(reader.ReadInt32());

                int imagesSize = reader.ReadInt32();

                UIContainer.FontNames = new List<FuiFontName>(reader.ReadInt32());
                UIContainer.ImportAssets = new List<string>(reader.ReadInt32());

                UIContainer.Header.FrameSize.MinX = reader.ReadSingle();
                UIContainer.Header.FrameSize.MaxX = reader.ReadSingle();
                UIContainer.Header.FrameSize.MinY = reader.ReadSingle();
                UIContainer.Header.FrameSize.MaxY = reader.ReadSingle();

                reader.Fill(UIContainer.Timelines, ReadTimeline);
                reader.Fill(UIContainer.TimelineActions, ReadTimelineAction);
                reader.Fill(UIContainer.Shapes, ReadShape);
                reader.Fill(UIContainer.ShapeComponents, ReadShapeComponent);
                reader.Fill(UIContainer.Verts, ReadVert);
                reader.Fill(UIContainer.TimelineFrames, ReadTimelineFrame);
                reader.Fill(UIContainer.TimelineEvents, ReadTimelineEvent);
                reader.Fill(UIContainer.TimelineEventNames, ReadString);
                reader.Fill(UIContainer.References, ReadReference);
                reader.Fill(UIContainer.Edittexts, ReadEdittext);
                reader.Fill(UIContainer.FontNames, ReadFontName);
                reader.Fill(UIContainer.Symbols, ReadSymbol);
                reader.Fill(UIContainer.ImportAssets, ReadString);
                reader.Fill(UIContainer.Bitmaps, ReadBitmap);

                using (var ms = new MemoryStream(reader.ReadBytes(imagesSize)))
                {
                    foreach (FuiBitmap bitmap in UIContainer.Bitmaps)
                    {
                        long origin = ms.Position;
                        ms.Seek(bitmap.Offset, SeekOrigin.Begin);
                        byte[] buffer = new byte[bitmap.Size];
                        ms.Read(buffer, 0, bitmap.Size);
                        ms.Seek(origin, SeekOrigin.Begin);
                        UIContainer.ImagesData.Add(buffer);
                    }
                }
            }
            return UIContainer;
        }

        private FuiBitmap ReadBitmap(EndiannessAwareBinaryReader reader)
        {
            FuiBitmap tline = new FuiBitmap();
            tline.SymbolIndex = reader.ReadInt32();
            tline.ImageFormat = reader.ReadInt32();
            tline.Width = reader.ReadInt32();
            tline.Height = reader.ReadInt32();
            tline.Offset = reader.ReadInt32();
            tline.Size = reader.ReadInt32();
            tline.ZlibDataOffset = reader.ReadInt32();
            tline.Unknown1 = reader.ReadInt32();
            return tline;
        }

        private FuiSymbol ReadSymbol(EndiannessAwareBinaryReader reader)
        {
            FuiSymbol symbol = new FuiSymbol();
            symbol.Name = reader.ReadString(0x40);
            symbol.ObjectType = reader.ReadInt32();
            symbol.Index = reader.ReadInt32();
            return symbol;
        }

        private FuiFontName ReadFontName(EndiannessAwareBinaryReader reader)
        {
            FuiFontName fontName = new FuiFontName();
            fontName.ID = reader.ReadInt32();
            fontName.FontName = reader.ReadString(0x40);
            reader.ReadBytes(0xc0); // unknown values
            return fontName;
        }

        private FuiEdittext ReadEdittext(EndiannessAwareBinaryReader reader)
        {
            FuiEdittext edittext = new FuiEdittext();
            edittext.Unknown0 = reader.ReadInt32();
            edittext.Rectangle.MinX = reader.ReadSingle();
            edittext.Rectangle.MaxX = reader.ReadSingle();
            edittext.Rectangle.MinY = reader.ReadSingle();
            edittext.Rectangle.MaxY = reader.ReadSingle();
            edittext.FontID = reader.ReadInt32();
            edittext.Unknown1 = reader.ReadSingle();
            edittext.Color = FuiRGBA.GetColor(reader.ReadInt32());
            edittext.Unknown2 = reader.ReadInt32();
            edittext.Unknown3 = reader.ReadInt32();
            edittext.Unknown4 = reader.ReadInt32();
            edittext.Unknown5 = reader.ReadInt32();
            edittext.Unknown6 = reader.ReadInt32();
            edittext.Unknown7 = reader.ReadInt32();
            edittext.htmltextformat = reader.ReadString(0x100);
            return edittext;
        }

        private FuiReference ReadReference(EndiannessAwareBinaryReader reader)
        {
            FuiReference reference = new FuiReference();
            reference.SymbolIndex = reader.ReadInt32();
            reference.Name = reader.ReadString(0x40);
            reference.Index = reader.ReadInt32();
            return reference;
        }

        private string ReadString(EndiannessAwareBinaryReader reader)
        {
            return reader.ReadString(0x40);
        }

        private FuiTimelineEvent ReadTimelineEvent(EndiannessAwareBinaryReader reader)
        {
            FuiTimelineEvent timelineEvent = new FuiTimelineEvent();
            timelineEvent.EventType = reader.ReadInt16();
            timelineEvent.ObjectType = reader.ReadInt16();
            timelineEvent.Unknown0 = reader.ReadInt16();
            timelineEvent.Index = reader.ReadInt16();
            timelineEvent.Unknown1 = reader.ReadInt16();
            timelineEvent.NameIndex = reader.ReadInt16();
            timelineEvent.Matrix.ScaleX = reader.ReadSingle();
            timelineEvent.Matrix.ScaleY = reader.ReadSingle();
            timelineEvent.Matrix.RotateSkew0 = reader.ReadSingle();
            timelineEvent.Matrix.RotateSkew1 = reader.ReadSingle();
            timelineEvent.Matrix.TranslationX = reader.ReadSingle();
            timelineEvent.Matrix.TranslationY = reader.ReadSingle();
            timelineEvent.ColorTransform.RedMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.GreenMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.BlueMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.AlphaMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.RedAddTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.GreenAddTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.BlueAddTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.AlphaAddTerm = reader.ReadSingle();
            timelineEvent.Color = FuiRGBA.GetColor(reader.ReadInt32());
            return timelineEvent;
        }

        private FuiTimelineFrame ReadTimelineFrame(EndiannessAwareBinaryReader reader)
        {
            FuiTimelineFrame timelineFrame = new FuiTimelineFrame();
            timelineFrame.FrameName = reader.ReadString(0x40);
            timelineFrame.EventIndex = reader.ReadInt32();
            timelineFrame.EventCount = reader.ReadInt32();
            return timelineFrame;
        }

        private FuiVert ReadVert(EndiannessAwareBinaryReader reader)
        {
            FuiVert vert = new FuiVert();
            vert.X = reader.ReadSingle();
            vert.Y = reader.ReadSingle();
            return vert;
        }

        private FuiShapeComponent ReadShapeComponent(EndiannessAwareBinaryReader reader)
        {
            FuiShapeComponent shapeComponent = new FuiShapeComponent();
            shapeComponent.FillInfo.Type = reader.ReadInt32();
            shapeComponent.FillInfo.Color = FuiRGBA.GetColor(reader.ReadInt32());
            shapeComponent.FillInfo.BitmapIndex = reader.ReadInt32();
            shapeComponent.FillInfo.Matrix.ScaleX = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.ScaleY = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.RotateSkew0 = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.RotateSkew1 = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.TranslationX = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.TranslationY = reader.ReadSingle();
            shapeComponent.VertIndex = reader.ReadInt32();
            shapeComponent.VertCount = reader.ReadInt32();
            return shapeComponent;
        }

        private FuiShape ReadShape(EndiannessAwareBinaryReader reader)
        {
            FuiShape shape = new FuiShape();
            shape.Unknown = reader.ReadInt32();
            shape.ShapeComponentIndex = reader.ReadInt32();
            shape.ShapeComponentCount = reader.ReadInt32();
            shape.Rectangle.MinX = reader.ReadInt32();
            shape.Rectangle.MaxX = reader.ReadInt32();
            shape.Rectangle.MinY = reader.ReadInt32();
            shape.Rectangle.MaxY = reader.ReadInt32();
            return shape;
        }

        private FuiTimeline ReadTimeline(EndiannessAwareBinaryReader reader)
        {
            FuiTimeline timeline = new FuiTimeline();
            timeline.SymbolIndex = reader.ReadInt32();
            timeline.FrameIndex = reader.ReadInt16();
            timeline.FrameCount = reader.ReadInt16();
            timeline.ActionIndex = reader.ReadInt16();
            timeline.ActionCount = reader.ReadInt16();
            timeline.Rectangle.MinX = reader.ReadInt32();
            timeline.Rectangle.MaxX = reader.ReadInt32();
            timeline.Rectangle.MinY = reader.ReadInt32();
            timeline.Rectangle.MaxY = reader.ReadInt32();
            return timeline;
        }

        private FuiTimelineAction ReadTimelineAction(EndiannessAwareBinaryReader reader)
        {
            FuiTimelineAction timelineAction = new FuiTimelineAction();
            timelineAction.ActionType = reader.ReadInt16();
            timelineAction.Unknown = reader.ReadInt16();
            timelineAction.StringArg0 = reader.ReadString(0x40);
            timelineAction.StringArg1 = reader.ReadString(0x40);
            return timelineAction;
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
