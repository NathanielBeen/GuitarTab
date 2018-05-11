using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class GuiTreeUpdater
    {
        private GuiObjectTree tree;
        private GuiObjectFactory factory;
        private CurrentPosition position;

        public GuiTreeUpdater(GuiObjectTree t, GuiObjectFactory fac, CurrentPosition curr)
        {
            tree = t;
            factory = fac;
            position = curr;
        }

        public void setTreePart(Part part)
        {
            tree.buildTree(part);
        }

        public void updatePartBounds(Part part)
        {
            IBounded part_bounds = tree.findPart(part)?.ObjectBounds;
            if (part_bounds is null) { return; }

            TreeNode part_node = tree.findPart(part);
            part_node?.ObjectBounds.updateBounds();
        }

        public void updateMeasureBoundsAtAndAfter(Part part, Measure measure)
        {
            int pos = measure.Position.Index;
            Measure prev_measure = part.ModelCollection.getItemMatchingCondition(x => x.Position.Index == pos - 1);
            VisualBounds bounds = tree.findMeasure(part, prev_measure)?.ObjectBounds.Bounds;
            position.resetPositionToMeasureBeginning(bounds);

            List<Measure> to_update = part.ModelCollection.getItemsMatchingCondition(x => x.Position.Index >= pos);
            foreach (Measure mes in to_update)
            {
                var measure_bounds = tree.findMeasure(part, mes)?.ObjectBounds;
                measure_bounds?.updateBounds();
            }
        }

        public void updateMeasureBounds(Part part, Measure measure)
        {
            IBounded measure_bounds = tree.findMeasure(part, measure)?.ObjectBounds;
            if (measure_bounds is null) { return; }

            position.resetPositionToMeasureBeginning(measure_bounds.Bounds);
            measure_bounds.updateBounds();
        }

        public void updateChordBounds(Part part, Measure measure, Chord chord)
        {
            IBounded chord_bounds = tree.findChord(part, measure, chord)?.ObjectBounds;
            if (chord_bounds is null) { return; }

            position.resetPositionToChordBeginning(chord_bounds.Bounds);
            chord_bounds.updateBounds();
        }

        public void updatePartDrawing(Part part)
        {
            TreeNode part_node = tree.findPart(part);
            part_node?.ObjectDrawer.refreshVisual();
        }

        public void updateMeasureDrawing(Part part, Measure measure)
        {
            TreeNode measure_node = tree.findMeasure(part, measure);
            measure_node?.ObjectDrawer.refreshVisual();
        }

        public void updateChordDrawing(Part part, Measure measure, Chord chord)
        {
            TreeNode chord_node = tree.findChord(part, measure, chord);
            chord_node?.ObjectDrawer.refreshVisual();
        }

        public void updateNoteDrawing(Part part, Measure measure, Chord chord, Note note)
        {
            TreeNode note_node = tree.findNote(part, measure, chord, note);
            note_node?.ObjectDrawer.refreshVisual();
        }

        public void updateEffectDrawing(Part part, Measure measure, Chord chord, Note note, IEffect effect)
        {
            TreeNode effect_node = tree.findEffect(part, measure, chord, note, effect);
            effect_node?.ObjectDrawer.refreshVisual();
        }

        public void rebarMeasure(Part part, Measure measure)
        {
            TreeNode measure_node = tree.findMeasure(part, measure);
            if (measure_node == null) { return; }

            MeasureBarrer.barMeasure(measure_node);
            measure_node?.ObjectDrawer.refreshVisual();
        }

        public void rebarMeasures(Part part, List<Measure> measures)
        {
            foreach (Measure measure in measures) { rebarMeasure(part, measure); }
        }

        public void handleItemAdded(object sender, ObjectAddedArgs args)
        {
            tree.handleItemAdded(args.Parent, args);
        }
    }
}
