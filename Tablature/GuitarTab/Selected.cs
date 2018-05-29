using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GuitarTab
{
    public class Selected
    {
        public MouseSelectedView SelectedView { get; set; }

        public PartTreeNode PartNode;
        public List<MeasureTreeNode> MeasureNodes;
        public List<ChordTreeNode> ChordNodes;
        public List<NoteTreeNode> NoteNodes;
        public EffectTreeNode EffectNode;

        private List<TreeNode> selected;

        public Selected()
        {
            PartNode = null;
            MeasureNodes = new List<MeasureTreeNode>();
            ChordNodes = new List<ChordTreeNode>();
            NoteNodes = new List<NoteTreeNode>();
            EffectNode = null;
        }

        public void populateNodeClick(NodeClick click)
        {
            click.PartNode = PartNode;
            click.MeasureNodes = MeasureNodes;
            click.ChordNodes = ChordNodes;
            click.NoteNodes = NoteNodes;
            click.EffectNode = EffectNode;
        }

        public void populateNodeClickForMultiEffect(NodeClick click)
        {
            click.NoteNodes = NoteNodes;
        }

        public void populateFromClick(NodeClick click)
        {
            Clear();
            setSelected(click);
            refreshSelectedTree();
            SelectedView.setSelectedObjects(selected.ToList());
        }

        public void Clear()
        {
            PartNode = null;
            MeasureNodes.Clear();
            ChordNodes.Clear();
            NoteNodes.Clear();
            EffectNode = null;
        }

        private void setSelected(NodeClick click)
        {
            selected = new List<TreeNode>();
            if (click.NoteNodes.Any()) { selected.AddRange(click.NoteNodes); }
            else if (click.ChordNodes.Any()) { selected.AddRange(click.ChordNodes); }
            else if (click.MeasureNodes.Any()) { selected.AddRange(click.MeasureNodes); }
        }

        private void refreshSelectedTree()
        {
            foreach (TreeNode node in selected) { node.addToSelected(this); }
            TreeNode curr_node = selected.FirstOrDefault()?.Parent;
            while (curr_node != null)
            {
                curr_node.addToSelected(this);
                curr_node = curr_node.Parent;
            }
        }

        public List<VisualBounds> getSelected() { return SelectedView.Selected.ToList(); }

        public bool selectedContainsPoint(Point point)
        {
            foreach (var item in selected)
            {
                if (item.ObjectBounds.hitTest(point)) { return true; }
            }
            return false;
        }
    }

    public enum Selection
    {
        Standard,
        Add_Measure,
        Add_Rest,
        Add_Note,
        Add_Effect,
        Add_Multi_Effect,
        Set_Length
    }

    public static class SelectionExtensions
    {
        public static AddItem convertToAddItem(this Selection selection)
        {
            switch (selection)
            {
                case Selection.Add_Rest:
                    return AddItem.Rest;
                case Selection.Add_Note:
                    return AddItem.Note;
                case Selection.Add_Multi_Effect:
                    return AddItem.Slide;
                case Selection.Add_Measure:
                    return AddItem.Measure;
                case Selection.Add_Effect:
                    return AddItem.Bend;
                case Selection.Set_Length:
                    return AddItem.Length;
                default:
                    return AddItem.None;
            }
        }
    }
}
