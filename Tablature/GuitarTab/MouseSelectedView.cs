using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GuitarTab
{
    public class MouseSelectedView
    {
        public Brush Brush { get; }

        public ObservableCollection<VisualBounds> Selected { get; set; }

        public MouseSelectedView(Brush brush)
        {
            Brush = brush;
            Selected = new ObservableCollection<VisualBounds>();
        }

        public void setSelectedObjects(List<TreeNode> selected)
        {
            Selected.Clear();
            foreach (TreeNode node in selected) { addBoundsToSelected(node.ObjectBounds.Bounds); }
        }

        public void addBoundsToSelected(VisualBounds bounds)
        {
            if (bounds is MultipleVisualBounds)
            {
                foreach (VisualBounds bound in (bounds as MultipleVisualBounds).AllBounds)
                {
                    Selected.Add(bound);
                }
            }
            else { Selected.Add(bounds); }
        }
    }
}
