using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GuitarTab
{
    public class MouseSelectedView : BaseViewModel, IRecieveDimensionUpdates
    {
        public const int TOP_MARGIN = 20;

        public Brush Brush { get; }

        public ObservableCollection<UpdatingVisualBounds> Selected { get; set; }
        public List<IBounds> Backing { get; set; }

        private int scroll_height;
        private int current_left;

        public MouseSelectedView(Brush brush)
        {
            Brush = brush;
            Selected = new ObservableCollection<UpdatingVisualBounds>();
            Backing = new List<IBounds>();
        }

        public void setSelectedObjects(List<TreeNode> selected)
        {
            foreach (var item in Selected) { item.PropertyChanged -= ViewPropertyChanged; }
            Selected.Clear();
            Backing.Clear();
            foreach (TreeNode node in selected) { addBoundsToSelected(node.Bounds); }
        }

        public void addBoundsToSelected(IBounds bounds)
        {
            foreach (var bound in bounds?.getBoundsList())
            {
                var up_bound = new UpdatingVisualBounds(bound, current_left, TOP_MARGIN - scroll_height);
                up_bound.PropertyChanged += ViewPropertyChanged;

                Selected.Add(up_bound);
                Backing.Add(bound);
            }
        }

        public void handleDimensionUpdate(int new_val, DimensionType type)
        {
            if (type == DimensionType.PageWidth)
            {
                foreach (var bound in Selected) { bound.Left += (new_val - current_left); }
                current_left = new_val;
            }
            else if (type == DimensionType.ScrollAmount)
            {
                foreach (var bound in Selected) { bound.Top -= (new_val - scroll_height); }
                scroll_height = new_val;
            }
        }

        public void ViewPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            var handler = getHandler();
            handler?.Invoke(sender, args);
        }
    }
}
