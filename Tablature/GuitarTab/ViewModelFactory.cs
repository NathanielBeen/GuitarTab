using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GuitarTab
{
    public class ViewModelFactory
    {
        private const string THICKNESS = "thickness";
        private const string BAR_THICKNESS = "barThickness";
        private const string DASH_LENGTH = "dashLength";
        private const string DELETE_IMG = "deleteButton";

        private const string HOVER_COLOR = "hoverColor";
        private const string SELECTED_COLOR = "selectedColor";

        private SettingsReader reader;
        private MouseStateConverter converter;
        private CommandSelections selections;
        private VisualInfo info;
        private GuiCommandExecutor executor;
        private Selected selected;
        private MouseHandler handler;
        private GuiObjectFactory factory;
        private GuiObjectTree tree;
        private GuiTreeUpdater updater;

        private LengthView length_view;
        private DeleteView delete_view;
        private AddItemView add_item_view;
        private MouseCanvasView canvas_view;
        private PropertyMenuView property_view;
        private FretMenuView fret_view;
        private MainView main_view;

        public ViewModelFactory() { }

        public void runFactory(string file_loc)
        {
            reader = createSettingsReader(file_loc);
            converter = createMouseStateConverter();
            selections = createCommandSelections();
            info = createVisualInfo();
            executor = createExecutor();
            selected = createSelected();
            factory = createFactory();
            tree = createTree();
            handler = createHandler();
            updater = createUpdater();

            length_view = createLengthView();
            delete_view = createDeleteView();
            add_item_view = createAddItemView();
            canvas_view = createCanvasView();
            property_view = createPropertyView();
            fret_view = createFretView();
            main_view = createMainView();
        }
        
        public SettingsReader createSettingsReader(string file_loc)
        {
            return new SettingsReader(file_loc);
        }

        public MouseStateConverter createMouseStateConverter()
        {
            return new MouseStateConverter();
        }

        public CommandSelections createCommandSelections()
        {
            return new CommandSelections(converter);
        }

        public VisualInfo createVisualInfo()
        {
            var dimensions = new VisualDimensions(reader.getDictionary(SettingsReader.DIMENSION), reader.getDictionary(SettingsReader.LENGTH));

            Color selected_color = (Color)ColorConverter.ConvertFromString(reader.getDictionaryEntry(SettingsReader.OTHER, SELECTED_COLOR));
            Color hover_color = (Color)ColorConverter.ConvertFromString(reader.getDictionaryEntry(SettingsReader.OTHER, HOVER_COLOR));
            var drawing = new DrawingObjects(int.Parse(reader.getDictionaryEntry(SettingsReader.OTHER, THICKNESS)), 
                int.Parse(reader.getDictionaryEntry(SettingsReader.OTHER, BAR_THICKNESS)), int.Parse(reader.getDictionaryEntry(SettingsReader.OTHER, DASH_LENGTH)),
                selected_color, hover_color);

            var images = new TabImages(reader.getDictionary(SettingsReader.REST), reader.getDictionary(SettingsReader.EFFECT));
            var position = new CurrentPosition(dimensions);
            return new VisualInfo(dimensions, drawing, images, position);
        }

        public GuiCommandExecutor createExecutor()
        {
            var executor = new CommandExecutor();
            return new GuiCommandExecutor(executor, selections, info);
        }

        public GuiObjectFactory createFactory()
        {
            return new GuiObjectFactory(info, executor);
        }

        public GuiObjectTree createTree()
        {
            var added = new TreeChangedHolding();
            var holding = new TreeRemovedHolding(20);
            var collection = new TreeVisualCollection();
            return new GuiObjectTree(factory, added, holding, collection);
        }

        public Selected createSelected()
        {
            return new Selected();
        }

        public MouseHandler createHandler()
        {
            return new MouseHandler(tree, selected, converter);
        }

        public GuiTreeUpdater createUpdater()
        {
            var updator = new GuiTreeUpdater(tree, factory, info.Position);
            executor.Updater = updator;
            return updator;
        }

        public LengthView createLengthView()
        {
            return new LengthView(selections, reader.getDictionary(SettingsReader.LENGTH_IMG));
        }

        public DeleteView createDeleteView()
        {
            return new DeleteView(executor, selected, reader.getDictionaryEntry(SettingsReader.OTHER_IMG, DELETE_IMG));
        }

        public AddItemView createAddItemView()
        {
            return new AddItemView(converter, reader.getDictionary(SettingsReader.ADDITEM));
        }

        public MouseCanvasView createCanvasView()
        {
            var state_view = new MouseStateView(reader.getDictionary(SettingsReader.MOUSESTATE), converter);
            var hover_view = new MouseHoverView(info.DrawingObjects.HoverBrush);
            var selected_view = new MouseSelectedView(info.DrawingObjects.SelectedBrush);
            selected.SelectedView = selected_view;
            var canvas = new MouseCanvasView(state_view, hover_view, selected_view, handler);
            return canvas;
        }

        public PropertyMenuView createPropertyView()
        {
            var fac = new PropertyMenuFactory(executor);
            return new PropertyMenuView(fac);
        }

        public FretMenuView createFretView()
        {
            return new FretMenuView();
        }

        public MainView createMainView()
        {
            return new MainView(length_view, delete_view, add_item_view, canvas_view, property_view, fret_view, executor, handler);
        }

        public void linkToView(MainControl control)
        {
            control.DataContext = main_view;
            control.canvas.View = canvas_view;
            control.tabVisuals.setVisualCollection(tree.getTreeVisuals());
        }

        public void initPart()
        {
            var click = new NodeClick(new System.Windows.Point(0, 0));
            executor.executeInitPart(click, 120, 4, NoteLength.Quarter);
        }
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
        public const int OTHER_IMG = 7;
        public const int OTHER = 8;
        public const int LOCATIONS = 9;

        public const string LENGTH_LOC = "LengthLoc";
        public const string REST_LOC = "RestLoc";
        public const string EFFECT_LOC = "EffectLoc";
        public const string ADD_LOC = "AddItemLoc";
        public const string STATE_LOC = "MouseStateLoc";
        public const string OTHER_LOC = "OtherImageLoc";
        public const string LOC = "ResourceLoc";

        public const string DIVISION_TAG = "<>";
        public const string ENTRY_SPLIT = "_:_";

        private Dictionary<int, Dictionary<string, string>> master_dict;

        public SettingsReader(string file_loc)
        {
            master_dict = initEmptyMasterDict();
            readInFile(file_loc);
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

        public void readInFile(string file_loc)
        {
            string[] lines = File.ReadAllLines(file_loc);
            int curr_dict = DIMENSION;
            foreach (string line in lines)
            {
                if (line.Contains(DIVISION_TAG)) { curr_dict++; }
                else { readEntryIntoDictionary(curr_dict, line); }
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
            return (type == LENGTH_IMG || type == REST || type == EFFECT || type == ADDITEM || type == MOUSESTATE || type == OTHER_IMG);
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
    }
}
