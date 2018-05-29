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

        public MouseDragView DragView { get; }

        public MouseCanvasView(MouseStateView state, MouseHoverView hover, MouseSelectedView selected, MouseDragView drag, MouseHandler h)
        {
            StateView = state;
            HoverView = hover;
            SelectedView = selected;
            DragView = drag;
            handler = h;
        }

        public void handleMousePositionChanged(Point position)
        {
            StateView.changePosition(position);
            HoverView.setHoveredObject(handler.hoverCheck(position));
            DragView.updateDragRect(position);
        }

        public void handleMouseUp(Point position)
        {
            DragView.clearDrag();
            handler?.mouseUp(position);
        }

        public void handleMouseDown(Point position)
        {
            DragView.setDownPoint(position, SelectedView.Selected.ToList());
            handler?.mouseDown(position);
        }
    }
}
