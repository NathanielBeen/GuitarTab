using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GuitarTab
{
    public class GuiObjectTree
    {
        public TreeNode Root { get; set; }
        private GuiObjectFactory factory;
        private TreeAddedHolding added_holding;
        private TreeRemovedHolding removed_holding;
        private TreeVisualCollection visuals;

        public GuiObjectTree(GuiObjectFactory f, TreeAddedHolding a, TreeRemovedHolding r, TreeVisualCollection v)
        {
            Root = null;
            factory = f;
            added_holding = a;
            removed_holding = r;
            visuals = v;
        }

        public TreeNode findPart(Part part)
        {
            return (Root.BaseObject.Equals(part)) ? Root : null;
        }

        public TreeNode findMeasure(Part part, Measure measure)
        {
            var part_node = findPart(part);
            if (part_node is null) { return null; }

            return part_node.findChild(measure);
        }

        public TreeNode findChord(Part part, Measure measure, Chord chord)
        {
            var measure_node = findMeasure(part, measure);
            if (measure_node is null) { return null; }

            return measure_node.findChild(chord);
        }

        public TreeNode findNote(Part part, Measure measure, Chord chord, Note note)
        {
            var chord_node = findChord(part, measure, chord);
            if (chord_node is null) { return null; }

            return chord_node.findChild(note);
        }

        public TreeNode findEffect(Part part, Measure measure, Chord chord, Note note, IEffect effect)
        {
            var note_node = findNote(part, measure, chord, note);
            if (note_node is null) { return null; }

            return note_node.findChild(effect);
        }
        
        public TreeNode findObjectWithoutParents(object base_object)
        {
            int max_depth = TreeNode.PART;
            if (base_object is Measure) { max_depth = TreeNode.MEASURE; }
            else if(base_object is Chord) { max_depth = TreeNode.CHORD; }
            else if (base_object is Note) { max_depth = TreeNode.NOTE; }
            else if (base_object is IEffect) { max_depth = TreeNode.EFFECT; }

            return Root?.findChildWithoutParent(base_object, max_depth, 0) ?? null;
        }

        //this whole mess should not be in the guitree. possibly put it in the guiupdater or guifactory class.
        //instead of calling buildobject, first call the specific factory method
        //then call the addvisual, addchild, ect in a common method
        //have a check for IContainObjects and add to their itemadded and itemremoved events in another common method
        //(then call to build children if needed)
        public void buildAddedNode(TreeNode parent, object child)
        {
            if (child is Part) { buildTree((Part)child); }
            else if (child is Measure) { buildMeasure((Measure)child, parent); }
            else if (child is Chord) { buildChord((Chord)child, parent); }
            else if (child is Note) { buildNote((Note)child, parent); }
            else if (child is IEffect) { buildEffect((IEffect)child, parent); }
        }

        public void buildTree(Part part)
        {
            TreeNode node = factory.buildPart(part);
            if (node != null)
            {
                visuals.addVisual(node);
                attachEventsToTree(part);
                buildChildren(node, part);
                Root = node;
            }
        }

        public void buildMeasure(Measure measure, TreeNode parent)
        {
            TreeNode child = factory.buildMeasure(measure);
            if (child != null)
            {
                attachNodeToTree(child, parent);
                attachEventsToTree(measure);
                buildChildren(child, measure);
            }
        }

        public void buildChord(Chord chord, TreeNode parent)
        {
            TreeNode child = factory.buildChord(chord);

            if (child != null)
            {
                attachNodeToTree(child, parent);
                if (chord is IContainModels)
                {
                    var contained_chord = chord as IContainModels;
                    attachEventsToTree(contained_chord);
                    buildChildren(child, contained_chord);
                }
            }
        }

        public void buildNote(Note note, TreeNode parent)
        {
            TreeNode child = factory.buildNote(note);
            if (child != null)
            {
                attachNodeToTree(child, parent);
                attachEventsToTree(note);

                List<IEffect> effects = note.ModelCollection.getItemsMatchingCondition(e => !(e is IMultiEffect && e.getPosition(note) == EffectPosition.Into));
                foreach (IEffect effect in effects)
                {
                    buildEffect(effect, parent);
                }
            }
        }

        public void buildEffect(IEffect effect, TreeNode parent)
        {
            TreeNode child = factory.buildEffectNode(effect, parent);
            if (child != null) { attachNodeToTree(child, parent); }
        }

        public void attachNodeToTree(TreeNode child, TreeNode parent)
        {
            visuals.addVisual(child);
            parent.addChild(child);
        }

        public void attachEventsToTree(IContainModels node)
        {
            node.ModelAdded += handleItemAdded;
            node.ModelRemoved += handleItemRemoved;
        }

        public void buildChildren(TreeNode node, IContainModels node_obj)
        {
            foreach (object obj in node_obj.getGenericModelList()) { buildAddedNode(node, obj); }
        }

        public void handleItemAdded(object parent_obj, ObjectAddedArgs args)
        {
            object child_obj = args.Added;
            TreeNode parent = findObjectWithoutParents(parent_obj);
            if (parent != null)
            {
                TreeNode child = removed_holding.searchForAddedItem(child_obj);
                if (child == null) { buildAddedNode(parent, child_obj); }
                else { attachNodeToTree(child, parent); }
            }
        }

        public void handleItemRemoved(object sender, ObjectRemovedArgs args)
        {
            TreeNode removed = findObjectWithoutParents(args.Removed);
            if (removed != null)
            {
                removed.Parent?.removeChild(removed);
                removed_holding.removeItem(removed);
                visuals.removeVisual(removed);
            }
        }

        public ObservableCollection<TabDrawingVisual> getTreeVisuals()
        {
            return visuals.Visuals;
        }

        public void HandleMouseEvent()
        {
            Root?.ObjectHandler.handleMouseEvent();
        }

        public VisualBounds GetDeepestBoundsAtPosition(Point point)
        {
            return GetDeepestBoundsAtPosition(point, Root);
        }

        public VisualBounds GetDeepestBoundsAtPosition(Point point, TreeNode node)
        {
            foreach (TreeNode child in node?.Children)
            {
                var found = GetDeepestBoundsAtPosition(point, child);
                if (found != null) { return found; }
            }
            return (node?.ObjectHandler.hitTest(point) ?? false) ? node.ObjectBounds.Bounds : null;
        }
    }

    public class TreeRemovedHolding
    {
        private List<TreeNode> removedNodes;
        private int num_retained;

        public TreeRemovedHolding(int retained)
        {
            removedNodes = new List<TreeNode>();
            num_retained = retained;
        }

        public void removeItem(TreeNode node)
        {
            removedNodes.Insert(0, node);
            if (removedNodes.Count() > num_retained) { removedNodes.RemoveAt(removedNodes.Count() - 1); }
        }

        public TreeNode searchForAddedItem(object to_add)
        {
            var found =  (from node in removedNodes
                          where node.BaseObject.Equals(to_add)
                          select node).FirstOrDefault();
            if (found != null) { removedNodes.Remove(found); }
            return found;
        }
    }

    public class TreeAddedHolding
    {
        private List<TreeNode> addedNodes;

        public TreeAddedHolding()
        {
            addedNodes = new List<TreeNode>();
        }

        public void clearAddedHolding() { addedNodes.Clear(); }

        public void populateMouseClick(MouseClick click)
        {
            foreach (TreeNode node in addedNodes) { node.addToMouseClick(click); }
            addedNodes.Clear();
        }
    }

    public class TreeVisualCollection
    {
        public ObservableCollection<TabDrawingVisual> Visuals { get; }

        public TreeVisualCollection()
        {
            Visuals = new ObservableCollection<TabDrawingVisual>();
        }

        public void tryAddNode(TreeNode node)
        {
            if (!Visuals.Contains(node.ObjectDrawer)) { Visuals.Add(node.ObjectDrawer); }
        }

        public void tryRemoveNode(TreeNode node)
        {
            if (Visuals.Contains(node.ObjectDrawer)) { Visuals.Remove(node.ObjectDrawer); }
        }

        public void addVisual(TreeNode node)
        {
            tryAddNode(node);
            foreach (TreeNode child in node.Children) { addVisual(child); }
        }

        public void removeVisual(TreeNode node)
        {
            tryRemoveNode(node);
            foreach (TreeNode child in node.Children) { removeVisual(child); }
        }
    }
}
