using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace GuitarTab
{
    //use composition to make bounded, handler, drawer all private and have the node reroute all neccisary methods so all references go to
    //the node instead of the parts

    //make an interface IContainTreeNodes<T> and have each subclass inherit the proper T to avoid having to cast a whole collection
    //have methods like "getNext", "getPrevious", "getChildAtPosition", "getChildrenAfterPosition", "addChild", "removeChild"
    public class TreeNode : IBounded, IHandleMouseEvents, IDraw
    {
        public const int PART = 0;
        public const int MEASURE = 1;
        public const int CHORD = 2;
        public const int NOTE = 3;
        public const int EFFECT = 4;

        public virtual object BaseObject { get; }
        protected IBounded bounds;
        protected IHandleMouseEvents handler;
        protected StaticDrawingVisual drawer;

        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; private set; }

        public IDelegate BoundsDelegate { get => bounds.BoundsDelegate; set => bounds.BoundsDelegate = value; }
        public IMouseDelegate MouseDelegate { get => handler.MouseDelegate; set => handler.MouseDelegate = value; }
        public IDelegate DrawDelegate { get => drawer.DrawDelegate; set => drawer.DrawDelegate = value; }
        public IBounds Bounds { get => bounds.Bounds; set => bounds.Bounds = value; }
        public IBoundedStrategy Strategy => bounds.Strategy;

        public TreeNode(IBounded b, IHandleMouseEvents h, StaticDrawingVisual d)
        {
            bounds = b;
            handler = h;
            drawer = d;

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
            child.subscribeToParentDelegates(BoundsDelegate, MouseDelegate, DrawDelegate);
        }

        public void removeChild(TreeNode child)
        {
            Children.Remove(child);

            child.Parent = null;
            child.unsubscribeFromParentDelegates(BoundsDelegate, MouseDelegate, DrawDelegate);
        }

        private void subscribeToParentDelegates(IDelegate bounding, IMouseDelegate mouse, IDelegate drawing)
        {
            bounding?.subscribeAction(updateBounds, this);
            mouse?.subscribeAction(handleMouseEvent, this);
            drawing?.subscribeAction(refreshVisual, this);
        }

        private void unsubscribeFromParentDelegates(IDelegate bounding, IMouseDelegate mouse, IDelegate drawing)
        {
            bounding?.unsubscribeAction(updateBounds);
            mouse?.unsubscribeAction(handleMouseEvent);
            drawing?.unsubscribeAction(refreshVisual);
        }

        public virtual void addToIHoldTreeNodes(IHoldTreeNodes nodes) { }

        public virtual void removeFromIHoldTreeNodes(IHoldTreeNodes nodes) { }

        public bool hitTest(Point point) { return bounds.hitTest(point); }

        public void updateBounds() { bounds.updateBounds(); }

        public void handleMouseEvent(MouseClick click)
        {
            (click as NodeClick)?.setPropNode(this);
            handler.handleMouseEvent(click);
        }

        public void refreshVisual() { drawer.refreshVisual(); }

        public DrawingVisual getVisual() { return drawer.getVisual(); }

        public IBounds initBounds() { return bounds.initBounds(); }
    }

    public class PartTreeNode : TreeNode
    {
        private Part part;
        public override object BaseObject { get { return part; } }

        public PartTreeNode(Part p, IBounded bounds, IHandleMouseEvents handler, StaticDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            part = p;
        }

        public Part getPart() { return part; }

        public override void addToIHoldTreeNodes(IHoldTreeNodes nodes) { nodes.PartNode = this; }

        public override void removeFromIHoldTreeNodes(IHoldTreeNodes nodes)
        {
            if (nodes.PartNode != null && nodes.PartNode.Equals(this)) { nodes.PartNode = null; }
        }

        public MeasureTreeNode getMeasureNodeAtPosition(int pos)
        {
            return (from node in Children
                    where (node as MeasureTreeNode).getMeasure().Position.Index == pos
                    select node as MeasureTreeNode).FirstOrDefault();
        }

        public List<MeasureTreeNode> getMeasureNodesAtAndAfterPosition(int pos)
        {
            return (from node in Children
                    where (node as MeasureTreeNode).getMeasure().Position.Index >= pos
                    orderby (node as MeasureTreeNode).getMeasure().Position.Index
                    select node as MeasureTreeNode).ToList();
        }

        public void beginRedrawMeasureHeads() { part.updateMeasureMatching(); }

        public void endRedrawMeasureheads() { foreach (MeasureTreeNode node in Children) { node.redrawMeasureHead(); } }

        
        public NoteTreeNode getNextNote(NoteTreeNode note_node)
        {
            MultiPosition note_pos = note_node.getNote().getPosition();
            if (note_pos == null) { return null; }

            MeasureTreeNode measure = getMeasureNodeAtPosition(note_pos.getNextMeasurePosition());
            if (measure == null) { return null; }

            ChordTreeNode chord = measure.getChordNodeAtPosition(note_pos.getNextPosition());
            if (chord == null) { return null; }

            return chord.getNoteNodeAtString(note_node.getNote().String);
        }
        

        public NoteTreeNode getPreviousNote(NoteTreeNode note_node)
        {
            MultiPosition note_pos = note_node.getNote().getPosition();
            if (note_pos == null) { return null; }

            MeasureTreeNode measure = getMeasureNodeAtPosition(note_pos.getPreviousMeasurePosition());
            if (measure == null) { return null; }

            ChordTreeNode chord = measure.getChordNodeAtPosition(note_pos.getPreviousPosition(measure.Children.Count()));
            if(chord == null) { return null; }

            return chord.getNoteNodeAtString(note_node.getNote().String);
        }

        public IRecieveDimensionUpdates getBounds() { return bounds.Strategy as IPartBounds; }
    }

    public class MeasureTreeNode : TreeNode
    {
        private Measure measure;
        public override object BaseObject { get { return measure; } }

        public MeasureTreeNode(Measure m, IBounded bounds, IHandleMouseEvents handler, StaticDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            measure = m;
        }

        public Measure getMeasure() { return measure; }

        public override void addToIHoldTreeNodes(IHoldTreeNodes nodes) { nodes.MeasureNodes.Add(this); }

        public override void removeFromIHoldTreeNodes(IHoldTreeNodes nodes)
        {
            if (nodes.MeasureNodes.Contains(this)) { nodes.MeasureNodes.Remove(this); }
        }

        public ChordTreeNode getChordNodeAtPosition(int pos)
        {
            return (from node in Children
                    where (node as ChordTreeNode).getChord().Position.Index == pos
                    select node as ChordTreeNode).FirstOrDefault();
        }

        public void redrawMeasureHead()
        {
            if (measure.MatchUpdated)
            {
                measure.MatchUpdated = false;
                (drawer as DynamicMeasureDrawingVisual).refreshMeasure();
            }
        }
    }

    public class ChordTreeNode : TreeNode
    {
        private Chord chord;
        public override object BaseObject { get { return chord; } }

        public ChordTreeNode(Chord c, IBounded bounds, IHandleMouseEvents handler, StaticDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            chord = c;
        }

        public Chord getChord() { return chord; }

        public override void addToIHoldTreeNodes(IHoldTreeNodes nodes) { nodes.ChordNodes.Add(this); }

        public override void removeFromIHoldTreeNodes(IHoldTreeNodes nodes)
        {
            if (nodes.ChordNodes.Contains(this)) { nodes.ChordNodes.Remove(this); }
        }

        public NoteTreeNode getNoteNodeAtString(int str)
        {
            return (from node in Children
                    where (node as NoteTreeNode).getNote().String == str
                    select node as NoteTreeNode).FirstOrDefault();
        }

        public IChordBounds getBounds() { return bounds.Strategy as IChordBounds; }
    }

    public class NoteTreeNode : TreeNode
    {
        private Note note;
        public override object BaseObject { get { return note; } }

        public NoteTreeNode(Note n, IBounded bounds, IHandleMouseEvents handler, StaticDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            note = n;
        }

        public Note getNote() { return note; }

        public override void addToIHoldTreeNodes(IHoldTreeNodes nodes) { nodes.NoteNodes.Add(this); }

        public override void removeFromIHoldTreeNodes(IHoldTreeNodes nodes)
        {
            if (nodes.NoteNodes.Contains(this)) { nodes.NoteNodes.Remove(this); }
        }
    }

    public class EffectTreeNode : TreeNode
    {
        private IEffect effect;
        public override object BaseObject { get { return effect; } }

        public EffectTreeNode(IEffect e, IBounded effect_bounds, IHandleMouseEvents handler, StaticDrawingVisual visual)
            :base(effect_bounds, handler, visual)
        {
            effect = e;
        }

        public IEffect getEffect() { return effect; }

        public override void addToIHoldTreeNodes(IHoldTreeNodes nodes) { nodes.EffectNode = this; }

        public override void removeFromIHoldTreeNodes(IHoldTreeNodes nodes)
        {
            if (nodes.EffectNode != null && nodes.EffectNode.Equals(this)) { nodes.EffectNode = null; }
        }
    }
}
