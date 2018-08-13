using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    class ViewingView : BaseViewModel
    {
        public ViewingCanvasView CanvasView { get; set; }

        public VisualsView VisualsView { get; }
        public TabScrollView ScrollView { get; }

        public ViewingView(ViewingCanvasView canvas, VisualsView visuals, TabScrollView scroll)
        {
            CanvasView = canvas;
            VisualsView = visuals;
            ScrollView = scroll;
        }
    }
}
