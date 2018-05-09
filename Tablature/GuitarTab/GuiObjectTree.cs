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
    //make the base object public. this will allow the removal of hasbaseobject, getbaseposition, and will make the findchild easier.
    //rename findobject to findchildwithoutparent and simplify.
    //maybe add a nodetype enum, which will contian extension methods to see if a depth is too far or not enough for findchild?
    public class TreeNode
    {
        public const int PART = 0;
        public const int MEASURE = 1;
        public const int CHORD = 2;
        public const int NOTE = 3;
        public const int EFFECT = 4;

        public object BaseObject { get; }
        public IBounded ObjectBounds { get; }
        public BaseMouseHandler ObjectHandler { get; }
        public TabDrawingVisual ObjectDrawer { get; }

        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; private set; }

        public TreeNode(object base_obj, IBounded bounds, BaseMouseHandler handler, TabDrawingVisual drawer)
        {
            BaseObject = base_obj;
            ObjectBounds = bounds;
            ObjectHandler = handler;
            ObjectDrawer = drawer;

            Parent = null;
            Children = new List<TreeNode>();
        }

        public TreeNode findChildWithoutParent(object child_base, int max_depth, int curr_depth)
        {
            if (BaseObject.Equals(child_base)) { return this; }
            
            if (curr_depth < max_depth)
            {
                foreach (TreeNode child in Children)
                {
                    var result = child.findChildWithoutParent(child_base, max_depth, curr_depth + 1);
                    if (result != null) { return result; }
                }
            }

            return null;
        }

        public TreeNode findChild(object child_base)
        {
            return (from child in Children
                    where child.BaseObject.Equals(child_base)
                    select child).FirstOrDefault();
        }

        public void addChild(TreeNode child)
        {
            Children.Add(child);
    
            child.Parent = this;
            child.subscribeToParentDelegates(ObjectBounds.Delegate, ObjectHandler.Delegate, ObjectDrawer.Delegate);
        }


        public void subscribeToParentDelegates(IDelegate bounding, IDelegate mouse, IDelegate drawing)
        {
            bounding?.subscribeAction(ObjectBounds.updateBounds, this);
            mouse?.subscribeAction(ObjectHandler.handleMouseEvent, this);
            drawing?.subscribeAction(ObjectDrawer.refreshVisual, this);
        }

        public void removeChild(TreeNode child)
        {
            Children.Remove(child);

            child.Parent = null;
            child.unsubscribeFromParentDelegates(ObjectBounds.Delegate, ObjectHandler.Delegate, ObjectDrawer.Delegate);
        }

        public void unsubscribeFromParentDelegates(IDelegate bounding, IDelegate mouse, IDelegate drawing)
        {
            bounding?.unsubscribeAction(ObjectBounds.updateBounds);
            mouse?.unsubscribeAction(ObjectHandler.handleMouseEvent);
            drawing?.unsubscribeAction(ObjectDrawer.refreshVisual);
        }
    }

    public class GuiObjectTree
    {
        public TreeNode Root { get; set; }
        private GuiObjectFactory factory;
        private TreeRemovedHolding holding;
        private TreeVisualCollection visuals;

        public GuiObjectTree(GuiObjectFactory f, TreeRemovedHolding h, TreeVisualCollection v)
        {
            Root = null;
            factory = f;
            holding = h;
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

        //put this in the factory? 
        public void handleItemAdded(object parent_obj, ObjectAddedArgs args)
        {
            object child_obj = args.Added;
            TreeNode parent = findObjectWithoutParents(parent_obj);
            if (parent != null)
            {
                TreeNode child = holding.searchForAddedItem(child_obj);
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
                holding.removeItem(removed);
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
                          where node.Equals(to_add)
                          select node).FirstOrDefault();
            if (found != null) { removedNodes.Remove(found); }
            return found;
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
