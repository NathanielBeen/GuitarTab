using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    public class NoteBounds : IBounded
    {
        public IDelegate Delegate { get; set; }
        public VisualBounds Bounds { get; set; }
        private Note note;
        private VisualInfo info;

        public NoteBounds(Note n, VisualInfo v_info, IDelegate del)
        {
            Delegate = del;
            note = n;
            info = v_info;
            Bounds = genBounds();
        }

        public Note getNote() { return note; }

        public VisualBounds genBounds()
        {
            int width = info.Dimensions.NoteWidth;
            int height = info.Dimensions.NoteHeight;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public void updateBounds()
        {
            Bounds.Left = info.Position.X;
            Bounds.Top = info.Position.Y + info.Dimensions.EffectHeight + note.String * info.Dimensions.StringHeight;
            Bounds.Width = info.Dimensions.NoteWidth;
            Bounds.Height = info.Dimensions.NoteHeight;
            Bounds.Bar = info.Position.CurrentBar;

            Delegate?.invokeDelegate();
        }
    }

    public class NoteMouseHandler : BaseMouseHandler
    {
        private Note note;

        public NoteMouseHandler(VisualBounds b, CommandSelections s, MouseSelections ms, GuiCommandExecutor e, Note n, IDelegate del)
            :base(b, s, ms, e, del)
        {
            note = n;
        }

        public override void mouseClick()
        {
            addToCommandSelections();
            addToMouseSelections();

            if (mouse_selections.checkSelectionState(Selection.Add_Multi_Effect) && selections.SelectedNote.Count > 1)
            {
                executor.executeAddMultiEffectToNotes();
            }
            else if (mouse_selections.checkSelectionState(Selection.Add_Effect))
            {
                executor.executeAddEffectToNote();
            }

            invokeClickDelegate();
        }

        public override void mouseDragRelease() { }

        public override void mouseDragSelect()
        {
            addToCommandSelections();
            addToMouseSelections();
        }

        public override void addToCommandSelections() { selections.SelectedNote.Add(note); }

        public override void addToMouseSelections() { mouse_selections.setToSingleSelectedObject(new ModelBoundsPair(bounds, note)); }

        public override void mousePositionCheck() { }
    }

    public class NoteDrawingVisual : TabDrawingVisual
    {
        private Note note;

        public NoteDrawingVisual(Note n, VisualBounds bounds, VisualInfo v_info, IDelegate del)
            :base(bounds, v_info, del)
        {
            note = n;
            refreshVisual();
        }

        public override void refreshDrawingContext(DrawingContext dc)
        {
            var text = new FormattedText(note.Fret.ToString(), CultureInfo.GetCultureInfo("en-us"), FlowDirection.LeftToRight,
                                         info.DrawingObjects.TypeFace, info.Dimensions.FontSize, info.DrawingObjects.Brush);
            int x_val = (int)(info.Dimensions.NoteWidth - text.Width) / 2;
            int y_val = (int)(info.Dimensions.StringHeight - text.Height) / 2;

            dc.DrawRectangle(new SolidColorBrush(Colors.White), null, new Rect(x_val, y_val, text.Width, text.Height));
            dc.DrawText(text, new Point(x_val, y_val));
        }
    }
}
