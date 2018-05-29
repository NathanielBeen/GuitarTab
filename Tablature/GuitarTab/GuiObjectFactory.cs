using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{ 
    public class GuiObjectFactory
    {
        private VisualInfo info;
        private GuiCommandExecutor executor;

        public GuiObjectFactory(VisualInfo v_info, GuiCommandExecutor ex)
        {
            info = v_info;
            executor = ex;
        }

        public TreeNode buildPart(Part part)
        {
            var bounded = new PartBounds(part, info, new OrderedDelegate());
            var handler = new PartMouseHandler(executor, new OrderedMouseDelegate());
            var drawing = new PartDrawingVisual(part, bounded.Bounds, info, new OrderedDelegate());

            return new PartTreeNode(part, bounded, handler, drawing);
        }

        public TreeNode buildMeasure(Measure measure)
        {
            var bounded = new MeasureBounds(measure, info, new OrderedDelegate());
            var handler = new MeasureMouseHandler(executor, new OrderedMouseDelegate());
            var drawing = new MeasureDrawingVisual(measure, bounded.Bounds, info, new OrderedDelegate());

            return new MeasureTreeNode(measure, bounded, handler, drawing);
        }

        public TreeNode buildChord(Chord chord)
        {
            var bounded = new ChordBounds(chord, info, new UnorderedDelegate());
            var handler = new ChordMouseHandler(chord, executor, new UnorderedMouseDelegate());
            ChordDrawingVisual drawing;
            if (chord is NoteChord) { drawing = new ChordDrawingVisual(chord, bounded.ChordBar, bounded.Bounds, info, new UnorderedDelegate()); }
            else { drawing = new RestChordDrawingVisual(chord, bounded.ChordBar, bounded.Bounds, info, new UnorderedDelegate()); }

            return new ChordTreeNode(chord, bounded, handler, drawing);
        }

        public TreeNode buildNote(Note note)
        {
            var bounded = new NoteBounds(note, info, new UnorderedDelegate());
            var handler = new NoteMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new NoteDrawingVisual(note, bounded.Bounds, info, new UnorderedDelegate());

            return new NoteTreeNode(note, bounded, handler, drawing);
        }

        public TreeNode buildEffectNode(IEffect effect, NoteTreeNode parent)
        {
            if (parent is null) { return null; }
            Note note = parent.getNote();
            VisualBounds bounds = parent.ObjectBounds.Bounds;

            if (effect is PalmMute) { return buildPalmMute((PalmMute)effect, note, bounds); }
            if (effect is Bend) { return buildBend((Bend)effect, note, bounds); }
            if (effect is PinchHarmonic) { return buildPinchHarmonic((PinchHarmonic)effect, note, bounds); }
            if (effect is Vibrato) { return buildVibrato((Vibrato)effect, note, bounds); }
            if (effect is Slide) { return buildSlide((Slide)effect, note, bounds); }
            if (effect is HOPO) { return buildHOPO((HOPO)effect, note, bounds); }
            if (effect is Tie) { return buildTie((Tie)effect, note, bounds); }
            return null;
        }

        public TreeNode buildPalmMute(PalmMute palm_mute, Note note, VisualBounds bounds)
        {
            var bounded = new PalmMuteBounds(palm_mute, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new PalmMuteDrawingVisual(palm_mute, bounded.Bounds, info, new UnorderedDelegate());

            return new EffectTreeNode(palm_mute, bounded, handler, drawing);
        }

        public TreeNode buildBend(Bend bend, Note note, VisualBounds bounds)
        {
            var bounded = new BendBounds(bend, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new BendDrawingVisual(bend, bounded.Bounds, info, new UnorderedDelegate());

            return new EffectTreeNode(bend, bounded, handler, drawing);
        }

        public TreeNode buildPinchHarmonic(PinchHarmonic pinch_harmonic, Note note, VisualBounds bounds)
        {
            var bounded = new PinchHarmonicBounds(pinch_harmonic, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new PinchHarmonicDrawingVisual(pinch_harmonic, bounded.Bounds, info, new UnorderedDelegate());

            return new EffectTreeNode(pinch_harmonic, bounded, handler, drawing);
        }

        public TreeNode buildVibrato(Vibrato vibrato, Note note, VisualBounds bounds)
        {
            var bounded = new VibratoBounds(vibrato, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new VibratoDrawingVisual(vibrato, bounded.Bounds, info, new UnorderedDelegate());

            return new EffectTreeNode(vibrato, bounded, handler, drawing);
        }

        public TreeNode buildSlide(Slide slide, Note note, VisualBounds bounds)
        {
            var bounded = new SlideBounds(slide, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new SlideDrawingVisual(slide, bounded.Bounds, info, new UnorderedDelegate());

            return new EffectTreeNode(slide, bounded, handler, drawing);
        }

        public TreeNode buildHOPO(HOPO hopo, Note note, VisualBounds bounds)
        {
            var bounded = new HOPOBounds(hopo, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new HOPODrawingVisual(hopo, bounded.Bounds, info, new UnorderedDelegate());

            return new EffectTreeNode(hopo, bounded, handler, drawing);
        }

        public TreeNode buildTie(Tie tie, Note note, VisualBounds bounds)
        {
            var bounded = new TieBounds(tie, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(executor, new UnorderedMouseDelegate());
            var drawing = new TieDrawingVisual(tie, bounded.Bounds, info, new UnorderedDelegate());

            return new EffectTreeNode(tie, bounded, handler, drawing);
        }
    }
}
