using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IGuiObjectFactory
    {
        TreeNode buildObject(object pbj, TreeNode parent);
    }

    public class StaticGuiObjectFactory : IGuiObjectFactory
    {
        private VisualInfo info;

        public StaticGuiObjectFactory(VisualInfo v_info)
        {
            info = v_info;
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
            var bounded = new StaticSingleBounded(new PartBounds(part, info, new OrderedDelegate()));
            var handler = new StaticPartMouseHandler(bounded.Bounds, new OrderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new PartDrawingStrategy(part, bounded.Bounds, info, new OrderedDelegate()));

            return new PartTreeNode(part, bounded, handler, drawing);
        }

        private TreeNode buildMeasure(Measure measure)
        {
            var bounded = new StaticMultiBounded(new MeasureBounds(measure, info, new OrderedDelegate()));
            var handler = new StaticMeasureMouseHandler(bounded.Bounds, new OrderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new MeasureDrawingStrategy(measure, bounded.Bounds, info, new OrderedDelegate()));

            return new MeasureTreeNode(measure, bounded, handler, drawing);
        }

        private TreeNode buildChord(Chord chord)
        {
            var bounds_strat = new ChordBounds(chord, info, new UnorderedDelegate());
            var bounded = new StaticSingleBounded(bounds_strat);
            var handler = new StaticChordMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());

            ChordDrawingStrategy strategy;
            if (chord is NoteChord) { strategy = new ChordDrawingStrategy(chord, bounds_strat.ChordBar, bounds_strat.ChordTuple, bounded.Bounds, info, new UnorderedDelegate()); }
            else { strategy = new RestChordDrawingStrategy(chord, bounds_strat.ChordBar, bounds_strat.ChordTuple, bounded.Bounds, info, new UnorderedDelegate()); }
            var drawing = new StaticDrawingVisual(strategy);

            return new ChordTreeNode(chord, bounded, handler, drawing);
        }

        private TreeNode buildNote(Note note)
        {
            var bounded = new StaticSingleBounded(new NoteBounds(note, info, new UnorderedDelegate()));
            var handler = new StaticNoteMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new NoteDrawingStrategy(note, bounded.Bounds, info, new UnorderedDelegate()));

            return new NoteTreeNode(note, bounded, handler, drawing);
        }

        private TreeNode buildEffectNode(IEffect effect, NoteTreeNode parent)
        {
            if (parent is null) { return null; }
            Note note = parent.getNote();
            IBounds bounds = parent.Bounds;

            if (effect is PalmMute) { return buildPalmMute((PalmMute)effect, note, bounds); }
            if (effect is Bend) { return buildBend((Bend)effect, note, bounds); }
            if (effect is PinchHarmonic) { return buildPinchHarmonic((PinchHarmonic)effect, note, bounds); }
            if (effect is Vibrato) { return buildVibrato((Vibrato)effect, note, bounds); }
            if (effect is Slide) { return buildSlide((Slide)effect, note, bounds); }
            if (effect is HOPO) { return buildHOPO((HOPO)effect, note, bounds); }
            if (effect is Tie) { return buildTie((Tie)effect, note, bounds); }
            return null;
        }

        private TreeNode buildPalmMute(PalmMute palm_mute, Note note, IBounds bounds)
        {
            var bounded = new StaticSingleBounded(new PalmMuteBounds(palm_mute, note, bounds, info, new UnorderedDelegate()));
            var handler = new StaticEffectMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new PalmMuteDrawingStrategy(palm_mute, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(palm_mute, bounded, handler, drawing);
        }

        private TreeNode buildBend(Bend bend, Note note, IBounds bounds)
        {
            var bounded = new StaticSingleBounded(new BendBounds(bend, note, bounds, info, new UnorderedDelegate()));
            var handler = new StaticEffectMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new BendDrawingStrategy(bend, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(bend, bounded, handler, drawing);
        }

        private TreeNode buildPinchHarmonic(PinchHarmonic pinch_harmonic, Note note, IBounds bounds)
        {
            var bounded = new StaticSingleBounded(new PinchHarmonicBounds(pinch_harmonic, note, bounds, info, new UnorderedDelegate()));
            var handler = new StaticEffectMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new PinchHarmonicDrawingStrategy(pinch_harmonic, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(pinch_harmonic, bounded, handler, drawing);
        }

        private TreeNode buildVibrato(Vibrato vibrato, Note note, IBounds bounds)
        {
            var bounded = new StaticSingleBounded(new VibratoBounds(vibrato, note, bounds, info, new UnorderedDelegate()));
            var handler = new StaticEffectMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new VibratoDrawingStrategy(vibrato, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(vibrato, bounded, handler, drawing);
        }

        private TreeNode buildSlide(Slide slide, Note note, IBounds bounds)
        {
            var bounded = new StaticSingleBounded(new SlideBounds(slide, note, bounds, info, new UnorderedDelegate()));
            var handler = new StaticEffectMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new SlideDrawingStrategy(slide, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(slide, bounded, handler, drawing);
        }

        private TreeNode buildHOPO(HOPO hopo, Note note, IBounds bounds)
        {
            var bounded = new StaticSingleBounded(new HOPOBounds(hopo, note, bounds, info, new UnorderedDelegate()));
            var handler = new StaticEffectMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new HOPODrawingStrategy(hopo, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(hopo, bounded, handler, drawing);
        }

        private TreeNode buildTie(Tie tie, Note note, IBounds bounds)
        {
            var bounded = new StaticSingleBounded(new TieBounds(tie, note, bounds, info, new UnorderedDelegate()));
            var handler = new StaticEffectMouseHandler(bounded.Bounds, new UnorderedMouseDelegate());
            var drawing = new StaticDrawingVisual(new TieDrawingStrategy(tie, bounded.Bounds, info, new UnorderedDelegate()));

            return new EffectTreeNode(tie, bounded, handler, drawing);
        }
    }
}
