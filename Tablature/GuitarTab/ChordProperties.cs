using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    class ChordProperties : BaseValidator, IPropertyMenu
    {
        private Chord chord;
        private GuiCommandExecutor executor;
        private NodeClick click;

        private NoteLength current_length;
        public string CurrentLength
        {
            get { return current_length.getStringFromNoteLength(); }
            set { SetProperty(ref current_length, NoteLengthExtensions.getNoteLengthFromString(value)); }
        }

        public List<string> Lengths { get; }

        public ChordProperties(Chord c, GuiCommandExecutor ex, NodeClick cl)
        {
            chord = c;
            executor = ex;
            click = cl;

            CurrentLength = chord.Length.NoteType.getStringFromNoteLength();
            Lengths = NoteLengthExtensions.getAllLengthStrings();
        }

        public void resetToDefault()
        {
            CurrentLength = chord.Length.NoteType.getStringFromNoteLength();
        }

        public void submitChanges()
        {
            if (current_length != chord.Length.NoteType) { executor.executeChangeChordLengthFromMenu(click, new Length(current_length)); }
        }
    }
}
