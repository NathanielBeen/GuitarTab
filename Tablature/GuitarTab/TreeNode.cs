using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class TreeNode
    {
        public const int PART = 0;
        public const int MEASURE = 1;
        public const int CHORD = 2;
        public const int NOTE = 3;
        public const int EFFECT = 4;

        public virtual object BaseObject { get; }
        public BaseBounded ObjectBounds { get; }
        public BaseMouseHandler ObjectHandler { get; }
        public TabDrawingVisual ObjectDrawer { get; }

        public TreeNode Parent { get; set; }
        public List<TreeNode> Children { get; private set; }

        public TreeNode(BaseBounded bounds, BaseMouseHandler handler, TabDrawingVisual drawer)
        {
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

        public void removeChild(TreeNode child)
        {
            Children.Remove(child);

            child.Parent = null;
            child.unsubscribeFromParentDelegates(ObjectBounds.Delegate, ObjectHandler.Delegate, ObjectDrawer.Delegate);
        }

        public void subscribeToParentDelegates(IDelegate bounding, IMouseDelegate mouse, IDelegate drawing)
        {
            bounding?.subscribeAction(ObjectBounds.updateBounds, this);
            mouse?.subscribeAction(handleMouseClick, this);
            drawing?.subscribeAction(ObjectDrawer.refreshVisual, this);
        }

        public void unsubscribeFromParentDelegates(IDelegate bounding, IMouseDelegate mouse, IDelegate drawing)
        {
            bounding?.unsubscribeAction(ObjectBounds.updateBounds);
            mouse?.unsubscribeAction(handleMouseClick);
            drawing?.unsubscribeAction(ObjectDrawer.refreshVisual);
        }

        public void handleMouseClick(MouseClick click)
        {
            if (click.matchesClickType(ClickType.Position)) { handlePositionClick(click as PositionClick); }
            else if (click.matchesClickType(ClickType.Select)) { handleSelectClick(click as SelectClick); }
            else if (click.matchesClickType(ClickType.Bounds)) { handleBoundsClick(click as BoundsClick); }

            else if (ObjectBounds.hitTest(click.Point))
            {
                addToMouseClick(click as NodeClick);
                ObjectHandler.handleMouseEvent(click);
            }
        }

        public void handleSelectClick(SelectClick click)
        {
            if (!ObjectBounds.Bounds.containedInRectangle(click.Rectangle)) { return; }

            addToMouseClick(click);
            click?.setContainsRect(ObjectBounds.Bounds);
            if (click?.ContainsRect ?? false) { ObjectHandler?.invokeClickDelegate(click); }
        }

        public void handleBoundsClick(BoundsClick click)
        {
            if (ObjectBounds.hitTest(click.Point))
            {
                click.DeepestBounds = ObjectBounds.Bounds;
                ObjectHandler?.invokeClickDelegate(click);
            }
        }

        public virtual void handlePositionClick(PositionClick click) { }

        public virtual void addToMouseClick(NodeClick click) { }

        public virtual void removeFromMouseClick(NodeClick click) { }

        public virtual void addToSelected(Selected selected) { }

        public virtual void removeFromSelected(Selected selected) { }
    }

    public class PartTreeNode : TreeNode
    {
        private Part part;
        public override object BaseObject { get { return part; } }

        public PartTreeNode(Part p, PartBounds bounds, PartMouseHandler handler, PartDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            part = p;
        }

        public Part getPart() { return part; }

        public override void addToMouseClick(NodeClick click) { click.PartNode = this; }

        public override void removeFromMouseClick(NodeClick click)
        {
            if (click.PartNode != null && click.PartNode.Equals(this)) { click.PartNode = null; }
        }

        public override void addToSelected(Selected selected) { selected.PartNode = this; }

        public override void removeFromSelected(Selected selected)
        {
            if (selected.PartNode != null && selected.PartNode.Equals(this)) { selected.PartNode = null; }
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
    }

    public class MeasureTreeNode : TreeNode
    {
        private Measure measure;
        public override object BaseObject { get { return measure; } }

        public MeasureTreeNode(Measure m, MeasureBounds bounds, MeasureMouseHandler handler, MeasureDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            measure = m;
        }

        public Measure getMeasure() { return measure; }

        public override void addToMouseClick(NodeClick click) { click.MeasureNodes.Add(this); }

        public override void removeFromMouseClick(NodeClick click)
        {
            if (click.MeasureNodes.Contains(this)) { click.MeasureNodes.Remove(this); }
        }

        public override void addToSelected(Selected selected) { selected.MeasureNodes.Add(this); }

        public override void removeFromSelected(Selected selected)
        {
            if (selected.MeasureNodes.Contains(this)) { selected.MeasureNodes.Remove(this); }
        }

        public override void handlePositionClick(PositionClick click) { click?.checkItem(measure.Position.Index, ObjectBounds.Bounds); }

        public void redrawMeasureHead()
        {
            if (measure.MatchUpdated)
            {
                measure.MatchUpdated = false;
                (ObjectDrawer as MeasureDrawingVisual).refreshMeasure();
            }
        }
    }

    public class ChordTreeNode : TreeNode
    {
        private Chord chord;
        public override object BaseObject { get { return chord; } }

        public ChordTreeNode(Chord c, ChordBounds bounds, ChordMouseHandler handler, ChordDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            chord = c;
        }

        public Chord getChord() { return chord; }

        public override void addToMouseClick(NodeClick click) { click.ChordNodes.Add(this); }

        public override void removeFromMouseClick(NodeClick click)
        {
            if (click.ChordNodes.Contains(this)) { click.ChordNodes.Remove(this); }
        }

        public override void addToSelected(Selected selected) { selected.ChordNodes.Add(this); }

        public override void removeFromSelected(Selected selected)
        {
            if (selected.ChordNodes.Contains(this)) { selected.ChordNodes.Remove(this); }
        }

        public override void handlePositionClick(PositionClick click) { click?.checkItem(chord.Position.Index, ObjectBounds.Bounds); }
    }

    public class NoteTreeNode : TreeNode
    {
        private Note note;
        public override object BaseObject { get { return note; } }

        public NoteTreeNode(Note n, NoteBounds bounds, NoteMouseHandler handler, NoteDrawingVisual visual)
            :base(bounds, handler, visual)
        {
            note = n;
        }

        public Note getNote() { return note; }

        public override void addToMouseClick(NodeClick click) { click.NoteNodes.Add(this); }

        public override void removeFromMouseClick(NodeClick click)
        {
            if (click.NoteNodes.Contains(this)) { click.NoteNodes.Remove(this); }
        }

        public override void addToSelected(Selected selected) { selected.NoteNodes.Add(this); }

        public override void removeFromSelected(Selected selected)
        {
            if (selected.NoteNodes.Contains(this)) { selected.NoteNodes.Remove(this); }
        }
    }

    public class EffectTreeNode : TreeNode
    {
        private IEffect effect;
        public override object BaseObject { get { return effect; } }

        public EffectTreeNode(IEffect e, BaseBounded effect_bounds, EffectMouseHandler handler, TabDrawingVisual visual)
            :base(effect_bounds, handler, visual)
        {
            effect = e;
        }

        public IEffect getEffect() { return effect; }

        public override void addToMouseClick(NodeClick click) { click.EffectNode = this; }

        public override void removeFromMouseClick(NodeClick click)
        {
            if (click.EffectNode != null && click.EffectNode.Equals(this)) { click.EffectNode = null; }
        }
    }
}
