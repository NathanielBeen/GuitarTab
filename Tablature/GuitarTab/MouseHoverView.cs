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

        private VisualBounds bounds;
        public ObservableCollection<VisualBounds> Hovered { get; private set; }

        public MouseHoverView(Brush brush)
        {
            Brush = brush;
            Hovered = new ObservableCollection<VisualBounds>();
        }

        public void setHoveredObject(VisualBounds hover)
        {
            if (bounds?.Equals(hover) ?? false) { return; }
            bounds = hover;
            List<VisualBounds> new_hover = createHoverList(hover);
            Hovered.Clear();
            foreach (VisualBounds bound in new_hover) { Hovered.Add(bounds); }
        }

        public List<VisualBounds> createHoverList(VisualBounds hover)
        {
            var list = new List<VisualBounds>();
            if (hover is MultipleVisualBounds) { list.AddRange(((MultipleVisualBounds)hover).AllBounds); }
            else { list.Add(hover); }
            return list;
        }
    }
}
