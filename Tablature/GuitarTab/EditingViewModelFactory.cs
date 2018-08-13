using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class EditingViewModelFactory : IViewModelFactory
    {
        private const string THICKNESS = "thickness";
        private const string BAR_THICKNESS = "barThickness";
        private const string DASH_LENGTH = "dashLength";
        private const string DELETE_IMG = "deleteButton";

        private const string HOVER_COLOR = "hoverColor";
        private const string SELECTED_COLOR = "selectedColor";

        private SettingsReader reader; //
        private VisualInfo info; //

        private Selected selected;
        private MouseStateConverter converter;
        private CommandSelections selections;
        private GuiCommandExecutor executor;
        private EditingMouseHandler handler; 
        private GuiObjectTree tree; 
        private GuiTreeUpdater updater;

        private LengthView length_view;
        private DeleteView delete_view;
        private AddItemView add_item_view;
        private EditingCanvasView canvas_view; 
        private PropertyMenuView property_view;
        private FretMenuView fret_view;
        private NoteSelectView select_view;
        private BPMTimeSigView time_sig_view;
        private PartSettingsMenuView part_view;
        private VisualsView visuals_view;
        private TabScrollView scroll_view;

        private EditingView main_view;

        public EditingViewModelFactory() { }

        public void runFactory(StartingFactory fac, Part part)
        {
            reader = fac.Reader;
            info = fac.Info;
            info.Position.reset();
            selected = createSelected();

            converter = createMouseStateConverter();
            selections = createCommandSelections();
            executor = createExecutor();
            tree = createTree();
            handler = createHandler();
            updater = createUpdater();

            length_view = createLengthView();
            delete_view = createDeleteView();
            add_item_view = createAddItemView();
            canvas_view = createCanvasView();
            property_view = createPropertyView();
            fret_view = createFretView();
            select_view = createNoteSelectView();
            time_sig_view = createBPMTimeSigView();
            part_view = createPartMenuView();
            visuals_view = createVisualsView();
            scroll_view = createScollView();

            main_view = createMainView();

            initView(part);
        }

        private Selected createSelected()
        {
            return new Selected();
        }

        private MouseStateConverter createMouseStateConverter()
        {
            return new MouseStateConverter();
        }

        private CommandSelections createCommandSelections()
        {
            return new CommandSelections(converter);
        }

        private GuiCommandExecutor createExecutor()
        {
            var executor = new CommandExecutor();
            return new GuiCommandExecutor(executor, selections, info);
        }

        private GuiObjectTree createTree()
        {
            var factory = new DynamicGuiObjectFactory(info, executor);
            var added = new TreeChangedHolding();
            var collection = new TreeVisualCollection();
            return new GuiObjectTree(factory, added, collection);
        }

        private EditingMouseHandler createHandler()
        {
            return new EditingMouseHandler(tree, selected, converter);
        }

        private GuiTreeUpdater createUpdater()
        {
            var updator = new GuiTreeUpdater(tree, info.Position);
            executor.Updater = updator;
            return updator;
        }

        private LengthView createLengthView()
        {
            return new LengthView(selections, reader.getDictionary(SettingsReader.LENGTH_IMG), reader.getDictionary(SettingsReader.TUPLE));
        }

        private DeleteView createDeleteView()
        {
            return new DeleteView(executor, selected, reader.getDictionaryEntry(SettingsReader.OTHER_IMG, DELETE_IMG));
        }

        private AddItemView createAddItemView()
        {
            return new AddItemView(converter, reader.getDictionary(SettingsReader.ADDITEM));
        }

        private EditingCanvasView createCanvasView()
        {
            var state_view = new MouseStateView(reader.getDictionary(SettingsReader.MOUSESTATE), converter);
            var hover_view = new MouseHoverView(info.DrawingObjects.HoverBrush);
            var selected_view = new MouseSelectedView(info.DrawingObjects.SelectedBrush);
            var drag_view = new MouseDragView();
            selected.SelectedView = selected_view;
            var canvas = new EditingCanvasView(state_view, hover_view, selected_view, drag_view, handler);
            return canvas;
        }

        private PropertyMenuView createPropertyView()
        {
            var fac = new PropertyMenuFactory(executor);
            return new PropertyMenuView(fac, info);
        }

        private FretMenuView createFretView()
        {
            return new FretMenuView();
        }

        private NoteSelectView createNoteSelectView()
        {
            return new NoteSelectView();
        }

        private BPMTimeSigView createBPMTimeSigView()
        {
            return new BPMTimeSigView(selections, executor, selected, reader.getDictionary(SettingsReader.LENGTH_IMG), reader.getDictionary(SettingsReader.OTHER_IMG), 120, 4, NoteLength.Quarter);
        }

        private PartSettingsMenuView createPartMenuView()
        {
            return new PartSettingsMenuView(tree);
        }

        private VisualsView createVisualsView()
        {
            return new VisualsView(tree.getTreeVisuals());
        }

        private TabScrollView createScollView()
        {
            return new TabScrollView();
        }

        private EditingView createMainView()
        {
            return new EditingView(length_view, delete_view, add_item_view, canvas_view, property_view, fret_view, select_view,
                time_sig_view, part_view, visuals_view, scroll_view, executor, handler);
        }

        public void initView(Part part)
        {
            createDimensionUpdaters();
            initOrSetPart(part);
        }

        private void createDimensionUpdaters()
        {
            var page_height = new PageHeightUpdater(400);
            var screen_height = new DimensionUpdater(DimensionType.ScreenHeight);
            var page_width = new PageWidthUpdater(info.Dimensions.PageWidth);
            var scroll_amount = new DimensionUpdater(DimensionType.ScrollAmount);

            screen_height.addReciever(page_height);
            page_height.addReciever(visuals_view);
            page_width.addReciever(canvas_view.SelectedView);
            page_width.addReciever(canvas_view);
            scroll_amount.addReciever(canvas_view);
            scroll_amount.addReciever(canvas_view.SelectedView);

            tree.RootChanged += page_height.handleRecieverChanged;

            info.Position.HeightChanged += page_height.handleDimensionChanged;
            canvas_view.HeightChanged += screen_height.handleDimensionChanged;
            canvas_view.WidthChanged += page_width.handleDimensionChanged;
            scroll_view.ScrollChanged += scroll_amount.handleDimensionChanged;
        }

        private void initOrSetPart(Part part)
        {
            if (part == null)
            {
                var click = new NodeClick(new System.Windows.Point(0, 0));
                executor.executeInitPart(click, 120, 4, NoteLength.Quarter);
            }
            else
            {
                tree.buildObject(null, part);
                tree.Root?.updateBounds();
                GuiTreeUpdater.rebarPart(tree.Root);
            }
        }

        public Part getPart() { return tree.Root?.BaseObject as Part; }

        public BaseViewModel getMainView() { return main_view; }
    }

    public class SettingsReader
    {
        public const int DIMENSION = 0;
        public const int LENGTH = 1;
        public const int LENGTH_IMG = 2;
        public const int REST = 3;
        public const int EFFECT = 4;
        public const int ADDITEM = 5;
        public const int MOUSESTATE = 6;
        public const int TUPLE = 7;
        public const int OTHER_IMG = 8;
        public const int OTHER = 9;
        public const int LOCATIONS = 10;

        public const string LENGTH_LOC = "LengthLoc";
        public const string REST_LOC = "RestLoc";
        public const string EFFECT_LOC = "EffectLoc";
        public const string ADD_LOC = "AddItemLoc";
        public const string STATE_LOC = "MouseStateLoc";
        public const string TUPLE_LOC = "TupleLoc";
        public const string OTHER_LOC = "OtherImageLoc";
        public const string LOC = "ResourceLoc";

        public const string DIVISION_TAG = "<>";
        public const string ENTRY_SPLIT = "_:_";

        private Dictionary<int, Dictionary<string, string>> master_dict;

        public SettingsReader()
        {
            master_dict = initEmptyMasterDict();
            readInFile();
        }

        public Dictionary<int, Dictionary<string, string>> initEmptyMasterDict()
        {
            var master = new Dictionary<int, Dictionary<string, string>>();
            for (int i = DIMENSION; i <= LOCATIONS; i++)
            {
                master.Add(i, new Dictionary<string, string>());
            }
            return master;
        }

        public void readInFile()
        {
            int curr_dict = DIMENSION;
            var doc = Assembly.GetExecutingAssembly().GetManifestResourceStream("GuitarTab.Resources.ResourceDocument.txt");
            using (StreamReader reader = new StreamReader(doc))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (line.Contains(DIVISION_TAG)) { curr_dict++; }
                    else { readEntryIntoDictionary(curr_dict, line); }
                }
            }
            correctAllDictionaries();
        }

        public void readEntryIntoDictionary(int dict_val, string val)
        {
            string[] entry = val.Split(ENTRY_SPLIT.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            Dictionary<string, string> dict = getDictionary(dict_val);
            dict?.Add(entry[0], entry[1]);
        }

        public void correctAllDictionaries()
        {
            var new_master = new Dictionary<int, Dictionary<string, string>>();
            foreach (var entry in master_dict) { new_master.Add(entry.Key, correctDictionaryLocation(entry.Key)); }
            master_dict = new_master;
        }

        public Dictionary<string, string> correctDictionaryLocation(int type)
        {
            if (isImageDict(type))
            {
                var corrected_dict = new Dictionary<string, string>();

                string prefix = getLocationPrefix(type);
                foreach (var key in master_dict[type].Keys)
                {

                    corrected_dict.Add(key, prefix + master_dict[type][key]);
                }
                return corrected_dict;
            }
            return master_dict[type];
        }

        public bool isImageDict(int type)
        {
            return (type == LENGTH_IMG || type == REST || type == EFFECT || type == ADDITEM || type == MOUSESTATE || type == TUPLE || type == OTHER_IMG);
        }

        public string getLocationPrefix(int type)
        {
            Dictionary<string, string> dict = master_dict[LOCATIONS];
            string prefix = dict[LOC];
            switch (type)
            {
                case LENGTH_IMG:
                    prefix += dict[LENGTH_LOC];
                    break;
                case REST:
                    prefix += dict[REST_LOC];
                    break;
                case EFFECT:
                    prefix += dict[EFFECT_LOC];
                    break;
                case ADDITEM:
                    prefix += dict[ADD_LOC];
                    break;
                case MOUSESTATE:
                    prefix += dict[STATE_LOC];
                    break;
                case TUPLE:
                    prefix += dict[TUPLE_LOC];
                    break;
                case OTHER_IMG:
                    prefix += dict[OTHER_LOC];
                    break;
                default:
                    return "";
            }
            return prefix;
        }

        public Dictionary<string, string> getDictionary(int type)
        {
            if (master_dict.TryGetValue(type, out Dictionary<string, string> dict)) { return dict; }
            else { return null; }
        }

        public string getDictionaryEntry(int type, string key)
        {
            Dictionary<string, string> dict = getDictionary(type);
            return dict?[key] ?? null;
        }

        public static BitmapImage getImageFromLocation(string loc)
        {
            BitmapImage bitmap;
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(loc))
            {
                bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
            }
            return bitmap;
        }
    }
}
