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

    public interface IMouseHandler
    {
        void mouseDown(Point point);
        void mouseUp(Point point);
        IBounds hoverCheck(Point point);
    }

    public class ViewMouseHandler : IMouseHandler
    {
        private GuiObjectTree tree;

        public ViewMouseHandler(GuiObjectTree t)
        {
            tree = t;
        }

        public void mouseUp(Point point) { }
        public void mouseDown(Point point) { }
        public IBounds hoverCheck(Point point) { return null; }
    }

    public class EditingMouseHandler : IMouseHandler
    {
        public const int MOVE_THRESHOLD = 10;
        public const int DOUBLE_CLICK_TIMER = 1000;

        private GuiObjectTree tree;
        private Selected selected;
        private MouseStateConverter converter;

        
        public event EventHandler<PropertyMenuEventArgs> PropertyMenuChanged;
        public event EventHandler<NoteSelectLaunchEventArgs> NoteSelectLaunched;
        public event EventHandler<NoteSelectEndEventArgs> NoteSelectEnd;
        

        public Selection SelectionState
        {
            get { return converter.SelectionState; }
            set { converter.SelectionState = value; }
        }

        private bool note_select;
        private Point down_point;
        private bool double_click;

        public EditingMouseHandler(GuiObjectTree t, Selected s, MouseStateConverter conv)
        {
            tree = t;
            selected = s;
            converter = conv;

            note_select = false;
            down_point = default(Point);
            double_click = false;
        }

        public void mouseDown(Point pos) { down_point = pos; }

        public void mouseUp(Point pos)
        {
            if (note_select)
            {
                var click = new NoteSelectClick(pos);
                tree.HandleMouseEvent(click);
                populateSelected(click);
                noteSelected(click);
            }

            else if (checkMouseMoved(pos))
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
                populateSelected(click);
            }

            else if (double_click && selected.selectedContainsPoint(pos))
            {
                var click = new NodeClick(pos);
                selected.populateNodeClick(click);

                var args = new PropertyMenuEventArgs(click);
                PropertyMenuChanged?.Invoke(this, args);
            }

            else
            {
                var click = new StandardClick(SelectionState, pos);

                tree.HandleMouseEvent(click);
                if (!click.Handled) { populateSelected(click); }

                if (SelectionState == Selection.Standard) { TimeDoubleClick(); }
            }
        }

        public IBounds hoverCheck(Point point)
        {
            var click = new BoundsClick(point);
            tree.HandleMouseEvent(click);
            return click.DeepestBounds;
        }

        public void handleNoteSelect(object sender, NoteSelectLaunchEventArgs args)
        {
            note_select = true;
            NoteSelectLaunched?.Invoke(this, args);
        }

        private void populateSelected(NodeClick click) { selected.populateFromClick(click); }

        private bool checkMouseMoved(Point up_point)
        {
            if (down_point == null) { return false; }
            return (Math.Abs(up_point.X - down_point.X) > MOVE_THRESHOLD || Math.Abs(up_point.Y - down_point.Y) > MOVE_THRESHOLD);
        }

        private bool checkForDragRelease(Point point)
        {
            List<IBounds> selected_bounds = selected.getSelected();
            if (down_point == null) { return false; }

            foreach (var bound in selected_bounds)
            {
                if (bound.containsPoint(down_point)) { return true; }
            }
            return false;
        }

        private async Task TimeDoubleClick()
        {
            double_click = true;
            await Task.Delay(DOUBLE_CLICK_TIMER);
            double_click = false;
        }

        private void noteSelected(NodeClick click)
        {
            note_select = false;
            NoteSelectEnd?.Invoke(this, new NoteSelectEndEventArgs(click));
        }
    }
}
