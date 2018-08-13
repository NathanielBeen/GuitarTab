using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IViewModelFactory
    {
        void runFactory(StartingFactory fac, Part part);
        void initView(Part part);

        Part getPart();
        BaseViewModel getMainView();
    }

    public class StartingFactory
    {
        private const string THICKNESS = "thickness";
        private const string BAR_THICKNESS = "barThickness";
        private const string DASH_LENGTH = "dashLength";
        private const string DELETE_IMG = "deleteButton";

        private const string HOVER_COLOR = "hoverColor";
        private const string SELECTED_COLOR = "selectedColor";

        public SettingsReader Reader { get; }
        public VisualInfo Info { get; }

        public StartingFactory()
        {
            Reader = createSettingsReader();
            Info = createVisualInfo();
        }

        private SettingsReader createSettingsReader()
        {
            return new SettingsReader();
        }

        private VisualInfo createVisualInfo()
        {
            var dimensions = new VisualDimensions(Reader.getDictionary(SettingsReader.DIMENSION), Reader.getDictionary(SettingsReader.LENGTH));

            Color selected_color = (Color)ColorConverter.ConvertFromString(Reader.getDictionaryEntry(SettingsReader.OTHER, SELECTED_COLOR));
            Color hover_color = (Color)ColorConverter.ConvertFromString(Reader.getDictionaryEntry(SettingsReader.OTHER, HOVER_COLOR));
            var drawing = new DrawingObjects(int.Parse(Reader.getDictionaryEntry(SettingsReader.OTHER, THICKNESS)),
                int.Parse(Reader.getDictionaryEntry(SettingsReader.OTHER, BAR_THICKNESS)), int.Parse(Reader.getDictionaryEntry(SettingsReader.OTHER, DASH_LENGTH)),
                selected_color, hover_color);

            var images = new TabImages(Reader.getDictionary(SettingsReader.REST), Reader.getDictionary(SettingsReader.EFFECT));
            var position = new CurrentPosition(dimensions);
            return new VisualInfo(dimensions, drawing, images, position);
        }
    }

    class ViewingViewModelFactory : IViewModelFactory
    {
        private SettingsReader reader;
        private VisualInfo info;

        private GuiObjectTree tree;

        private ViewingCanvasView canvas_view;
        private VisualsView visuals_view;
        private TabScrollView scroll_view;

        private ViewingView main_view;

        public ViewingViewModelFactory() { }

        public void runFactory(StartingFactory fac, Part part)
        {
            reader = fac.Reader;
            info = fac.Info;
            info.Position.reset();

            tree = createTree();

            canvas_view = createCanvasView();
            visuals_view = createVisualsView();
            scroll_view = createScrollView();

            main_view = createMainView();

            initView(part);
        }

        private GuiObjectTree createTree()
        {
            var factory = new StaticGuiObjectFactory(info);
            var added = new TreeChangedHolding();
            var collection = new TreeVisualCollection();
            return new GuiObjectTree(factory, added, collection);
        }

        private ViewingCanvasView createCanvasView()
        {
            return new ViewingCanvasView(null);
        }

        private VisualsView createVisualsView()
        {
            return new VisualsView(tree.getTreeVisuals());
        }

        private TabScrollView createScrollView()
        {
            return new TabScrollView();
        }

        private ViewingView createMainView()
        {
            return new ViewingView(canvas_view, visuals_view, scroll_view);
        }

        public void initView(Part part)
        {
            createDimensionUpdaters();
            setPart(part);
            tree.Root?.updateBounds();
            tree.Root?.refreshVisual();
            GuiTreeUpdater.rebarPart(tree.Root);
        }

        private void createDimensionUpdaters()
        {
            var page_height = new PageHeightUpdater(400);
            var screen_height = new DimensionUpdater(DimensionType.ScreenHeight);
            var page_width = new PageWidthUpdater(info.Dimensions.PageWidth);
            var scroll_amount = new DimensionUpdater(DimensionType.ScrollAmount);

            screen_height.addReciever(page_height);
            page_height.addReciever(visuals_view);
            page_width.addReciever(canvas_view);
            scroll_amount.addReciever(canvas_view);

            tree.RootChanged += page_height.handleRecieverChanged;

            info.Position.HeightChanged += page_height.handleDimensionChanged;
            canvas_view.HeightChanged += screen_height.handleDimensionChanged;
            canvas_view.WidthChanged += page_width.handleDimensionChanged;
            scroll_view.ScrollChanged += scroll_amount.handleDimensionChanged;
        }

        private void setPart(Part part) { tree.buildObject(null, part); }

        public Part getPart() { return tree.Root?.BaseObject as Part; }

        public BaseViewModel getMainView() { return main_view; }
    }
}
