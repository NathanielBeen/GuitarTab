using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{ 
    public class DynamicGuiObjectFactory : IGuiObjectFactory
    {
        private VisualInfo info;
        private GuiCommandExecutor executor;

        public DynamicGuiObjectFactory(VisualInfo v_info, GuiCommandExecutor ex)
        {
            info = v_info;
            executor = ex;
        }

        public TreeNode buildObject(object obj, TreeNode parent)
        {
            if (obj is Part) { return buildPart(obj as Part); }
            else if (obj is Measure) { return buildMeasure(obj as Measure); }
            else if (obj is Chord) { return buildChord(obj as Chord); }
            else if (obj is Note) { return buildNote(obj as Note); }
            else if (obj is IEffect && parent is NoteTreeNode) { return buildEffectNode(obj as IEffect, parent as NoteTreeNode); }
            else { return null; }
        }

        private TreeNode buildPart(Part part)
        {
            var bounded = new DynamicSingleBounded( new DynamicPartBounds(part, info, new OrderedDelegate()));
            var handler = new DynamicPartMouseHandler(bounded.Bounds, executor, new OrderedMouseDelegate());
            var drawing = new DynamicDrawingVisual( new PartDrawingStrategy(part, bounded.Bounds, info, new OrderedDelegate()));

            return new PartTreeNode(part, bounded, handler, drawing);
        }

        private TreeNode buildMeasure(Measure measure)
        {
            var bounded = new DynamicMultiBounded( new MeasureBounds(measure, info, new OrderedDelegate()));
            var handler = new DynamicMeasureMouseHandler(measure, bounded.Bounds as DynamicBounds, executor, new OrderedMouseDelegate());
            var drawing = new DynamicMeasureDrawingVisual( new MeasureDrawingStrategy(measure, bounded.Bounds, info, new OrderedDelegate()));

            return new MeasureTreeNode(measure, bounded, handler, drawing);
        }

        private TreeNode buildChord(Chord chord)
        {
            var bounds_strat = new ChordBounds(chord, info, new UnorderedDelegate());
            var bounded = new DynamicSingleBounded(bounds_strat);
            var handler = new DynamicChordMouseHandler(chord, bounded.Bounds, executor, new UnorderedMouseDelegate());
            ChordDrawingStrategy strategy;
            if (chord is NoteChord) { strategy = new ChordDrawingStrategy(chord, bounds_strat.ChordBar, bounds_strat.ChordTuple, bounded.Bounds, info, new UnorderedDelegate()); }
            else { strategy = new RestChordDrawingStrategy(chord, bounds_strat.ChordBar, bounds_strat.ChordTuple, bounded.Bounds, info, new UnorderedDelegate()); }
            var drawing = new DynamicDrawingVisual(strategy);

            return new ChordTreeNode(chord, bounded, handler, drawing);
        }

        private TreeNode buildNote(Note note)
        {
            var bounded = new DynamicSingleBounded( new NoteBounds(note, info, new UnorderedDelegate()));
            var handler = new DynamicNoteMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual( new NoteDrawingStrategy(note, bounded.Bounds, info, new UnorderedDelegate()));

            return new NoteTreeNode(note, bounded, handler, drawing);
        }

        private TreeNode buildEffectNode(IEffect effect, NoteTreeNode parent)
        {
            if (parent is null) { return null; }
            Note note = parent.getNote();
            DynamicBounds bounds = parent.Bounds as DynamicBounds;

            if (effect is PalmMute) { return buildPalmMute((PalmMute)effect, note, bounds); }
            if (effect is Bend) { return buildBend((Bend)effect, note, bounds); }
            if (effect is PinchHarmonic) { return buildPinchHarmonic((PinchHarmonic)effect, note, bounds); }
            if (effect is Vibrato) { return buildVibrato((Vibrato)effect, note, bounds); }
            if (effect is Slide) { return buildSlide((Slide)effect, note, bounds); }
            if (effect is HOPO) { return buildHOPO((HOPO)effect, note, bounds); }
            if (effect is Tie) { return buildTie((Tie)effect, note, bounds); }
            return null;
        }

        private TreeNode buildPalmMute(PalmMute palm_mute, Note note, DynamicBounds bounds)
        {
            var bounded = new DynamicSingleBounded( new PalmMuteBounds(palm_mute, note, bounds, info, new UnorderedDelegate()));
            var handler = new DynamicEffectMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual(new PalmMuteDrawingStrategy(palm_mute, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(palm_mute, bounded, handler, drawing);
        }

        private TreeNode buildBend(Bend bend, Note note, DynamicBounds bounds)
        {
            var bounded = new DynamicSingleBounded( new BendBounds(bend, note, bounds, info, new UnorderedDelegate()));
            var handler = new DynamicEffectMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual(new BendDrawingStrategy(bend, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(bend, bounded, handler, drawing);
        }

        private TreeNode buildPinchHarmonic(PinchHarmonic pinch_harmonic, Note note, DynamicBounds bounds)
        {
            var bounded = new DynamicSingleBounded( new PinchHarmonicBounds(pinch_harmonic, note, bounds, info, new UnorderedDelegate()));
            var handler = new DynamicEffectMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual(new PinchHarmonicDrawingStrategy(pinch_harmonic, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(pinch_harmonic, bounded, handler, drawing);
        }

        private TreeNode buildVibrato(Vibrato vibrato, Note note, DynamicBounds bounds)
        {
            var bounded = new DynamicSingleBounded( new VibratoBounds(vibrato, note, bounds, info, new UnorderedDelegate()));
            var handler = new DynamicEffectMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual(new VibratoDrawingStrategy(vibrato, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(vibrato, bounded, handler, drawing);
        }

        private TreeNode buildSlide(Slide slide, Note note, DynamicBounds bounds)
        {
            var bounded = new DynamicSingleBounded( new SlideBounds(slide, note, bounds, info, new UnorderedDelegate()));
            var handler = new DynamicEffectMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual(new SlideDrawingStrategy(slide, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(slide, bounded, handler, drawing);
        }

        private TreeNode buildHOPO(HOPO hopo, Note note, DynamicBounds bounds)
        {
            var bounded = new DynamicSingleBounded( new HOPOBounds(hopo, note, bounds, info, new UnorderedDelegate()));
            var handler = new DynamicEffectMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual(new HOPODrawingStrategy(hopo, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(hopo, bounded, handler, drawing);
        }

        private TreeNode buildTie(Tie tie, Note note, DynamicBounds bounds)
        {
            var bounded = new DynamicSingleBounded( new TieBounds(tie, note, bounds, info, new UnorderedDelegate()));
            var handler = new DynamicEffectMouseHandler(bounded.Bounds, executor, new UnorderedMouseDelegate());
            var drawing = new DynamicDrawingVisual(new TieDrawingStrategy(tie, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(tie, bounded, handler, drawing);
        }
    }
}
