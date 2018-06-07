using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class LengthView : BaseViewModel
    {
        private CommandSelections selections;

        public NoteLength Length
        {
            get { return selections.SelectedLength; }
            set { selections.SelectedLength = value; }
        }

        public TupletType TupletType
        {
            get { return selections.TupletType; }
            set { selections.TupletType = value; }
        }

        public Dictionary<NoteLength, ImageBrush> LengthImages { get; set; }
        public Dictionary<TupletType, ImageBrush> TupletImages { get; set; }

        public LengthView(CommandSelections sel, Dictionary<string, string> length_image_uri, Dictionary<string, string> tuplet_image_uri)
        {
            selections = sel;
            selections.PropertyChanged += handleLengthChanged;
            LengthImages = getLengthImages(length_image_uri);
            TupletImages = genTupletImages(tuplet_image_uri);

            Length = NoteLength.None;
            TupletType = TupletType.None;
        }

        public Dictionary<NoteLength, ImageBrush> getLengthImages(Dictionary<string, string> uri_dict)
        {
            var image_dict = new Dictionary<NoteLength, ImageBrush>();
            foreach (var entry in uri_dict)
            {
                NoteLength length = NoteLengthExtensions.getNoteLengthFromString(entry.Key);
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(entry.Value, UriKind.Relative));

                image_dict[length] = brush;
            }

            return image_dict;
        }

        public Dictionary<TupletType, ImageBrush> genTupletImages(Dictionary<string, string> uri_dict)
        {
            var image_dict = new Dictionary<TupletType, ImageBrush>();
            foreach (var entry in uri_dict)
            {
                TupletType type = TupletTypeExtensions.getTupletTypeFromString(entry.Key);
                var brush = new ImageBrush();
                brush.ImageSource = new BitmapImage(new Uri(entry.Value, UriKind.Relative));

                image_dict[type] = brush;
            }

            return image_dict;
        }

        public void handleLengthChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(CommandSelections.SelectedLength))
            {
                onPropertyChanged(nameof(Length));
            }
            else if (args.PropertyName == nameof(CommandSelections.TupletType))
            {
                onPropertyChanged(nameof(TupletType));
            }
        }
    }

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }

}
