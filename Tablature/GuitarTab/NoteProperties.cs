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
    public interface IPropertyMenu
    {
        void resetToDefault();
        void submitChanges();
    }

    public class NoteProperties : BaseValidator, IPropertyMenu
    {
        private Note note;
        private GuiCommandExecutor executor;
        private CommandSelections attributes;

        private int note_string;
        public string String
        {
            get { return note_string.ToString(); }
            set
            {
                if (ValidateProperty(nameof(String), value, validateString))
                {
                    SetProperty(ref note_string, int.Parse(value));
                }
            }
        }

        private int fret;
        public string Fret
        {
            get { return fret.ToString(); }
            set
            {
                if (ValidateProperty(nameof(Fret), value, validateFret))
                {
                    SetProperty(ref fret, int.Parse(value));
                }
            }
        }

        public EffectProperties IntoEffectProperties { get; }
        public EffectProperties StrikeEffectProperties { get; }
        public EffectProperties AfterEffectProperties { get; }

        public NoteProperties(Note n, GuiCommandExecutor ex, CommandSelections attr)
            :base()
        {
            note = n;
            executor = ex;
            attributes = attr;

            String = note.String.ToString();
            Fret = note.Fret.ToString();

            Note prev_note = attributes.SelectedPart?.getPreviousNote(note) ?? null;
            Note next_note = attributes.SelectedPart?.getNextNote(note) ?? null;
            IntoEffectProperties = new EffectProperties(executor, note.getEffectAtPosition(EffectPosition.Into), prev_note, note);
            StrikeEffectProperties = new EffectProperties(executor, note.getEffectAtPosition(EffectPosition.Strike), null, null);
            AfterEffectProperties = new EffectProperties(executor, note.getEffectAtPosition(EffectPosition.After), note, next_note);
        }

        public void resetToDefault()
        {
            Fret = note.Fret.ToString();
            String = note.String.ToString();
            IntoEffectProperties.resetToDefault();
            StrikeEffectProperties.resetToDefault();
            AfterEffectProperties.resetToDefault();
        }

        public void submitChanges()
        {
            if (fret != note.Fret) { executor.continueChangeNoteFret(fret); }

            if (note_string != note.String)
            {
                executor.executeChangeNoteStringFromMenu(note_string);
                //popup error if any effects tried to change
            }
            else
            {
                IntoEffectProperties.submitChanges();
                StrikeEffectProperties.submitChanges();
                AfterEffectProperties.submitChanges();
            }
        }

        public ICollection<string> validateFret(string new_fret)
        {
            var errors = new List<string>();
            int n_fret = -1;

            if (int.TryParse(new_fret, out n_fret))
            {
                if (n_fret < 0 || n_fret > 24) { errors.Add("must be between 0 and 24"); }
            }
            else { errors.Add("must be a number"); }
            return errors;
        }

        public ICollection<string> validateString(string new_string)
        {
            var errors = new List<string>();
            int str = 0;

            if (int.TryParse(new_string, out str))
            {
                if (str <= 0 || str > 6) { errors.Add("Must be between 1 and 6"); }
            }
            else { errors.Add("must be a number"); }
            return errors;
        }
    }
}
