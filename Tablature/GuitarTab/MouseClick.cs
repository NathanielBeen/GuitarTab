using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public enum ClickType
    {
        Click,
        DoubleClick,
        Select,
        Release,
        Position
    }

    public class MouseClick
    {
        private ClickType type;
        private Selection selection;
        public bool Handled { get; set; }

        public PartTreeNode PartNode { get; set; }
        public List<MeasureTreeNode> MeasureNodes { get; set; }
        public List<ChordTreeNode> ChordNodes { get; set; }
        public List<NoteTreeNode> NoteNodes { get; set; }
        public List<EffectTreeNode> EffectNodes { get; set; }

        public VisualBounds Selected { get; set; }

        public MouseClick(ClickType t, Selection s)
        {
            type = t;
            selection = s;
            Handled = false;

            PartNode = null;
            MeasureNodes = new List<MeasureTreeNode>();
            ChordNodes = new List<ChordTreeNode>();
            NoteNodes = new List<NoteTreeNode>();
            EffectNodes = new List<EffectTreeNode>();

            Selected = null;
        }

        public void populateCommandSelections(CommandSelections selections)
        {
            if (PartNode != null) { selections.SelectedPart = PartNode.getPart(); }
            foreach (MeasureTreeNode node in MeasureNodes) { selections.SelectedMeasure.Add(node.getMeasure()); }
            foreach (ChordTreeNode node in ChordNodes) { selections.SelectedChord.Add(node.getChord()); }
            foreach (NoteTreeNode node in NoteNodes) { selections.SelectedNote.Add(node.getNote()); }
            foreach (EffectTreeNode node in EffectNodes) { selections.SelectedEffect.Add(node.getEffect()); }
        }

        public bool matchesType(ClickType desired) { return (desired == type); }

        public bool matchesSelection(Selection desired) { return (desired == selection); }

        public bool anyPartSelected() { return PartNode != null; }

        public bool anyMeasureSelected() { return MeasureNodes.Any(); }

        public bool multipleMeasuresSelected() { return MeasureNodes.Count() > 1; }

        public bool anyChordSelected() { return ChordNodes.Any(); }

        public bool multipleChordsSelected() { return ChordNodes.Count > 1; }

        public bool anyNoteSelected() { return NoteNodes.Any(); }

        public bool multipleNoteSelected() { return NoteNodes.Count > 1; }

        public bool anyEffectSelected() { return EffectNodes.Any(); }
    }
}
