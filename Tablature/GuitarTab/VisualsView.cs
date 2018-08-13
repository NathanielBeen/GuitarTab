using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GuitarTab
{
    public class VisualsView : BaseViewModel, IRecieveDimensionUpdates
    {
        public ObservableCollection<DrawingVisual> Visuals { get; set; }

        private int height;
        public int Height
        {
            get { return height; }
            set { SetProperty(ref height, value); }
        }

        public VisualsView(ObservableCollection<DrawingVisual> visuals)
        {
            Visuals = visuals;
            Height = 0;
        }

        public void handleDimensionUpdate(int value, DimensionType type)
        {
            if (type == DimensionType.PageHeight) { Height = value; }
        }
    }
}
