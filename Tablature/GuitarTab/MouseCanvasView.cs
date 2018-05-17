using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public class MouseCanvasView
    {
        private MouseHandler handler;

        public MouseStateView StateView { get; }

        public MouseHoverView HoverView { get; }

        public MouseSelectedView SelectedView { get; }

        public MouseCanvasView(MouseStateView state, MouseHoverView hover, MouseSelectedView selected, MouseHandler h)
        {
            StateView = state;
            HoverView = hover;
            SelectedView = selected;
            handler = h;
        }

        public void handleMousePositionChanged(Point position)
        {
            StateView.changePosition(position);
            HoverView.setHoveredObject(handler.hoverCheck(position));
        }

        public void handleMouseUp(Point position)
        {
            handler?.mouseUp(position);
        }

        public void handleMouseDown(Point position)
        {
            handler?.mouseDown(position);
        }
    }
}
