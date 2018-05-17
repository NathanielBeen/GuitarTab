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

        private GuiObjectTree tree;
        private Selected selected;
        private MouseStateConverter converter;

        public event EventHandler<PropertyMenuEventArgs> PropertyMenuChanged;

        public Selection SelectionState
        {
            get { return converter.SelectionState; }
            set { converter.SelectionState = value; }
        }

        private Point down_point;
        private bool double_click;

        public MouseHandler(GuiObjectTree t, Selected s, MouseStateConverter conv)
        {
            tree = t;
            selected = s;
            converter = conv;

            down_point = default(Point);
            double_click = false;
        }

        public void mouseDown(Point pos) { down_point = pos; }

        public void mouseUp(Point pos)
        {
            if (mouseMoved(pos))
            {
                NodeClick click;
                if (checkForDragRelease(pos))
                {
                    click = new ReleaseClick(pos);
                    selected.populateNodeClick(click);
                }
                else
                {
                    click = new SelectClick(pos, new Rect(pos, down_point));
                }
                tree.HandleMouseEvent(click);
                selected.populateFromClick(click);
            }

            else if (double_click)
            {
                var click = new NodeClick(pos);
                selected.populateNodeClick(click);

                var args = new PropertyMenuEventArgs(click);
                PropertyMenuChanged?.Invoke(this, args);
            }

            else
            {
                var click = new StandardClick(SelectionState, pos);
                if (SelectionState == Selection.Add_Multi_Effect) { selected.populateNodeClickForMultiEffect(click); }

                tree.HandleMouseEvent(click);
                selected.populateFromClick(click);

                if (SelectionState == Selection.Standard) { TimeDoubleClick(); }
            }
        }

        public VisualBounds hoverCheck(Point point)
        {
            var click = new BoundsClick(point);
            tree.HandleMouseEvent(click);
            return click.DeepestBounds;
        }

        public bool mouseMoved(Point up_point)
        {
            if (down_point == null) { return false; }
            return (Math.Abs(up_point.X - down_point.X) > MOVE_THRESHOLD || Math.Abs(up_point.Y - down_point.Y) > MOVE_THRESHOLD);
        }

        public bool checkForDragRelease(Point point)
        {
            List<VisualBounds> selected_bounds = selected.getSelected();
            if (down_point == null) { return false; }

            foreach (var bound in selected_bounds)
            {
                if (bound.containsPoint(down_point)) { return true; }
            }
            return false;
        }

        public async Task TimeDoubleClick()
        {
            double_click = true;
            await Task.Delay(DOUBLE_CLICK_TIMER);
            double_click = false;
        }
    }
}
