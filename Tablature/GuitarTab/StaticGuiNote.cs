using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class NoteBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        protected Note note;
        protected VisualInfo info;

        public NoteBounds(Note n, VisualInfo v_info, IDelegate del)
        {
            BoundsDelegate = del;
            note = n;
            info = v_info;
        }

        public Note getNote() { return note; }

        public void updateBounds()
        {
            Bounds.Left = info.Position.X;
            Bounds.Top = info.Position.Y + info.Dimensions.EffectHeight + note.String * info.Dimensions.StringHeight;
            Bounds.Width = info.Dimensions.NoteWidth;
            Bounds.Height = info.Dimensions.NoteHeight;
            Bounds.Bar = info.Position.CurrentBar;

            BoundsDelegate?.invokeDelegate();
        }
    }

    public class StaticNoteMouseHandler : StaticMouseHandler
    {
        public StaticNoteMouseHandler(IBounds b, IMouseDelegate del) :base(del, b) { }

        public override void handleMouseEvent(MouseClick click)
        {
            invokeClickDelegate(click);
        }
    }
}
