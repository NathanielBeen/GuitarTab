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
        //have the factory attach the node to its parent, as the factory has the more specific type of node and more casting will be avoided
        //change root to partTreeNode?
        public TreeNode Root { get; set; }
        private IGuiObjectFactory factory;
        private TreeChangedHolding click_holding;
        private TreeVisualCollection visuals;

        public GuiObjectTree(IGuiObjectFactory f, TreeChangedHolding c, TreeVisualCollection v)
        {
            Root = null;
            factory = f;
            click_holding = c;
            visuals = v;
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

        public void buildObject(TreeNode parent, object child)
        {
            TreeNode node = buildNode(child, parent);
            if (child is Part) { setRoot(node); }
        }

        public TreeNode buildNode(object obj, TreeNode parent)
        {
            TreeNode node = factory.buildObject(obj, parent);
            if (node != null)
            {
                attachNodeToTree(node, parent);
                buildIContainModels(obj, node);
                click_holding.nodeAdded(node);
            }

            return node;
        }

        public void buildIContainModels(object obj, TreeNode node)
        {
            if (obj is IContainModels)
            {
                IContainModels contain = obj as IContainModels;
                attachEventsToTree(contain);
                buildChildren(node, contain);
            }
        }

        public void setRoot(TreeNode new_root)
        {
            IRecieveDimensionUpdates old_bounds = (Root as PartTreeNode)?.getBounds();
            IRecieveDimensionUpdates new_bounds = (new_root as PartTreeNode)?.getBounds();

            Root = new_root;
            RootChanged?.Invoke(this, new ReceiverChangedEventArgs(old_bounds, new_bounds));
        }

        public void attachNodeToTree(TreeNode child, TreeNode parent)
        {
            visuals.addVisual(child);
            parent?.addChild(child);
        }

        public void attachEventsToTree(IContainModels node)
        {
            node.ModelAdded += handleItemAdded;
            node.ModelRemoved += handleItemRemoved;
        }

        public void removeEventsFromTree(IContainModels node)
        {
            node.ModelAdded -= handleItemAdded;
            node.ModelRemoved -= handleItemRemoved;
        }

        public void buildChildren(TreeNode node, IContainModels node_obj)
        {
            foreach (object obj in node_obj.getChildrenToBuild()) { buildObject(node, obj); }
        }

        public void handleItemAdded(object parent_obj, ObjectAddedArgs args)
        {
            object child_obj = args.Added;
            TreeNode parent = findObjectWithoutParents(parent_obj);
            if (parent != null) { buildObject(parent, child_obj); }
        }

        public void handleItemRemoved(object sender, ObjectRemovedArgs args)
        {
            TreeNode removed = findObjectWithoutParents(args.Removed);
            if (removed != null)
            {
                removed.Parent?.removeChild(removed);
                if (args.Removed is IContainModels) { removeEventsFromTree(args.Removed as IContainModels); }
                click_holding.nodeRemoved(removed);
                visuals.removeVisual(removed);
            }
        }

        public ObservableCollection<DrawingVisual> getTreeVisuals() { return visuals.Visuals; }

        public void HandleMouseEvent(MouseClick click) { Root?.handleMouseEvent(click); }

        public void populateMouseClick(NodeClick click) { click_holding.populateMouseClick(click); }

        public event EventHandler<ReceiverChangedEventArgs> RootChanged;
    }

    /*public class TreeRemovedHolding
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
    }*/

    public class TreeChangedHolding
    {
        private List<TreeNode> addedNodes;
        private List<TreeNode> removedNodes;

        public TreeChangedHolding()
        {
            addedNodes = new List<TreeNode>();
            removedNodes = new List<TreeNode>();
        }

        public void nodeAdded(TreeNode node) { addedNodes.Add(node); }

        public void nodeRemoved(TreeNode node) { removedNodes.Add(node); }

        public void clearChangedHolding() { addedNodes.Clear(); }

        public void populateMouseClick(NodeClick click)
        {
            foreach (TreeNode node in removedNodes) { node.removeFromIHoldTreeNodes(click); }
            foreach (TreeNode node in addedNodes) { node.addToIHoldTreeNodes(click); }
            addedNodes.Clear();
            removedNodes.Clear();
        }
    }

    public class TreeVisualCollection
    {
        public ObservableCollection<DrawingVisual> Visuals { get; }

        public TreeVisualCollection()
        {
            Visuals = new ObservableCollection<DrawingVisual>();
        }

        public void tryAddNode(TreeNode node)
        {
            if (!Visuals.Contains(node.getVisual())) { Visuals.Add(node.getVisual()); }
        }

        public void tryRemoveNode(TreeNode node)
        {
            if (Visuals.Contains(node.getVisual())) { Visuals.Remove(node.getVisual()); }
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
