using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public delegate void MouseDelegate();

    public class MouseHandler
    {
        public const int MOVE_THRESHOLD = 10;
        public const int DOUBLE_CLICK_TIMER = 1000;

        private GuiCommandExecutor executor;
        private MouseSelections mouse_selections;
        private CommandSelections selections;
        private GuiObjectTree tree;

        public event EventHandler<PropertyMenuEventArgs> PropertyMenuChanged;

        private Point down_point;
        private bool double_click;

        public MouseHandler(GuiCommandExecutor ex, MouseSelections m_sel, CommandSelections sel, GuiObjectTree t)
        {
            executor = ex;
            mouse_selections = m_sel;
            selections = sel;
            tree = t;

            down_point = default(Point);
            double_click = false;
        }

        public void mouseDown(Point pos) { down_point = pos; }

        public void mouseUp(Point pos)
        {
            mouse_selections.EventHandled = false;
            mouse_selections.SelectedPoint = pos;
            if (mouseMoved(pos))
            {
                if (checkForDragRelease(pos, out bool multiple))
                {
                    mouse_selections.EventType = (multiple) ? MouseSelections.MULTIPLE_DRAG_RELEASE : MouseSelections.DRAG_RELEASE;
                    tree.HandleMouseEvent();
                    clearAllSelections();
                }
                else
                {
                    clearAllSelections();
                    mouse_selections.EventType = MouseSelections.DRAG_SELECT;
                    mouse_selections.SelectedRectangle = new Rect(pos, down_point);
                    tree.HandleMouseEvent();
                }
            }

            else if (double_click)
            {
                var args = new PropertyMenuEventArgs(mouse_selections.getFirstSelectedObject());
                PropertyMenuChanged?.Invoke(this, args);
            }

            else
            {
                if (mouse_selections.SelectionState != Selection.Add_Multi_Effect) { selections.Clear(); }

                mouse_selections.EventType = MouseSelections.CLICK;
                tree.HandleMouseEvent();

                if (mouse_selections.SelectionState == Selection.Standard)
                {
                    double_click = true;
                    TimeDoubleClick();
                }
            }

            mouse_selections.EventHandled = true;
        }

        public bool mouseMoved(Point up_point)
        {
            if (down_point == null) { return false; }
            return (Math.Abs(up_point.X - down_point.X) > MOVE_THRESHOLD || Math.Abs(up_point.Y - down_point.Y) > MOVE_THRESHOLD);
        }

        public bool checkForDragRelease(Point point, out bool multiple)
        {
            List<ModelBoundsPair> selected = mouse_selections.getAllSelectedObjects();
            multiple = (selected.Count() > 1);
            if (down_point == null) { return false; }

            foreach (var obj in selected)
            {
                if (obj.Bounds.containsPoint(down_point)) { return true; }
            }
            return false;
        }

        public async Task TimeDoubleClick()
        {
            await Task.Delay(DOUBLE_CLICK_TIMER);
            double_click = false;
        }

        public void clearAllSelections()
        {
            mouse_selections.clearAllSelections();
            selections.Clear();
        }
    }
}
