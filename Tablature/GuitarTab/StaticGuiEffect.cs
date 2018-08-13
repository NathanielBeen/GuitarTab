using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class StaticEffectMouseHandler : StaticMouseHandler
    {
        public StaticEffectMouseHandler(IBounds b, IMouseDelegate del) :base(del, b) { }
    }

    public class PalmMuteBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        private PalmMute palm_mute;
        private Note note;
        private IBounds note_bounds;
        private VisualInfo info;

        public PalmMuteBounds(PalmMute pm, Note n, IBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            palm_mute = pm;
            note = n;
            note_bounds = n_bounds;
            info = v_info;
        }

        public void updateBounds() { GuiEffectBoundDrawing.updateAboveLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class BendBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        private Bend bend;
        private Note note;
        private IBounds note_bounds;
        private VisualInfo info;

        public BendBounds(Bend b, Note n, IBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            bend = b;
            note = n;
            note_bounds = n_bounds;
            info = v_info;
        }

        public void updateBounds() { GuiEffectBoundDrawing.updateInLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class PinchHarmonicBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        private PinchHarmonic pinch_harmonic;
        private Note note;
        private IBounds note_bounds;
        private VisualInfo info;

        public PinchHarmonicBounds(PinchHarmonic ph, Note n, IBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            pinch_harmonic = ph;
            note = n;
            note_bounds = n_bounds;
            info = v_info;
        }

        public void updateBounds() { GuiEffectBoundDrawing.updateBeforeNoteEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class VibratoBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        private Vibrato vibrato;
        private Note note;
        private IBounds note_bounds;
        private VisualInfo info;

        public VibratoBounds(Vibrato v, Note n, IBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            vibrato = v;
            note = n;
            note_bounds = n_bounds;
            info = v_info;
        }

        public void updateBounds() { GuiEffectBoundDrawing.updateAboveLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class SlideBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        private Slide slide;
        private Note note;
        private IBounds note_bounds;
        private VisualInfo info;

        public SlideBounds(Slide s, Note n, IBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            slide = s;
            note = n;
            note_bounds = n_bounds;
            info = v_info;
        }

        public void updateBounds() { GuiEffectBoundDrawing.updateHalvedInLineEffectBounds(Bounds, note_bounds, note, info); }
    }

    public class HOPOBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        private HOPO hopo;
        private Note note;
        private IBounds note_bounds;
        private VisualInfo info;

        public HOPOBounds(HOPO h, Note n, IBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            hopo = h;
            note = n;
            note_bounds = n_bounds;
            info = v_info;
        }

        public void updateBounds() { GuiEffectBoundDrawing.updateAboveNoteEffectBounds(Bounds, note_bounds, note, info); }

    }

    public class TieBounds : IBoundedStrategy
    {
        public IBounds Bounds { get; set; }
        public IDelegate BoundsDelegate { get; set; }

        private Tie tie;
        private Note note;
        private IBounds note_bounds;
        private VisualInfo info;

        public TieBounds(Tie t, Note n, IBounds n_bounds, VisualInfo v_info, IDelegate del)
        {
            tie = t;
            note = n;
            note_bounds = n_bounds;
            info = v_info;
        }

        public void updateBounds() { GuiEffectBoundDrawing.updateBelowLineEffectBounds(Bounds, note_bounds, note, info); }
    }
}
