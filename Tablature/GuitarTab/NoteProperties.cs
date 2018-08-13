using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class NoteProperties : BasePropertyMenu
    {
        public const int MIN_STRING = 0;
        public const int MAX_STRING = 6;
        public const int MIN_FRET = 0;
        public const int MAX_FRET = 26;

        private Note note;

        private string note_string;
        public string String
        {
            get { return note_string; }
            set
            {
                string error = StringError;
                setIntProperty(ref note_string, value, MIN_STRING, MAX_STRING, ref error);
                StringError = error;
            }
        }

        private string note_string_error;
        public string StringError
        {
            get { return note_string_error; }
            set { SetProperty(ref note_string_error, value); }
        }

        private string fret;
        public string Fret
        {
            get { return fret.ToString(); }
            set
            {
                string error = FretError;
                setIntProperty(ref fret, value, MIN_FRET, MAX_FRET, ref error);
                FretError = error;
            }
        }

        private string fret_error;
        public string FretError
        {
            get { return fret_error; }
            set { SetProperty(ref fret_error, value); }
        }

        public EffectProperties IntoEffectProperties { get; }
        public EffectProperties StrikeEffectProperties { get; }
        public EffectProperties AfterEffectProperties { get; }

        public NoteProperties(NoteTreeNode n, GuiCommandExecutor ex, NodeClick c)
            :base(c, ex)
        {
            note = n.getNote();

            String = note.String.ToString();
            Fret = note.Fret.ToString();

            NoteTreeNode prev_note = ref_click.PartNode?.getPreviousNote(n) ?? null;
            NoteTreeNode next_note = ref_click.PartNode?.getNextNote(n) ?? null;
            IntoEffectProperties = new EffectProperties(executor, ref_click, note.getEffectAtPosition(EffectPosition.Into), EffectPosition.Into, prev_note, n);
            StrikeEffectProperties = new EffectProperties(executor, ref_click, note.getEffectAtPosition(EffectPosition.Strike), EffectPosition.Strike, null, null);
            AfterEffectProperties = new EffectProperties(executor, ref_click, note.getEffectAtPosition(EffectPosition.After), EffectPosition.After, n, next_note);
        }

        public override void resetToDefault()
        {
            Fret = note.Fret.ToString();
            String = note.String.ToString();
            IntoEffectProperties.resetToDefault();
            StrikeEffectProperties.resetToDefault();
            AfterEffectProperties.resetToDefault();
        }

        public override void submitChanges()
        {
            if (StringError != String.Empty || FretError != String.Empty || !Int32.TryParse(fret, out int fret_i)
                || !Int32.TryParse(note_string, out int note_string_i)) { return; }

            if (fret_i != note.Fret) { executor.continueChangeNoteFret(getClickCopy(), fret_i); }

            if (note_string_i != note.String)
            {
                executor.executeChangeNoteStringFromMenu(getClickCopy(), note_string_i);
                //popup error if any effects tried to change
            }
            else
            {
                IntoEffectProperties.submitChanges();
                StrikeEffectProperties.submitChanges();
                AfterEffectProperties.submitChanges();
            }
        }
    }
}
