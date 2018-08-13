using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    class ChordProperties : BasePropertyMenu
    {
        private Chord chord;

        private NoteLength current_length;
        public string CurrentLength
        {
            get { return current_length.getStringFromNoteLength(); }
            set { SetProperty(ref current_length, NoteLengthExtensions.getNoteLengthFromString(value)); }
        }

        public List<string> Lengths { get; }

        public ChordProperties(ChordTreeNode c, GuiCommandExecutor ex, NodeClick cl)
            :base(cl, ex)
        {
            chord = c.getChord();

            CurrentLength = chord.Length.NoteType.getStringFromNoteLength();
            Lengths = NoteLengthExtensions.getAllLengthStrings();
        }

        public override void resetToDefault()
        {
            CurrentLength = chord.Length.NoteType.getStringFromNoteLength();
        }

        public override void submitChanges()
        {
            if (current_length != chord.Length.NoteType) { executor.executeChangeChordLengthFromMenu(getClickCopy(), current_length); }
        }
    }
}
