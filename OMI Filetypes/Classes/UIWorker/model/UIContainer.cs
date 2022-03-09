using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIWorker.model
{
    public class UIContainer
    {
        public UIComponent.fuiHeader Header;
        public List<UIComponent.fuiTimeline> Timelines;
        public List<UIComponent.fuiTimelineAction> TimelineActions;
        public List<UIComponent.fuiShape> Shapes;
        public List<UIComponent.fuiShapeComponent> ShapeComponents;
        public List<UIComponent.fuiVert> Verts;
        public List<UIComponent.fuiTimelineFrame> TimelineFrames;
        public List<UIComponent.fuiTimelineEvent> TimelineEvents;
        public List<UIComponent.fuiTimelineEventName> TimelineEventNames;
        public List<UIComponent.fuiReference> References;
        public List<UIComponent.fuiEdittext> Edittexts;
        public List<UIComponent.fuiFontName> FontNames;
        public List<UIComponent.fuiSymbol> Symbols;
        public List<UIComponent.fuiImportAsset> ImportAssets;
        public List<UIComponent.fuiBitmap> Bitmaps;
        public List<UIComponent.fuiImage> Images = new List<UIComponent.fuiImage>();



        public UIContainer()
        {
            Header = new UIComponent.fuiHeader();
            Timelines = new List<UIComponent.fuiTimeline>();
            TimelineActions = new List<UIComponent.fuiTimelineAction>();
            Shapes = new List<UIComponent.fuiShape>();
            ShapeComponents = new List<UIComponent.fuiShapeComponent>();
            Verts = new List<UIComponent.fuiVert>();
            TimelineFrames = new List<UIComponent.fuiTimelineFrame>();
            TimelineEvents = new List<UIComponent.fuiTimelineEvent>();
            TimelineEventNames = new List<UIComponent.fuiTimelineEventName>();
            References = new List<UIComponent.fuiReference>();
            Edittexts = new List<UIComponent.fuiEdittext>();
            FontNames = new List<UIComponent.fuiFontName>();
            Symbols = new List<UIComponent.fuiSymbol>();
            ImportAssets = new List<UIComponent.fuiImportAsset>();
            Bitmaps = new List<UIComponent.fuiBitmap>();
        }
    }
}
