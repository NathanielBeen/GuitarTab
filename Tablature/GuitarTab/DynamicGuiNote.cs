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
    public class DynamicNoteMouseHandler : DynamicMouseHandler
    {
        public DynamicNoteMouseHandler(IBounds b, GuiCommandExecutor e, IMouseDelegate del) :base(e, del, b) { }

        public override void mouseClick(StandardClick click)
        {
            if (click.matchesSelectionType(Selection.Add_Multi_Effect))
            {
                executor.executeAddMultiEffectToNotesMenu(click);
            }
            else if (click.matchesSelectionType(Selection.Add_Effect))
            {
                executor.executeAddEffectToNote(click);
            }

            invokeClickDelegate(click);
        }

        public override void mouseDragRelease(ReleaseClick click) { }
    }

    public class NoteDrawingStrategy : IDrawStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate DrawDelegate { get; set; }

        private VisualInfo info;
        private Note note;

        public NoteDrawingStrategy(Note n, IBounds bounds, VisualInfo v_info, IDelegate del)
        {
            Bounds = bounds;
            DrawDelegate = del;

            info = v_info;
            note = n;
        }

        public void refreshDrawingContext(DrawingContext dc)
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
