using System;
using System.Collections.Generic;
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
            get { return selections.SelectedLength.NoteType; }
            set { selections.SelectedLength = new Length(value); }
        }

        public Dictionary<NoteLength, ImageBrush> Images { get; set; }

        public LengthView(CommandSelections sel, Dictionary<string, string> image_uri)
        {
            selections = sel;
            Images = getImages(image_uri);
            Length = NoteLength.None;
        }

        public Dictionary<NoteLength, ImageBrush> getImages(Dictionary<string, string> uri_dict)
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
