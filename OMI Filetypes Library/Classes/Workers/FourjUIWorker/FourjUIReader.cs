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
                Stopwatch stopwatch = Stopwatch.StartNew();

                UIContainer.Header.Signature = reader.ReadInt64(Endianness.BigEndian);
                UIContainer.Header.ContentSize = reader.ReadInt32();
                UIContainer.Header.SwfFileName = reader.ReadString(0x40);
                UIContainer.Header.fuiTimelineCount = reader.ReadInt32();
                UIContainer.Header.fuiTimelineEventNameCount = reader.ReadInt32();
                UIContainer.Header.fuiTimelineActionCount = reader.ReadInt32();
                UIContainer.Header.fuiShapeCount = reader.ReadInt32();
                UIContainer.Header.fuiShapeComponentCount = reader.ReadInt32();
                UIContainer.Header.fuiVertCount = reader.ReadInt32();
                UIContainer.Header.fuiTimelineFrameCount = reader.ReadInt32();
                UIContainer.Header.fuiTimelineEventCount = reader.ReadInt32();
                UIContainer.Header.fuiReferenceCount = reader.ReadInt32();
                UIContainer.Header.fuiEdittextCount = reader.ReadInt32();
                UIContainer.Header.fuiSymbolCount = reader.ReadInt32();
                UIContainer.Header.fuiBitmapCount = reader.ReadInt32();
                UIContainer.Header.imagesSize = reader.ReadInt32();
                UIContainer.Header.fuiFontNameCount = reader.ReadInt32();
                UIContainer.Header.fuiImportAssetCount = reader.ReadInt32();
                UIContainer.Header.frameSize.MinX = reader.ReadSingle();
                UIContainer.Header.frameSize.MaxX = reader.ReadSingle();
                UIContainer.Header.frameSize.MinY = reader.ReadSingle();
                UIContainer.Header.frameSize.MaxY = reader.ReadSingle();
                

                for (int i = 0; i < UIContainer.Header.fuiTimelineCount; i++)
                {
                    FuiTimeline tline = new FuiTimeline();
                    tline.SymbolIndex = reader.ReadInt32();
                    tline.FrameIndex = reader.ReadInt16();
                    tline.FrameCount = reader.ReadInt16();
                    tline.ActionIndex = reader.ReadInt16();
                    tline.ActionCount = reader.ReadInt16();
                    tline.Rectangle.MinX = reader.ReadSingle();
                    tline.Rectangle.MaxX = reader.ReadSingle();
                    tline.Rectangle.MinY = reader.ReadSingle();
                    tline.Rectangle.MaxY = reader.ReadSingle();
                    UIContainer.Timelines.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiTimelineActionCount; i++)
                {
                    UIContainer.TimelineActions.Add(new FuiTimelineAction
                    {
                        ActionType = reader.ReadByte(),
                        Unknown = reader.ReadByte(),
                        FrameIndex = reader.ReadInt16(),
                        StringArg0 = reader.ReadString(0x40),
                        StringArg1 = reader.ReadString(0x40)
                    });
                }

                for (int i = 0; i < UIContainer.Header.fuiShapeCount; i++)
                {
                    FuiShape tline = new FuiShape();
                    tline.Unknown = reader.ReadInt32();
                    tline.ShapeComponentIndex = reader.ReadInt32();
                    tline.ShapeComponentCount = reader.ReadInt32();
                    tline.Rectangle.MinX = reader.ReadInt32();
                    tline.Rectangle.MaxX = reader.ReadInt32();
                    tline.Rectangle.MinY = reader.ReadInt32();
                    tline.Rectangle.MaxY = reader.ReadInt32();
                    UIContainer.Shapes.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiShapeComponentCount; i++)
                {
                    FuiShapeComponent tline = new FuiShapeComponent();
                    tline.FillInfo.Type = reader.ReadInt32();
                    tline.FillInfo.Color.RGBA = reader.ReadInt32();
                    tline.FillInfo.BitmapIndex = reader.ReadInt32();
                    tline.FillInfo.Matrix.ScaleX = reader.ReadSingle();
                    tline.FillInfo.Matrix.ScaleY = reader.ReadSingle();
                    tline.FillInfo.Matrix.RotateSkew0 = reader.ReadSingle();
                    tline.FillInfo.Matrix.RotateSkew1 = reader.ReadSingle();
                    tline.FillInfo.Matrix.TranslationX = reader.ReadSingle();
                    tline.FillInfo.Matrix.TranslationY = reader.ReadSingle();
                    tline.VertIndex = reader.ReadInt32();
                    tline.VertCount = reader.ReadInt32();
                    UIContainer.ShapeComponents.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiVertCount; i++)
                {
                    FuiVert tline = new FuiVert();
                    tline.X = reader.ReadSingle();
                    tline.Y = reader.ReadSingle();
                    UIContainer.Verts.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiTimelineFrameCount; i++)
                {
                    FuiTimelineFrame tline = new FuiTimelineFrame();
                    tline.FrameName = reader.ReadString(0x40);
                    tline.EventIndex = reader.ReadInt32();
                    tline.EventCount = reader.ReadInt32();
                    UIContainer.TimelineFrames.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiTimelineEventCount; i++)
                {
                    FuiTimelineEvent tline = new FuiTimelineEvent();
                    tline.EventType = reader.ReadInt16();
                    tline.ObjectType = reader.ReadInt16();
                    tline.Unknown0 = reader.ReadInt16();
                    tline.Index = reader.ReadInt16();
                    tline.Unknown1 = reader.ReadInt16();
                    tline.NameIndex = reader.ReadInt16();
                    tline.matrix.ScaleX = reader.ReadSingle();
                    tline.matrix.ScaleY = reader.ReadSingle();
                    tline.matrix.RotateSkew0 = reader.ReadSingle();
                    tline.matrix.RotateSkew1 = reader.ReadSingle();
                    tline.matrix.TranslationX = reader.ReadSingle();
                    tline.matrix.TranslationY = reader.ReadSingle();
                    tline.ColorTransform.RedMultTerm = reader.ReadSingle();
                    tline.ColorTransform.GreenMultTerm = reader.ReadSingle();
                    tline.ColorTransform.BlueMultTerm = reader.ReadSingle();
                    tline.ColorTransform.AlphaMultTerm = reader.ReadSingle();
                    tline.ColorTransform.RedAddTerm = reader.ReadSingle();
                    tline.ColorTransform.GreenAddTerm = reader.ReadSingle();
                    tline.ColorTransform.BlueAddTerm = reader.ReadSingle();
                    tline.ColorTransform.AlphaAddTerm = reader.ReadSingle();
                    tline.Color.RGBA = reader.ReadInt32();
                    UIContainer.TimelineEvents.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiTimelineEventNameCount; i++)
                {
                    FuiTimelineEventName tline = new FuiTimelineEventName();
                    tline.Name = reader.ReadString(0x40);
                    UIContainer.TimelineEventNames.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiReferenceCount; i++)
                {
                    FuiReference tline = new FuiReference();
                    tline.SymbolIndex = reader.ReadInt32();
                    tline.Name = reader.ReadString(0x40);
                    tline.Index = reader.ReadInt32();
                    UIContainer.References.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiEdittextCount; i++)
                {
                    FuiEdittext tline = new FuiEdittext();
                    tline.Unknown0 = reader.ReadInt32();
                    tline.Rectangle.MinX = reader.ReadSingle();
                    tline.Rectangle.MaxX = reader.ReadSingle();
                    tline.Rectangle.MinY = reader.ReadSingle();
                    tline.Rectangle.MaxY = reader.ReadSingle();
                    tline.FontID = reader.ReadInt32();
                    tline.Unknown1 = reader.ReadSingle();
                    tline.Color.RGBA = reader.ReadInt32();
                    tline.Unknown2 = reader.ReadInt32();
                    tline.Unknown3 = reader.ReadInt32();
                    tline.Unknown4 = reader.ReadInt32();
                    tline.Unknown5 = reader.ReadInt32();
                    tline.Unknown6 = reader.ReadInt32();
                    tline.Unknown7 = reader.ReadInt32();
                    tline.htmlSource = reader.ReadString(0x100);
                    UIContainer.Edittexts.Add(tline);
                }

                for (int i = 0; i < UIContainer.Header.fuiFontNameCount; i++)
                {
                    UIContainer.FontNames.Add(new FuiFontName
                    {
                        ID = reader.ReadInt32(),
                        Name = reader.ReadString(0x40)
                    });
                    reader.ReadBytes(0xc0); // unknown values
                }

                for (int i = 0; i < UIContainer.Header.fuiSymbolCount; i++)
                {
                    UIContainer.Symbols.Add(new FuiSymbol
                    {
                        SymbolName = reader.ReadString(0x40),
                        ObjectType = reader.ReadInt32(),
                        Index = reader.ReadInt32()
                    });
                }

                for (int i = 0; i < UIContainer.Header.fuiImportAssetCount; i++)
                {
                    UIContainer.ImportAssets.Add(new FuiImportAsset
                    {
                        Name = reader.ReadString(0x40)
                    });
                }

                for (int i = 0; i < UIContainer.Header.fuiBitmapCount; i++)
                {
                    FuiBitmap tline = new FuiBitmap();
                    tline.Unknown0 = reader.ReadInt32();
                    tline.ImageFormat = reader.ReadInt32();
                    tline.Width = reader.ReadInt32();
                    tline.Height = reader.ReadInt32();
                    tline.Offset = reader.ReadInt32();
                    tline.Size = reader.ReadInt32();
                    tline.ZlibDataOffset = reader.ReadInt32();
                    tline.Unknown1 = reader.ReadInt32();
                    UIContainer.Bitmaps.Add(tline);
                }

                foreach (FuiBitmap bitmap in UIContainer.Bitmaps)
                {
                    UIContainer.ImagesData.Add(reader.ReadBytes(bitmap.Size));
                }

                stopwatch.Stop();
                Debug.WriteLine("Completed in: " + stopwatch.Elapsed, category: nameof(FourjUIReader.FromStream));
            }
            return UIContainer;
        }

        object IDataFormatReader.FromStream(Stream stream) => FromStream(stream);

        object IDataFormatReader.FromFile(string filename) => FromFile(filename);
    }
}
