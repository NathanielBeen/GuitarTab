using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class ChordBounds : IChordBounds
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        public ChordBar ChordBar { get; set; }
        public ChordTuple ChordTuple { get; set; }

        protected Chord chord;
        protected VisualInfo info;

        public ChordBounds(Chord c, VisualInfo v_info, IDelegate del)
        {
            ChordBar = new ChordBar(c.Length.NoteType);
            ChordTuple = new ChordTuple(c.Length);

            BoundsDelegate = del;
            chord = c;
            info = v_info;
        }

        public void updateBounds()
        {
            Bounds.Left = info.Position.X;
            Bounds.Top = info.Position.Y + info.Dimensions.EffectHeight;
            Bounds.Width = info.Dimensions.NoteWidth;
            Bounds.Height = info.Dimensions.BarHeight;
            Bounds.Bar = info.Position.CurrentBar;

            BoundsDelegate?.invokeDelegate();

            info.Position.incrementXPosition(info.Dimensions.NoteWidth + info.Dimensions.getLength(chord.Length.NoteType));
        }
        public Chord getChord() { return chord; }
    }

    public class StaticChordMouseHandler : StaticMouseHandler
    {
        public StaticChordMouseHandler(IBounds b, IMouseDelegate del) :base(del, b) { }

        public override void handleMouseEvent(MouseClick click)
        {
            invokeClickDelegate(click);
        }
    }
}
