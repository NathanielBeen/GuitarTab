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

        public MouseSelectedView(Brush brush, MouseSelections selections)
        {
            Brush = brush;
            selections.SelectedChanged += handleSelectedChanged;
            Selected = new ObservableCollection<VisualBounds>();
        }

        public void handleSelectedChanged(object sender, SelectedObjectChangedArgs args)
        {
            Selected?.Clear();
            foreach (ModelBoundsPair new_selected in args.SelectedObjects)
            {
                if (new_selected != null) { Selected.Add(new_selected.Bounds); }

            }
        }
    }
}
