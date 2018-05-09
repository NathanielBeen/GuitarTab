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
        private CommandSelections selections;
        private MouseSelections mouse_selections;
        private GuiCommandExecutor executor;
        private ChordPositionCheck chord_position;
        private MeasurePositionCheck measure_position;

        public GuiObjectFactory(VisualInfo v_info, CommandSelections sel, MouseSelections m_sel, GuiCommandExecutor ex, ChordPositionCheck cpos, MeasurePositionCheck mpos)
        {
            info = v_info;
            selections = sel;
            mouse_selections = m_sel;
            executor = ex;
            chord_position = cpos;
            measure_position = mpos;
        }

        public TreeNode buildPart(Part part)
        {
            var bounded = new PartBounds(part, info, new OrderedDelegate());
            var handler = new PartMouseHandler(bounded.Bounds, selections, mouse_selections, executor, part, measure_position, new OrderedDelegate());
            var drawing = new PartDrawingVisual(part, bounded.Bounds, info, new OrderedDelegate());

            return new TreeNode(part, bounded, handler, drawing);
        }

        public TreeNode buildMeasure(Measure measure)
        {
            var bounded = new MeasureBounds(measure, info, new OrderedDelegate());
            var handler = new MeasureMouseHandler(bounded.Bounds, selections, mouse_selections, executor, measure, chord_position, measure_position, new OrderedDelegate());
            var drawing = new MeasureDrawingVisual(measure, bounded.Bounds, info, new OrderedDelegate());

            return new TreeNode(measure, bounded, handler, drawing);
        }

        public TreeNode buildChord(Chord chord)
        {
            var bounded = new ChordBounds(chord, info, new UnorderedDelegate());
            var handler = new ChordMouseHandler(bounded.Bounds, selections, mouse_selections, executor, chord, chord_position, new UnorderedDelegate());
            TabDrawingVisual drawing;
            if (chord is NoteChord) { drawing = new ChordDrawingVisual(chord, bounded.ChordBar, bounded.Bounds, info, new UnorderedDelegate()); }
            else { drawing = new RestChordDrawingVisual(chord, bounded.ChordBar, bounded.Bounds, info, new UnorderedDelegate()); }

            return new TreeNode(chord, bounded, handler, drawing);
        }

        public TreeNode buildNote(Note note)
        {
            var bounded = new NoteBounds(note, info, new UnorderedDelegate());
            var handler = new NoteMouseHandler(bounded.Bounds, selections, mouse_selections, executor, note, new UnorderedDelegate());
            var drawing = new NoteDrawingVisual(note, bounded.Bounds, info, new UnorderedDelegate());

            return new TreeNode(note, bounded, handler, drawing);
        }

        public TreeNode buildEffectNode(IEffect effect, TreeNode parent)
        {
            if (parent is null || !(parent.BaseObject is Note)) { return null; }
            Note note = parent.BaseObject as Note;
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
            var handler = new EffectMouseHandler(bounded.Bounds, selections, mouse_selections, executor, new UnorderedDelegate());
            var drawing = new PalmMuteDrawingVisual(palm_mute, bounded.Bounds, info, new UnorderedDelegate());

            return new TreeNode(palm_mute, bounded, handler, drawing);
        }

        public TreeNode buildBend(Bend bend, Note note, VisualBounds bounds)
        {
            var bounded = new BendBounds(bend, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(bounded.Bounds, selections, mouse_selections, executor, new UnorderedDelegate());
            var drawing = new BendDrawingVisual(bend, bounded.Bounds, info, new UnorderedDelegate());

            return new TreeNode(bend, bounded, handler, drawing);
        }

        public TreeNode buildPinchHarmonic(PinchHarmonic pinch_harmonic, Note note, VisualBounds bounds)
        {
            var bounded = new PinchHarmonicBounds(pinch_harmonic, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(bounded.Bounds, selections, mouse_selections, executor, new UnorderedDelegate());
            var drawing = new PinchHarmonicDrawingVisual(pinch_harmonic, bounds, info, new UnorderedDelegate());

            return new TreeNode(pinch_harmonic, bounded, handler, drawing);
        }

        public TreeNode buildVibrato(Vibrato vibrato, Note note, VisualBounds bounds)
        {
            var bounded = new VibratoBounds(vibrato, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(bounded.Bounds, selections, mouse_selections, executor, new UnorderedDelegate());
            var drawing = new VibratoDrawingVisual(vibrato, bounds, info, new UnorderedDelegate());

            return new TreeNode(vibrato, bounded, handler, drawing);
        }

        public TreeNode buildSlide(Slide slide, Note note, VisualBounds bounds)
        {
            var bounded = new SlideBounds(slide, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(bounded.Bounds, selections, mouse_selections, executor, new UnorderedDelegate());
            var drawing = new SlideDrawingVisual(slide, bounds, info, new UnorderedDelegate());

            return new TreeNode(slide, bounded, handler, drawing);
        }

        public TreeNode buildHOPO(HOPO hopo, Note note, VisualBounds bounds)
        {
            var bounded = new HOPOBounds(hopo, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(bounded.Bounds, selections, mouse_selections, executor, new UnorderedDelegate());
            var drawing = new HOPODrawingVisual(hopo, bounds, info, new UnorderedDelegate());

            return new TreeNode(hopo, bounded, handler, drawing);
        }

        public TreeNode buildTie(Tie tie, Note note, VisualBounds bounds)
        {
            var bounded = new TieBounds(tie, note, bounds, info, new UnorderedDelegate());
            var handler = new EffectMouseHandler(bounded.Bounds, selections, mouse_selections, executor, new UnorderedDelegate());
            var drawing = new TieDrawingVisual(tie, bounds, info, new UnorderedDelegate());

            return new TreeNode(tie, bounded, handler, drawing);
        }
    }
}
