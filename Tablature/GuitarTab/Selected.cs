using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class Selected
    {
        public MouseSelectedView SelectedView { get; set; }

        private PartTreeNode part_node;
        private List<MeasureTreeNode> measure_nodes;
        private List<ChordTreeNode> chord_nodes;
        private List<NoteTreeNode> note_nodes;
        private EffectTreeNode effect_node;

        public Selected()
        {
            part_node = null;
            measure_nodes = new List<MeasureTreeNode>();
            chord_nodes = new List<ChordTreeNode>();
            note_nodes = new List<NoteTreeNode>();
            effect_node = null;
        }

        public void populateNodeClick(NodeClick click)
        {
            click.PartNode = part_node;
            click.MeasureNodes = measure_nodes;
            click.ChordNodes = chord_nodes;
            click.NoteNodes = note_nodes;
            click.EffectNode = effect_node;
        }

        public void populateNodeClickForMultiEffect(NodeClick click)
        {
            click.NoteNodes = note_nodes;
        }

        public void populateFromClick(NodeClick click)
        {
            part_node = click.PartNode;
            measure_nodes = click.MeasureNodes;
            chord_nodes = click.ChordNodes;
            note_nodes = click.NoteNodes;
            effect_node = click.EffectNode;

            setSelected();
        }

        private void setSelected()
        {
            var new_selected = new List<TreeNode>(); 

            if (effect_node != null) { new_selected.Add(effect_node); }
            else if (note_nodes.Any()) { new_selected.AddRange(note_nodes); }
            else if (chord_nodes.Any()) { new_selected.AddRange(chord_nodes); }
            else if (measure_nodes.Any()) { new_selected.AddRange(measure_nodes); }
            else if (part_node != null) { new_selected.Add(part_node); }

            SelectedView.setSelectedObjects(new_selected);
        }

        public List<VisualBounds> getSelected() { return SelectedView.Selected.ToList(); }

        public TreeNode getFirstSelected()
        {
            if (effect_node != null) { return effect_node; }
            else if (note_nodes.Any()) { return note_nodes.First(); }
            else if (chord_nodes.Any()) { return chord_nodes.First(); }
            else if (measure_nodes.Any()) { return measure_nodes.First(); }
            else if (part_node != null) { return part_node; }
            return null;
        }
    }

    public enum Selection
    {
        Add_Measure,
        Add_Rest,
        Add_Note,
        Add_Effect,
        Add_Multi_Effect,
        Set_Length,
        Standard
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
                default:
                    return AddItem.None;
            }
        }
    }
}
