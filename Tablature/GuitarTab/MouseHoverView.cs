using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public class MouseHoverView : BaseViewModel
    {
        public Brush Brush { get; }

        private IBounds bounds;
        public ObservableCollection<IBounds> Hovered { get; private set; }

        public MouseHoverView(Brush brush)
        {
            Brush = brush;
            Hovered = new ObservableCollection<IBounds>();
        }

        public void setHoveredObject(IBounds hover, int scroll_height, int current_left)
        {
            if (bounds?.Equals(hover) ?? false) { return; }
            bounds = hover;
            List<IBounds> new_hover = createHoverList(hover, scroll_height, current_left);
            Hovered.Clear();
            foreach (IBounds bound in new_hover) { Hovered.Add(bound); }
        }

        public List<IBounds> createHoverList(IBounds hover, int scroll_height, int current_left)
        {
            var list = new List<IBounds>();
            if (hover == null) { return list; }
            foreach (var bound in hover.getBoundsList()) { list.Add(createCorrectedBounds(bound, scroll_height, current_left)); }
            return list;
        }

        public IBounds createCorrectedBounds(IBounds bounds, int scroll_height, int current_left)
        {
            return new SingleBounds(bounds.Left + current_left, bounds.Top - scroll_height, bounds.Width, bounds.Height, bounds.Bar);
        }
    }
}
