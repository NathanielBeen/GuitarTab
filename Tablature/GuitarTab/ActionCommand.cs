using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IActionCommand
    {
        void executeAction();
    }

    public class MultipleActionCommand : IActionCommand
    {
        private List<IActionCommand> commands;

        public MultipleActionCommand()
        {
            commands = new List<IActionCommand>();
        }

        public void AddCommand(IActionCommand command) { commands.Add(command); }

        public void executeAction()
        {
            foreach (IActionCommand command in commands) { command.executeAction(); }
        }
    }

    public class AddChordToMeasureCom : IActionCommand
    {
        private Measure measure;
        private Chord chord;

        public AddChordToMeasureCom(Measure m, Chord c)
        {
            measure = m;
            chord = c;
        }

        public void executeAction()
        {
            measure.ModelCollection.performActionOnSpecificItems(
                c => c.Position.Index == chord.Position.Index,
                c => c.breakEffectsAtPosition(EffectPosition.Into));

            measure.ModelCollection.performActionOnSpecificItems(
                c => c.Position.Index >= chord.Position.Index,
                c => c.Position.Index += 1);

            measure.Add(chord);
            (chord.Position as MultiPosition)?.setMeasureReference(measure.Position);
        }
    }

    public class AddNoteToChordCom : IActionCommand
    {
        private NoteChord chord;
        private Note note;

        public AddNoteToChordCom(NoteChord c, Note n)
        {
            chord = c;
            note = n;
        }

        public void executeAction()
        {
            chord.Add(note);
        }
    }

    public class RemoveNoteFromChordCom : IActionCommand
    {
        private NoteChord chord;
        private Note note;

        public RemoveNoteFromChordCom(NoteChord c, Note n)
        {
            chord = c;
            note = n;
        }

        public void executeAction()
        {
            note.removeMultiEffects();
            chord.Remove(note);
        }
    }

    public class RemoveChordFromMeasureCom : IActionCommand
    {
        private Measure measure;
        private Chord chord;

        public RemoveChordFromMeasureCom(Measure m, Chord c)
        {
            measure = m;
            chord = c;
        }

        public void executeAction()
        {
            chord.breakMultiEffects();

            measure.Remove(chord);
            measure.ModelCollection.performActionOnSpecificItems(
                c => c.Position.Index > chord.Position.Index,
                c => c.Position.Index -= 1);
        }
    }

    public class ChangeChordLengthCom : IActionCommand
    {
        private Chord chord;
        private Length length;

        public ChangeChordLengthCom(Chord c, Length l)
        {
            chord = c;
            length = l;
        }

        public void executeAction() { chord.Length = length; }
    }

    public class ChangeChordPositionCom : IActionCommand
    {
        private Chord chord;
        private int position;

        public ChangeChordPositionCom(Chord c, int p)
        {
            chord = c;
            position = p;
        }

        public void executeAction() { chord.Position.Index = position; }
    }

    public class ChangeNoteStringCom : IActionCommand
    {
        private Note note;
        private int new_string;

        public ChangeNoteStringCom(Note n, int ns)
        {
            note = n;
            new_string = ns;
        }

        public void executeAction()
        {
            note.removeMultiEffects();
            note.String = new_string;
        }
    }

    public class ChangeNoteFretCom : IActionCommand
    {
        private Note note;
        private int fret;

        public ChangeNoteFretCom(Note n, int f)
        {
            note = n;
            fret = f;
        }

        public void executeAction()
        {
            if (note.getEffectAtPosition(EffectPosition.Into) is Tie) { note.removeEffectAtPosition(EffectPosition.Into); }
            if (note.getEffectAtPosition(EffectPosition.After) is Tie) { note.removeEffectAtPosition(EffectPosition.After); }

            note.Fret = fret;
        }
    }

    public class AddPalmMuteEffectCom : IActionCommand
    {
        private NoteChord chord;
        private IEffect palm_mute;

        public AddPalmMuteEffectCom(NoteChord c, IEffect pm)
        {
            chord = c;
            palm_mute = pm;
        }

        public void executeAction()
        {
            chord.ModelCollection.performActionOnAllItems(n => n.Add(palm_mute));
        }
    }

    public class AddSingleNoteEffectCom : IActionCommand
    {
        private Note note;
        private IEffect effect;

        public AddSingleNoteEffectCom(Note n, IEffect e)
        {
            note = n;
            effect = e;
        }

        public void executeAction()
        {
            note.Add(effect);
        }
    }

    public class RemoveNoteEffectCom : IActionCommand
    {
        private Note note;
        private IEffect effect;

        public RemoveNoteEffectCom(Note n, IEffect e)
        {
            note = n;
            effect = e;
        }

        public void executeAction()
        {
            note.Remove(effect);
        }
    }

    public class AddMultiNoteEffectCom : IActionCommand
    {
        private IMultiEffect effect;

        public AddMultiNoteEffectCom(IMultiEffect e)
        {
            effect = e;
        }

        public void executeAction()
        {
            effect.First.Add(effect);
            effect.Second.Add(effect);
        }
    }

    public class AddMeasureToPartCom : IActionCommand
    {
        private Part part;
        private Measure measure;

        public AddMeasureToPartCom(Part p, Measure m)
        {
            part = p;
            measure = m;
        }

        public void executeAction()
        {
            part.ModelCollection.performActionOnSpecificItems(
                m => m.Position.Index == measure.Position.Index,
                m => m.breakEffectsWithPrevMeasure());

            part.ModelCollection.performActionOnSpecificItems(
                m => m.Position.Index >= measure.Position.Index,
                m => m.Position.Index += 1);

            part.Add(measure);
        }
    }

    public class RemoveMeasureFromPartCom : IActionCommand
    {
        private Part part;
        private Measure measure;

        public RemoveMeasureFromPartCom(Part p, Measure m)
        {
            part = p;
            measure = m;
        }

        public void executeAction()
        {
            measure.breakCrossMeasureEffects();

            part.ModelCollection.performActionOnSpecificItems(
                m => m.Position.Index > measure.Position.Index,
                m => m.Position.Index -= 1);
            part.Remove(measure);
        }
    }

    public class ChangeMeasureTimeSigCom : IActionCommand
    {
        private Part part;
        private Measure measure;
        private int num_beats;
        private NoteLength beat_type;

        public ChangeMeasureTimeSigCom(Part p, Measure m, int n, NoteLength b)
        {
            part = p;
            measure = m;
            num_beats = n;
            beat_type = b;
        }

        public void executeAction()
        {
            measure.TimeSignature.setTimeSignature(num_beats, beat_type);
            measure.MatchesPart = (part.TimeSignature.matchesSignature(measure.TimeSignature) && part.DefaultBPM == measure.Bpm);
        }
    }

    public class ChangeMeasureBpmCom : IActionCommand
    {
        private Part part;
        private Measure measure;
        private int bpm;

        public ChangeMeasureBpmCom(Part p, Measure m, int b)
        {
            part = p;
            measure = m;
            bpm = b;
        }

        public void executeAction()
        {
            measure.Bpm = bpm;
            measure.MatchesPart = (part.TimeSignature.matchesSignature(measure.TimeSignature) && part.DefaultBPM == measure.Bpm);
        }
    }

    public class ChangeMeasurePositionCom : IActionCommand
    {
        private Measure measure;
        private int position;

        public ChangeMeasurePositionCom(Measure m, int p)
        {
            measure = m;
            position = p;
        }

        public void executeAction()
        {
            measure.Position.Index = position;
        }
    }
}
