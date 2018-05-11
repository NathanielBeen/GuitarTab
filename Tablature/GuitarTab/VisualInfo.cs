using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public class VisualDimensions
    {
        public const string PAGE_HEIGHT = "PageHeight";
        public const string PAGE_WIDTH = "PageWidth";
        public const string PAGE_HEAD_HEIGHT = "PageHeadHeight";
        public const string LINE_HEIGHT = "LineHeight";
        public const string BAR_HEIGHT = "BarHeight";
        public const string BAR_WIDTH = "BarWidth";
        public const string BAR_MARGIN = "BarMargin";
        public const string BARRING_HEIGHT = "BarringHeight";
        public const string BARRING_MARGIN = "BarringMargin";
        public const string EFFECT_HEIGHT = "EffectHeight";
        public const string EFFECT_MARGIN = "EffectMargin";
        public const string MEASURE_HEAD_WIDTH = "MeasureHeadWidth";
        public const string MEASURE_HEAD_MARGIN = "MeasureHeadMargin";
        public const string STRING_HEIGHT = "StringHeight";
        public const string NOTE_WIDTH = "NoteWidth";
        public const string NOTE_HEIGHT = "NoteHeight";
        public const string LINE_THICKNESS = "LineThickness";
        public const string DOT_SIZE = "DotSize";
        public const string DOT_MARGIN = "DotMargin";
        public const string SINGLE_BAR_LENGTH = "SingleBarLength";
        public const string BAR_SPACING = "BarSpacing";
        public const string FONT_SIZE = "FontSize";
        public const string LARGE_FONT_SIZE = "LargeFontSize";

        public int PageHeight { get; set; }
        public int PageWidth { get; set; }
        public int PageHeadHeight { get; set; }
        public int LineHeight { get; set; }
        public int BarHeight { get; set; }
        public int BarWidth { get; set; }
        public int BarMargin { get; set; }
        public int BarringHeight { get; set; }
        public int BarringMargin { get; set; }
        public int EffectHeight { get; set; }
        public int EffectMargin { get; set; }
        public int MeasureHeadWidth { get; set; }
        public int MeasureHeadMargin { get; set; }
        public int StringHeight { get; set; }
        public int NoteWidth { get; set; }
        public int NoteHeight { get; set; }
        public int LineThickness { get; set; }
        public int DotSize { get; set; }
        public int DotMargin { get; set; }
        public int SingleBarLength { get; set; }
        public int BarSpacing { get; set; }
        public int FontSize { get; set; }
        public int LargeFontSize { get; set; }

        private Dictionary<NoteLength, int> note_lengths;

        public VisualDimensions(Dictionary<string, string> dim_dict, Dictionary<string, string> note_dict)
        {
            //210,210,160,110,90,60,45,30,20,20,20
            readDimensionsFromFile(dim_dict);
            readNoteLengthsFromFile(note_dict);
        }

        public void readDimensionsFromFile(Dictionary<string, string> parsed_file)
        {
            PageHeight = int.Parse(parsed_file[PAGE_HEIGHT]);
            PageWidth = int.Parse(parsed_file[PAGE_WIDTH]);
            PageHeadHeight = int.Parse(parsed_file[PAGE_HEAD_HEIGHT]);
            LineHeight = int.Parse(parsed_file[LINE_HEIGHT]);
            BarHeight = int.Parse(parsed_file[BAR_HEIGHT]);
            BarWidth = int.Parse(parsed_file[BAR_WIDTH]);
            BarMargin = int.Parse(parsed_file[BAR_MARGIN]);
            BarringHeight = int.Parse(parsed_file[BARRING_HEIGHT]);
            BarringMargin = int.Parse(parsed_file[BARRING_MARGIN]);
            EffectHeight = int.Parse(parsed_file[EFFECT_HEIGHT]);
            EffectMargin = int.Parse(parsed_file[EFFECT_MARGIN]);
            MeasureHeadWidth = int.Parse(parsed_file[MEASURE_HEAD_WIDTH]);
            MeasureHeadMargin = int.Parse(parsed_file[MEASURE_HEAD_MARGIN]);
            StringHeight = int.Parse(parsed_file[STRING_HEIGHT]);
            NoteWidth = int.Parse(parsed_file[NOTE_WIDTH]);
            NoteHeight = int.Parse(parsed_file[NOTE_HEIGHT]);
            LineThickness = int.Parse(parsed_file[LINE_THICKNESS]);
            DotSize = int.Parse(parsed_file[DOT_SIZE]);
            DotMargin = int.Parse(parsed_file[DOT_MARGIN]);
            SingleBarLength = int.Parse(parsed_file[SINGLE_BAR_LENGTH]);
            BarSpacing = int.Parse(parsed_file[BAR_SPACING]);
            FontSize = int.Parse(parsed_file[FONT_SIZE]);
            LargeFontSize = int.Parse(parsed_file[LARGE_FONT_SIZE]);
        }

        public void readNoteLengthsFromFile(Dictionary<string, string> parsed_file)
        {
            var dict = new Dictionary<NoteLength, int>();
            foreach (var entry in parsed_file) { dict.Add(NoteLengthExtensions.getNoteLengthFromString(entry.Key), int.Parse(parsed_file[entry.Key])); }
            note_lengths = dict;
        }

        public int getLength(NoteLength length)
        {
            note_lengths.TryGetValue(length, out int val);
            return val;
        }
    }

    public class DrawingObjects
    {
        public Brush Brush { get; }
        public Pen Pen { get; }
        public Pen BarringPen { get; }
        public Pen DashPen { get; }
        public Typeface TypeFace { get; }

        public Brush SelectedBrush { get; }
        public Brush HoverBrush { get; }

        public DrawingObjects(int thickness, int bar_thickness, int dash_length, Color selected_color, Color hover_color)
        {
            Brush = createBrush(Colors.Black);
            Pen = createPen(thickness);
            BarringPen = createPen(bar_thickness);
            DashPen = createDashPen(thickness, dash_length);
            TypeFace = createTypeface();

            SelectedBrush = createBrush(selected_color);
            HoverBrush = createBrush(hover_color);
        }

        public Brush createBrush(Color color) { return new SolidColorBrush(color); }

        public Pen createPen(int thickness) { return new Pen(Brush, thickness); }

        public Pen createDashPen(int thickness, int dash_length)
        { 
            //add dashes later
            var dashes = new List<double>() { dash_length, dash_length / 2 };
            Pen pen = new Pen(Brush, thickness);
            return pen;
        }

        public Pen createColoredPen(int thickness, Color color)
        {
            var brush = new SolidColorBrush(color);
            return new Pen(brush, thickness);
        }

        public Typeface createTypeface()
        {
            return new Typeface(new FontFamily("Arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        }
    }

    public class TabImages
    {
        public const int VIBRATO = 0;
        public const int WIDE_VIBRATO = 1;
        public const int PINCH_HARMONIC = 2;

        private Dictionary<NoteLength, string> rest_image_paths;
        private Dictionary<int, string> effect_image_paths;

        public TabImages(Dictionary<string, string> rest_names, Dictionary<string, string> effect_names)
        {
            rest_image_paths = getRestImagePaths(rest_names);
            effect_image_paths = getEffectImagePaths(effect_names);
        }

        public Dictionary<NoteLength, string> getRestImagePaths( Dictionary<string, string> file_names)
        {
            var image_dict = new Dictionary<NoteLength, string>();
            foreach (string length_name in file_names.Keys)
            {
                NoteLength length = NoteLengthExtensions.getNoteLengthFromString(length_name);
                image_dict.Add(length, file_names[length_name]);
            }
            
            return image_dict;
        }

        public Dictionary<int, string> getEffectImagePaths(Dictionary<string, string> file_names)
        {
            var image_dict = new Dictionary<int, string>();;

            image_dict.Add(VIBRATO, file_names["Vibrato"]);
            image_dict.Add(WIDE_VIBRATO, file_names["WideVibrato"]);
            image_dict.Add(PINCH_HARMONIC, file_names["PinchHarmonic"]);
            return image_dict;
        }

        public Uri getRestImagePath(NoteLength length) { return new Uri(rest_image_paths[length], UriKind.Relative); }

        public Uri getEffectImagePath(int code) { return new Uri(effect_image_paths[code]); }
    }

    //this class does not fit in the visualinfo master class. it needs to be moved.
    public class CurrentPosition
    {
        public const int NUM_STRINGS = 6;
        public int X { get; set; }
        public int Y { get; set; }
        public int CurrentBar { get; set; }
        public int MaxBar { get; set; }

        private VisualDimensions dimensions;

        public CurrentPosition(VisualDimensions dim)
        {
            dimensions = dim;

            X = dimensions.BarMargin;
            Y = dimensions.PageHeadHeight;
            CurrentBar = 0;
        }

        public void incrementXPosition(int length)
        {
            X += length;
            if (X > dimensions.BarringMargin + dimensions.BarWidth - dimensions.NoteWidth * 2)
            {
                Y += dimensions.LineHeight;
                X = dimensions.BarMargin;
                CurrentBar++;
                if (CurrentBar > MaxBar) { MaxBar = CurrentBar; }
            }
        }

        public void incrementXPositionForMeasure(int length)
        {
            X += length;
            if (X > dimensions.BarringMargin + dimensions.BarWidth)
            {
                int remaining = X - dimensions.BarringMargin - dimensions.BarWidth;
                Y += dimensions.LineHeight;
                X = dimensions.BarMargin + remaining;
                CurrentBar++;
                if (CurrentBar > MaxBar) { MaxBar = CurrentBar; }
            }
        }

        public void jumpToNextBar()
        {
            Y += dimensions.LineHeight;
            X = dimensions.BarMargin;
            CurrentBar++;
            if (CurrentBar > MaxBar) { MaxBar = CurrentBar; }
        }

        public int truncateHorizontalLengthIfNeeded(int prop_length)
        {
            int available = dimensions.BarMargin + dimensions.BarWidth - X;
            if (prop_length > available) { return available; }
            else { return prop_length; }
        }

        public void resetPositionToPartBeginning(VisualBounds part_bounds)
        {
            X = part_bounds.Left + dimensions.BarMargin;
            Y = part_bounds.Top + dimensions.PageHeadHeight;
            CurrentBar = 0;
        }

        public void resetPositionToMeasureBeginning(VisualBounds measure_bounds)
        {
            X = measure_bounds?.Left + measure_bounds?.Width ?? dimensions.BarMargin;
            Y = (measure_bounds != null) ? measure_bounds.Top + measure_bounds.Height - dimensions.BarHeight - dimensions.EffectHeight : dimensions.PageHeadHeight;
            CurrentBar = measure_bounds?.Bar ?? 0;
        }

        public void resetPositionToChordBeginning(VisualBounds chord_bounds)
        {
            X = chord_bounds.Left;
            Y = chord_bounds.Top - dimensions.EffectHeight;
            CurrentBar = chord_bounds.Bar;
        }

        public int getStringFromYPosition(int y_val)
        {
            int adj_y_pos = ((y_val - dimensions.PageHeadHeight) % dimensions.LineHeight) - dimensions.EffectHeight;
            if (adj_y_pos < 0 || adj_y_pos > NUM_STRINGS * dimensions.StringHeight) { return 0; }

            return (int)Math.Floor((decimal)adj_y_pos / dimensions.StringHeight);
        }
    }

    public class VisualInfo
    {
        public VisualDimensions Dimensions { get; set; }
        public DrawingObjects DrawingObjects { get; set; }
        public TabImages Images { get; set; }
        public CurrentPosition Position { get; set; }

        public VisualInfo(VisualDimensions dimensions, DrawingObjects drawing, TabImages images, CurrentPosition position)
        {
            Dimensions = dimensions;
            DrawingObjects = drawing;
            Images = images;
            Position = position;
        }
    }
}
