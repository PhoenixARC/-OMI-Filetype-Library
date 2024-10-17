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
using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Collections.Generic;
using OMI.Extentions;
using OMI.Formats.FUI;
using OMI.Formats.FUI.Components;
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
                UIContainer.Header.Signature = reader.ReadInt64(Endianness.LittleEndian);
                UIContainer.Header.ContentSize = reader.ReadInt32();
                UIContainer.Header.SwfFileName = reader.ReadString(0x40);

                UIContainer.Timelines = new List<FuiTimeline>(reader.ReadInt32());
                UIContainer.TimelineEventNames = new List<string>(reader.ReadInt32());
                UIContainer.TimelineActions = new List<FuiTimelineAction>(reader.ReadInt32());
                UIContainer.Shapes = new List<FuiShape>(reader.ReadInt32());
                UIContainer.ShapeComponents = new List<FuiShapeComponent>(reader.ReadInt32());
                UIContainer.Verts = new List<PointF>(reader.ReadInt32());
                UIContainer.TimelineFrames = new List<FuiTimelineFrame>(reader.ReadInt32());
                UIContainer.TimelineEvents = new List<FuiTimelineEvent>(reader.ReadInt32());
                UIContainer.References = new List<FuiReference>(reader.ReadInt32());
                UIContainer.Edittexts = new List<FuiEdittext>(reader.ReadInt32());
                UIContainer.Symbols = new List<FuiSymbol>(reader.ReadInt32());
                UIContainer.Bitmaps = new List<FuiBitmap>(reader.ReadInt32());

                int imagesSize = reader.ReadInt32();

                UIContainer.FontNames = new List<FuiFontName>(reader.ReadInt32());
                UIContainer.ImportAssets = new List<string>(reader.ReadInt32());

                UIContainer.Header.FrameSize.Min.X = reader.ReadSingle();
                UIContainer.Header.FrameSize.Max.X = reader.ReadSingle();
                UIContainer.Header.FrameSize.Min.Y = reader.ReadSingle();
                UIContainer.Header.FrameSize.Max.Y = reader.ReadSingle();

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
            tline.ImageFormat = (FuiBitmap.FuiImageFormat)reader.ReadInt32();
            tline.ImageSize.Width = reader.ReadInt32();
            tline.ImageSize.Height = reader.ReadInt32();
            tline.Offset = reader.ReadInt32();
            tline.Size = reader.ReadInt32();
            tline.ZlibDataOffset = reader.ReadInt32();
            //tline.BindHandle = reader.ReadInt32();
            reader.ReadInt32();
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
            fontName.Name = reader.ReadString(0x40);
            reader.ReadBytes(0xc0); // unknown values
            return fontName;
        }

        private FuiEdittext ReadEdittext(EndiannessAwareBinaryReader reader)
        {
            FuiEdittext edittext = new FuiEdittext();
            edittext.Unknown0 = reader.ReadInt32();
            edittext.Rectangle.Min.X = reader.ReadSingle();
            edittext.Rectangle.Max.X = reader.ReadSingle();
            edittext.Rectangle.Min.Y = reader.ReadSingle();
            edittext.Rectangle.Max.Y = reader.ReadSingle();
            edittext.FontId = reader.ReadInt32();
            edittext.FontScale = reader.ReadSingle();
            edittext.Color = (reader.ReadUInt32());
            edittext.Alignment = reader.ReadInt32();
            edittext.Unknown3 = reader.ReadInt32();
            edittext.Unknown4 = reader.ReadInt32();
            edittext.Unknown5 = reader.ReadInt32();
            edittext.Unknown6 = reader.ReadInt32();
            edittext.Unknown7 = reader.ReadInt32();
            edittext.htmlSource = reader.ReadString(0x100);
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
            timelineEvent.Matrix.Scale.Width = reader.ReadSingle();
            timelineEvent.Matrix.Scale.Height = reader.ReadSingle();
            timelineEvent.Matrix.RotateSkew0 = reader.ReadSingle();
            timelineEvent.Matrix.RotateSkew1 = reader.ReadSingle();
            timelineEvent.Matrix.Translation.X = reader.ReadSingle();
            timelineEvent.Matrix.Translation.Y = reader.ReadSingle();
            timelineEvent.ColorTransform.RedMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.GreenMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.BlueMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.AlphaMultTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.RedAddTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.GreenAddTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.BlueAddTerm = reader.ReadSingle();
            timelineEvent.ColorTransform.AlphaAddTerm = reader.ReadSingle();
            timelineEvent.Color = (reader.ReadUInt32());
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

        private PointF ReadVert(EndiannessAwareBinaryReader reader)
        {
            PointF vert = new PointF();
            vert.X = reader.ReadSingle();
            vert.Y = reader.ReadSingle();
            return vert;
        }

        private FuiShapeComponent ReadShapeComponent(EndiannessAwareBinaryReader reader)
        {
            FuiShapeComponent shapeComponent = new FuiShapeComponent();
            shapeComponent.FillInfo.Type = (FuiFillStyle.FillType)reader.ReadInt32();
            shapeComponent.FillInfo.Color = (reader.ReadUInt32());
            shapeComponent.FillInfo.BitmapIndex = reader.ReadInt32();
            shapeComponent.FillInfo.Matrix.Scale.Width = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.Scale.Height = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.RotateSkew0 = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.RotateSkew1 = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.Translation.X = reader.ReadSingle();
            shapeComponent.FillInfo.Matrix.Translation.Y = reader.ReadSingle();
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
            shape.Rectangle.Min.X = reader.ReadSingle();
            shape.Rectangle.Max.X = reader.ReadSingle();
            shape.Rectangle.Min.Y = reader.ReadSingle();
            shape.Rectangle.Max.Y = reader.ReadSingle();
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
            timeline.Rectangle.Min.X = reader.ReadSingle();
            timeline.Rectangle.Max.X = reader.ReadSingle();
            timeline.Rectangle.Min.Y = reader.ReadSingle();
            timeline.Rectangle.Max.Y = reader.ReadSingle();
            return timeline;
        }

        private FuiTimelineAction ReadTimelineAction(EndiannessAwareBinaryReader reader)
        {
            FuiTimelineAction timelineAction = new FuiTimelineAction();
            timelineAction.ActionType = reader.ReadByte();
            timelineAction.Unknown = reader.ReadByte();
            timelineAction.FrameIndex = reader.ReadInt16();
            timelineAction.StringArg0 = reader.ReadString(0x40);
            timelineAction.StringArg1 = reader.ReadString(0x40);
            return timelineAction;
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
