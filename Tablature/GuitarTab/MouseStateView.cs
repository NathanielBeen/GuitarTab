using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GuitarTab
{
    public class MouseStateView : BaseViewModel
    {
        private MouseStateConverter converter;
        private Dictionary<Selection, BitmapImage> mouse_images;

        private ImageBrush current_image;
        public ImageBrush CurrentImage
        {
            get { return current_image; }
            set { SetProperty(ref current_image, value); }
        }

        private int top, left;
        public int Top
        {
            get { return top; }
            set { SetProperty(ref top, value); }
        }

        public int Left
        {
            get { return left; }
            set { SetProperty(ref left, value); }
        }

        public MouseStateView(Dictionary<string, string> uris, MouseStateConverter conv)
            : base()
        {
            mouse_images = getImages(uris);
            converter = conv;
            converter.PropertyChanged += handleStateChanged;
        }

        public Dictionary<Selection, BitmapImage> getImages(Dictionary<string, string> uris)
        {
            var dict = new Dictionary<Selection, BitmapImage>();
            foreach (var entry in uris)
            {
                Selection state = (Selection)int.Parse(entry.Key);
                var image = new BitmapImage(new Uri(entry.Value, UriKind.Relative));
                dict.Add(state, image);
            }
            return dict;
        }

        public void handleStateChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == nameof(MouseSelections.SelectionState))
            {
                CurrentImage = new ImageBrush(mouse_images[converter.SelectionState]);
            }
        }

        public void changePosition(Point new_pos)
        {
            Left = (int)new_pos.X;
            Top = (int)new_pos.Y;
        }
    }
}
