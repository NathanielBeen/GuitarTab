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
    public class NoteBounds : BaseBounded
    {
        private Note note;
        private VisualInfo info;

        public NoteBounds(Note n, VisualInfo v_info, IDelegate del)
            :base(del)
        {
            note = n;
            info = v_info;
        }

        public Note getNote() { return note; }

        public override VisualBounds initBounds()
        {
            int width = info.Dimensions.NoteWidth;
            int height = info.Dimensions.NoteHeight;

            return new VisualBounds(0, 0, width, height, 0);
        }

        public override void updateBounds()
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
        public NoteMouseHandler(GuiCommandExecutor e, IMouseDelegate del) :base(e, del) { }

        public override void mouseClick(StandardClick click)
        {
            if (click.matchesSelectionType(Selection.Add_Multi_Effect) && click.multipleNotes())
            {
                executor.executeAddMultiEffectToNotes();
            }
            else if (click.matchesSelectionType(Selection.Add_Effect))
            {
                executor.executeAddEffectToNote();
            }

            invokeClickDelegate(click);
        }

        public override void mouseDragRelease(ReleaseClick click) { }
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
