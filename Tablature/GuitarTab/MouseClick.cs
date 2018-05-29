using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    //make bounds checking a type of mouse click
    public enum ClickType
    {
        Click,
        Select,
        Release,
        Position,
        Bounds,
        NoteSelect
    }

    public class MouseClick
    {
        public Point Point { get; }

        public MouseClick(Point p) { Point = p; }

        public virtual bool matchesClickType(ClickType desired) { return false; }
    }

    public class NodeClick : MouseClick
    {
        public PartTreeNode PartNode { get; set; }
        public List<MeasureTreeNode> MeasureNodes { get; set; }
        public List<ChordTreeNode> ChordNodes { get; set; }
        public List<NoteTreeNode> NoteNodes { get; set; }
        public EffectTreeNode EffectNode { get; set; }

        public VisualBounds Selected { get; set; }
        public bool Handled { get; private set; }

        public NodeClick(Point p)
            : base(p)
        {
            PartNode = null;
            MeasureNodes = new List<MeasureTreeNode>();
            ChordNodes = new List<ChordTreeNode>();
            NoteNodes = new List<NoteTreeNode>();
            EffectNode = null;

            Selected = null;
            Handled = false;
        }

        public void populateCommandSelections(CommandSelections selections)
        {
            selections.ClearModel();
            if (PartNode != null) { selections.SelectedPart = PartNode.getPart(); }
            foreach (MeasureTreeNode node in MeasureNodes) { selections.SelectedMeasure.Add(node.getMeasure()); }
            foreach (ChordTreeNode node in ChordNodes) { selections.SelectedChord.Add(node.getChord()); }
            foreach (NoteTreeNode node in NoteNodes) { selections.SelectedNote.Add(node.getNote()); }
            if (EffectNode != null) { selections.SelectedEffect = EffectNode.getEffect(); }
        }

        public MeasureTreeNode getFirstMeasureNodeByPosition()
        {
            return MeasureNodes.OrderBy(m => m.getMeasure().Position.Index).FirstOrDefault();
        }

        public TreeNode getFirstSelected()
        {
            if (EffectNode != null) { return EffectNode; }
            else if (NoteNodes.Any()) { return NoteNodes.First(); }
            else if (ChordNodes.Any()) { return ChordNodes.First(); }
            else if (MeasureNodes.Any()) { return MeasureNodes.First(); }
            else if (PartNode != null) { return PartNode; }
            return null;
        }

        public void setHandled() { Handled = true; }
    }

    public class NoteSelectClick : NodeClick
    {
        public NoteSelectClick(Point p) : base(p) { }

        public override bool matchesClickType(ClickType desired) { return (desired == ClickType.NoteSelect); }
    }

    public class StandardClick : NodeClick
    {
        private Selection add_type;

        public StandardClick(Selection add, Point p)
            : base(p)
        {
            add_type = add;
        }

        public bool multipleNotes() { return NoteNodes.Count() > 1 && !Handled; }

        public bool matchesSelectionType(Selection wanted) { return (wanted == add_type && !Handled); }

        public override bool matchesClickType(ClickType desired) { return (desired == ClickType.Click); }
    }

    public class ReleaseClick : NodeClick
    {
        public ReleaseClick(Point p) :base(p) { }

        public bool anyPart() { return PartNode != null && !Handled; }

        public bool anyMeasure() { return MeasureNodes.Any() && !Handled; }

        public bool multipleMeasures() { return MeasureNodes.Count() > 1 && !Handled; }

        public bool anyChord() { return ChordNodes.Any() && !Handled; }

        public bool multipleChords() { return ChordNodes.Count > 1 && !Handled; }

        public bool anyNote() { return NoteNodes.Any() && !Handled; }

        public bool multipleNotes() { return NoteNodes.Count > 1 && !Handled; }

        public bool anyEffect() { return EffectNode != null && !Handled; }

        public override bool matchesClickType(ClickType desired) { return (desired == ClickType.Release); }

        public bool chordMatchesFirst(Chord chord)
        {
            Chord other = (ChordNodes?.FirstOrDefault()?.BaseObject as Chord) ?? null;
            return (other == null) ? false : other.Equals(chord);
        }
    }

    public class SelectClick : NodeClick
    {
        public bool ContainsRect { get; private set; }
        public Rect Rectangle { get; }

        public SelectClick(Point p, Rect rect)
            :base(p)
        {
            Rectangle = rect;
            ContainsRect = false;
        }

        public void setContainsRect(VisualBounds bounds)
        {
            ContainsRect = bounds.containsRectangle(Rectangle);
        }

        public override bool matchesClickType(ClickType desired) { return (desired == ClickType.Select); }
    }

    public class PositionClick : MouseClick
    {
        public int Position { get; protected set; }

        public PositionClick(Point p)
            :base(p)
        {
            Position = 0;
        }

        public virtual void checkItem(int pos, VisualBounds bounds) { }

        public override bool matchesClickType(ClickType desired) { return (desired == ClickType.Position); }
    }

    public class MeasurePositionClick : PositionClick
    {
        public MeasurePositionClick(Point p) : base(p) { }

        public override void checkItem(int pos, VisualBounds bounds)
        {
            if (bounds.containsPoint(Point)) { Position = pos; }
        }
    }

    public class ChordPositionClick : PositionClick
    {
        private VisualBounds current_closest;

        public ChordPositionClick(Point p)
            :base(p)
        {
            current_closest = null;
        }

        public override void checkItem(int pos, VisualBounds bounds)
        {
            if (checkChordBar(bounds))
            {
                Position = pos + 1;
                current_closest = bounds;
            }
        }

        private bool checkChordBar(VisualBounds bounds)
        {
            if (bounds.Top <= Point.Y && bounds.Bottom >= Point.Y && bounds.Right <= Point.X) { return checkSameBar(bounds); }
            else if (bounds.Bottom <= Point.Y) { return checkPrevBar(bounds); }
            return false;
        }

        private bool checkSameBar(VisualBounds bounds)
        {
            if (current_closest is null) { return true; }
            return (current_closest.Bar < bounds.Bar || current_closest.Right <= bounds.Right);
        }

        private bool checkPrevBar(VisualBounds bounds)
        {
            if (current_closest is null) { return true; }
            if (current_closest.Bar > bounds.Bar) { return false; }
            return (current_closest.Right <= bounds.Right);
        }
    }

    public class BoundsClick : MouseClick
    {
        public VisualBounds DeepestBounds { get; set; }

        public BoundsClick(Point p)
            :base(p)
        {
            DeepestBounds = null;
        }

        public override bool matchesClickType(ClickType desired) { return desired == ClickType.Bounds; }
    }
}
