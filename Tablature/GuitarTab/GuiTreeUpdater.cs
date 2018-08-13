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
        private CurrentPosition position;

        public GuiTreeUpdater(GuiObjectTree t, CurrentPosition curr)
        {
            tree = t;
            position = curr;
        }

        public void setTreePart(Part part) { tree.buildObject(null, part); }

        public void updatePartBounds(PartTreeNode node)
        {
            position.resetPositionToPartBeginning(node.Bounds);
            node?.updateBounds();
        }

        public void updateMeasureBoundsAtAndAfter(MeasureTreeNode prev_measure_node, List<MeasureTreeNode> to_update)
        {
            IBounds bounds = prev_measure_node?.Bounds.getBoundsList().LastOrDefault();
            position.resetPositionToMeasureEnd(bounds);
            foreach (MeasureTreeNode measure in to_update) { measure.updateBounds(); }
        }

        public void updateMeasureBounds(MeasureTreeNode measure_node)
        {
            IBounds bounds = measure_node?.Bounds.getBoundsList().FirstOrDefault();
            position.resetPositionToMeasureBeginning(bounds);
            measure_node.updateBounds();
        }

        public void updateChordBounds(ChordTreeNode chord_node)
        {
            position.resetPositionToChordBeginning(chord_node.Bounds);
            chord_node.updateBounds();
        }

        public void updateDrawing(TreeNode node) { node?.refreshVisual(); }

        public void updateRootDrawing() { tree.Root?.refreshVisual(); }

        public static void rebarPart(TreeNode node)
        {
            if (node == null) { return; }
            foreach (MeasureTreeNode measure in node.Children) { rebarMeasure(measure); }
        }

        public static void rebarMeasure(MeasureTreeNode measure_node)
        {
            MeasureBarrer.barMeasure(measure_node);
            TupletBarrer.barMeasure(measure_node);
            measure_node?.refreshVisual();
        }

        public void rebarMeasures(List<MeasureTreeNode> measure_nodes)
        {
            foreach (MeasureTreeNode measure in measure_nodes) { rebarMeasure(measure); }
        }

        public void populateMouseClick(NodeClick click) { tree.populateMouseClick(click); }
    }
}
