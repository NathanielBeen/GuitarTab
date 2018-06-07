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

        public void setTreePart(Part part) { tree.buildTree(part); }

        public void updatePartBounds(PartTreeNode node)
        {
            position.resetPositionToPartBeginning(node.ObjectBounds.Bounds);
            node?.ObjectBounds.updateBounds();
        }

        public void updateMeasureBoundsAtAndAfter(MeasureTreeNode prev_measure_node, List<MeasureTreeNode> to_update)
        {
            var bounds = prev_measure_node?.ObjectBounds.Bounds as MultipleVisualBounds;
            position.resetPositionToMeasureEnd(bounds?.AllBounds.Last());
            foreach (MeasureTreeNode measure in to_update) { measure.ObjectBounds.updateBounds(); }
        }

        public void updateMeasureBounds(MeasureTreeNode measure_node)
        {
            var bounds = measure_node.ObjectBounds.Bounds as MultipleVisualBounds;
            position.resetPositionToMeasureBeginning(bounds.AllBounds.First());
            measure_node.ObjectBounds.updateBounds();
        }

        public void updateChordBounds(ChordTreeNode chord_node)
        {
            position.resetPositionToChordBeginning(chord_node.ObjectBounds.Bounds);
            chord_node.ObjectBounds.updateBounds();
        }

        public void updateDrawing(TreeNode node) { node?.ObjectDrawer.refreshVisual(); }

        public void rebarMeasure(MeasureTreeNode measure_node)
        {
            MeasureBarrer.barMeasure(measure_node);
            TupletBarrer.barMeasure(measure_node);
            measure_node?.ObjectDrawer.refreshVisual();
        }

        public void rebarMeasures(PartTreeNode part_node, List<MeasureTreeNode> measure_nodes)
        {
            foreach (MeasureTreeNode measure in measure_nodes) { rebarMeasure(measure); }
        }

        public void handleItemAdded(object sender, ObjectAddedArgs args) { tree.handleItemAdded(args.Parent, args); }

        public void populateMouseClick(NodeClick click) { tree.populateMouseClick(click); }
    }
}
