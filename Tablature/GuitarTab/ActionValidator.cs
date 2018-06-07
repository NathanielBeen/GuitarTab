using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IActionValidator
    {
        bool validateAction();
    }

    public class MultipleActionValidator : IActionValidator
    {
        private List<IActionValidator> validators;

        public MultipleActionValidator()
        {
            validators = new List<IActionValidator>();
        }

        public void AddValidator(IActionValidator validator) { validators.Add(validator); }

        public bool validateAction()
        {
            foreach (IActionValidator validator in validators)
            {
                if (!validator.validateAction()) { return false; }
            }
            return true;
        }
    }

    public class InitPartVal : IActionValidator
    {
        public InitPartVal() { }

        public bool validateAction() { return true; }
    }

    public class AddChordToMeasureVal : IActionValidator
    {
        private Chord chord;
        private Measure measure;

        public AddChordToMeasureVal(Chord c, Measure m)
        {
            chord = c;
            measure = m;
        }

        public bool validateAction()
        {
            if (chord is null || measure is null) { return false; }
            if (chord.Position.Index < 0 || chord.Position.Index > measure.getLastChordPosition() + 1) { return false; }
            if (measure.getSpaceTaken() + chord.Length.getLength() > measure.getTotalSpace()) { return false; }
            return true;
        }
    }

    public class AddChordToPartVal : IActionValidator
    {
        private Part part;
        private Measure measure;
        private Chord chord;

        public AddChordToPartVal(Part p, Measure m, Chord c)
        {
            part = p;
            measure = m;
            chord = c;
        }

        public bool validateAction()
        {
            if (part == null || measure == null || chord == null) { return false; }
            if (chord.Length.getLength() >= measure.getTotalSpace()) { return false; }
            return true;
        }
    }

    public class AddNoteToChordVal : IActionValidator
    {
        private NoteChord chord;
        private Note note;

        public AddNoteToChordVal(NoteChord c, Note n)
        {
            chord = c;
            note = n;
        }

        public bool validateAction()
        {
            if (chord is null || note is null) { return false; }
            foreach (Note n in chord.ModelCollection.Items())
            {
                if (n.String == note.String) { return false; }
            }
            return true;
        }
    }

    public class AddNoteToPartVal : IActionValidator
    {
        private Part part;
        private Measure measure;
        private NoteChord chord;
        private Note note;

        public AddNoteToPartVal(Part p, Measure m, NoteChord c, Note n)
        {
            part = p;
            measure = m;
            chord = c;
            note = n;
        }

        public bool validateAction()
        {
            if (part == null || measure == null || chord == null || note == null) { return false; }
            if (chord.Length.getLength() > measure.getTotalSpace()) { return false; }
            return true;
        }
    }

    public class RemoveNoteFromChordVal : IActionValidator
    {
        private Measure measure;
        private NoteChord chord;
        private Note note;

        public RemoveNoteFromChordVal(Measure m, NoteChord c, Note n)
        {
            measure = m;
            chord = c;
            note = n;
        }

        public bool validateAction()
        {
            if (measure is null || chord is null || note is null) { return false; }
            if (!chord.ModelCollection.Contains(note)) { return false; }
            return true;
        }
    }

    public class RemoveChordFromMeasureVal : IActionValidator
    {
        private Measure measure;
        private Chord chord;

        public RemoveChordFromMeasureVal(Measure m, Chord c)
        {
            measure = m;
            chord = c;
        }

        public bool validateAction()
        {
            if (measure is null || chord is null) { return false; }
            if (!measure.ModelCollection.Contains(chord)) { return false; }
            return true;
        }
    }

    public class ChangeNotePositionVal : IActionValidator
    {
        private Measure measure;
        private NoteChord first_chord;
        private NoteChord second_chord;
        private Note note;
        private int? new_string;

        public ChangeNotePositionVal(Measure m, NoteChord f, NoteChord s, Note n, int? ns)
        {
            measure = m;
            first_chord = f;
            second_chord = s;
            note = n;
            new_string = ns;
        }

        public bool validateAction()
        {
            if (measure is null || first_chord is null || second_chord is null || note is null || new_string is null) { return false; }
            if (!measure.ModelCollection.Contains(first_chord)) { return false; }
            if (!first_chord.ModelCollection.Contains(note)) { return false; }
            foreach (Note note in second_chord.ModelCollection.Items())
            {
                if (note.String == new_string) { return false; }
            }
            return true;
        }
    }

    public class ChangeNotePositionNewChordVal : IActionValidator
    {
        private Measure first_measure;
        private Measure second_measure;
        private NoteChord first_chord;
        private NoteChord second_chord;
        private Note note;
        private int? note_string;

        public ChangeNotePositionNewChordVal(Measure fm, Measure sm, NoteChord fc, NoteChord sc, Note n, int? ns)
        {
            first_measure = fm;
            second_measure = sm;
            first_chord = fc;
            second_chord = sc;
            note = n;
            note_string = ns;
        }

        public bool validateAction()
        {
            if (first_measure == null || second_measure == null || first_chord == null || second_chord == null || note == null || note_string == null) { return false; }
            if (!first_measure.ModelCollection.Contains(first_chord)) { return false; }
            if (!first_chord.ModelCollection.Contains(note)) { return false; }
            if (second_chord.Position.Index < 0 || second_chord.Position.Index > second_measure.getLastChordPosition()+1) { return false; }
            if (first_chord.ModelCollection.Count() > 1 || !(first_measure.Equals(second_measure)))
            {
                if (second_measure.getSpaceTaken() + second_chord.Length.getLength() > second_measure.getTotalSpace()) { return false; }
            }

            return true;
        }
    }

    public class ChangeNotePositionNewMeasureVal : IActionValidator
    {
        private Measure first_measure;
        private Measure second_measure;
        private NoteChord first_chord;
        private NoteChord second_chord;
        private Note note;
        private int? note_string;

        public ChangeNotePositionNewMeasureVal(Measure fm, Measure sm, NoteChord fc, NoteChord sc, Note n, int? ns)
        {
            first_measure = fm;
            second_measure = sm;
            first_chord = fc;
            second_chord = sc;
            note = n;
            note_string = ns;
        }

        public bool validateAction()
        {
            if (first_measure == null || second_measure == null || first_chord == null || second_chord == null || note == null || note_string == null) { return false; }
            if (!first_measure.ModelCollection.Contains(first_chord)) { return false; }
            if (!first_chord.ModelCollection.Contains(note)) { return false; }
            if (second_chord.Position.Index < 0 || second_chord.Position.Index > second_measure.getLastChordPosition()) { return false; }
            if (second_measure.getSpaceTaken() + second_chord.Length.getLength() > second_measure.getTotalSpace()) { return false; }

            return true;
        }
    }

    public class ChangeChordPositionVal : IActionValidator
    {
        private Chord chord;
        private Measure first_measure;
        private Measure second_measure;
        private int? position;

        public ChangeChordPositionVal(Chord c, Measure f, Measure s, int? p)
        {
            chord = c;
            first_measure = f;
            second_measure = s;
            position = p;
        }

        public bool validateAction()
        {
            if (chord is null || first_measure is null || second_measure is null || position == null) { return false; }
            if (first_measure.Equals(second_measure))
            {
                if (chord.Position.Index == position) { return false; }
                if (position > second_measure.getLastChordPosition()) { return false; }
            }
            else
            {
                if (position > second_measure.getLastChordPosition() + 1) { return false; }
                if (second_measure.getSpaceTaken() + chord.Length.getLength() > second_measure.getTotalSpace()) { return false; }

            }
            return true;
        }
    }

    public class ChangeMultipleChordPositionVal : IActionValidator
    {
        private Measure home_measure;
        private Measure dest_measure;
        private List<Chord> chords;
        private int? position;

        public ChangeMultipleChordPositionVal(Measure h, Measure d, List<Chord> c, int? p)
        {
            home_measure = h;
            dest_measure = d;
            chords = c;
            position = p;
        }

        public bool validateAction()
        {
            if (home_measure is null || dest_measure is null || chords is null || !chords.Any() || position is null) { return false; }
            foreach (Chord chord in chords)
            {
                if (!home_measure.ModelCollection.Contains(chord)){ return false; }
            }

            if (position > dest_measure.getLastChordPosition() + 1) { return false; }
            if (getNewSpaceTaken() > dest_measure.getTotalSpace()) { return false; }

            return true;
        }

        public int getNewSpaceTaken()
        {
            int length = dest_measure.getSpaceTaken();
            foreach (Chord chord in chords) { length += chord.Length.getLength(); }
            return length;
        }
    }

    public class ChangeChordLengthVal : IActionValidator
    {
        private Measure measure;
        private Chord chord;
        private Length length;

        public ChangeChordLengthVal(Measure m, Chord c, Length l)
        {
            measure = m;
            chord = c;
            length = l;
        }

        public bool validateAction()
        {
            if (measure is null || chord is null || length is null) { return false; }
            if (length is TupleLength && !validateTuple(length as TupleLength)) { return false; }

            if (measure.getSpaceTaken() - chord.Length.getLength() + length.getLength() > measure.getTotalSpace()) { return false; }
            return true;
        }

        public bool validateTuple(TupleLength length)
        {
            return (length.TupleType != TupletType.None && length.NoteType != NoteLength.None);
        }
    }

    public class ChangeMultipleChordLengthVal : IActionValidator
    {
        private Measure measure;
        private List<Chord> chords;
        private Length length;

        public ChangeMultipleChordLengthVal(Measure m, List<Chord> c, Length l)
        {
            measure = m;
            chords = c;
            length = l;
        }

        public bool validateAction()
        {
            if (measure is null || chords is null || !chords.Any() || length is null) { return false; }
            if (length is TupleLength && !validateTuple(length as TupleLength)) { return false; }
            foreach (Chord chord in chords)
            {
                if (!measure.ModelCollection.Contains(chord)) { return false; }
            }
   
            if (getSpaceTaken() > measure.getTotalSpace()) { return false; }

            return true;
        }

        public int getSpaceTaken()
        {
            int curr_length = measure.getSpaceTaken();
            foreach (Chord chord in chords)
            {
                curr_length += length.getLength() - chord.Length.getLength();
            }
            return curr_length;
        }

        public bool validateTuple(TupleLength length)
        {
            return (length.TupleType != TupletType.None && length.NoteType != NoteLength.None);
        }
    }

    public class ChangeNoteStringVal : IActionValidator
    {
        private NoteChord chord;
        private Note note;
        private int? new_string;

        public ChangeNoteStringVal(NoteChord c, Note n, int? s)
        {
            chord = c;
            note = n;
            new_string = s;
        }

        public bool validateAction()
        {
            if (chord == null || note == null || new_string == null) { return false; }
            if (note.String == new_string) { return false; }
            foreach (Note n in chord.ModelCollection.Items())
            {
                if (n != note && n.String == new_string) { return false; }
            }
            return true;
        }
    }

    public class ChangeNoteFretVal : IActionValidator
    {
        private Note note;
        private int? fret;

        public ChangeNoteFretVal(Note n, int? f)
        {
            note = n;
            fret = f;
        }

        public bool validateAction()
        {
            if (note is null || fret is null) { return false; }
            if (note.Fret == fret) { return false; }
            return true;
        }
    }

    public class AddPalmMuteEffectVal : IActionValidator
    {
        private NoteChord chord;
        private IEffect palm_mute;

        public AddPalmMuteEffectVal(NoteChord c, IEffect p)
        {
            chord = c;
            palm_mute = p;
        }

        public bool validateAction()
        {
            return !(chord is null);
        }
    }

    public class AddSingleNoteEffectVal : IActionValidator
    {
        private Note note;
        private IEffect effect;

        public AddSingleNoteEffectVal(Note n, IEffect e)
        {
            note = n;
            effect = e;
        }

        public bool validateAction()
        {
            if (note is null || effect is null) { return false; }
            return true;
        }
    }

    public class RemoveNoteEffectVal : IActionValidator
    {
        private Note note;
        private IEffect effect;

        public RemoveNoteEffectVal(Note n, IEffect e)
        {
            note = n;
            effect = e;
        }

        public bool validateAction()
        {
            if (note == null || effect == null) { return false; }
            if (!note.ModelCollection.Contains(effect)) { return false; }
            return true;
        }
    }

    public class AddMultiNoteEffectVal : IActionValidator
    {
        private IMultiEffect effect;
        private Note first;
        private Note second;

        public AddMultiNoteEffectVal(IMultiEffect e)
        {
            effect = e;
            first = e.First;
            second = e.Second;
        }

        public bool validateAction()
        {
            if (effect is null || first is null || second is null) { return false; }
            if (first.Equals(second)) { return false; }
            if (first.String != second.String) { return false; }
            return first.Position.occursDirectlyBefore(second.Position);
        }
    }

    public class AddMeasureToPartVal : IActionValidator
    {
        private Part part;
        private Measure measure;

        public AddMeasureToPartVal(Part p, Measure m)
        {
            part = p;
            measure = m;
        }

        public bool validateAction()
        {
            if (part == null || measure == null) { return false; }
            return (measure.Position.Index >= 0 && measure.Position.Index <= part.getLastMeasurePosition() + 1);
        }
    }

    public class RemoveMeasureFromPartVal : IActionValidator
    {
        private Part part;
        private Measure measure;

        public RemoveMeasureFromPartVal(Part p, Measure m)
        {
            part = p;
            measure = m;
        }

        public bool validateAction()
        {
            if (part == null || measure == null) { return false; }
            return part.ModelCollection.Contains(measure);
        }
    }

    public class ChangeMeasurePositionVal : IActionValidator
    {
        private Part part;
        private Measure measure;
        private int? position;

        public ChangeMeasurePositionVal(Part p, Measure m, int? po)
        {
            part = p;
            measure = m;
            position = po;
        }

        public bool validateAction()
        {
            if (part == null || measure == null || position == null) { return false; }
            if (!part.ModelCollection.Contains(measure)) { return false; }
            if (measure.Position.Index == position) { return false; }
            if (position < 0 || position > part.getLastMeasurePosition()) { return false; }
            return true;
        }
    }

    public class ChangeMultipleMeasurePositionVal : IActionValidator
    {
        private Part part;
        private List<Measure> measures;
        private int? position;

        public ChangeMultipleMeasurePositionVal(Part p, List<Measure> m, int? po)
        {
            part = p;
            measures = m;
            position = po;
        }

        public bool validateAction()
        {
            if (part == null || measures == null || !measures.Any() || position == null) { return false; }
            foreach (Measure measure in measures)
            {
                if (!part.ModelCollection.Contains(measure)) { return false; }
            }
            if (position < 0 || position > part.ModelCollection.Count() - measures.Count()) { return false; }
            return true;
        }
    }

    public class ChangeMeasureTimeSigVal : IActionValidator
    {
        private Part part;
        private Measure measure;
        private int? num_beats;
        private NoteLength? beat_type;

        public ChangeMeasureTimeSigVal(Part p, Measure m, int? n, NoteLength? b)
        {
            part = p;
            measure = m;
            num_beats = n;
            beat_type = b;
        }

        public bool validateAction()
        {
            if (part == null || measure == null || num_beats == null || beat_type == null) { return false; }
            if (!part.ModelCollection.Contains(measure)) { return false; }
            return (TimeSignature.canSetTimeSignature((int)num_beats, (NoteLength)beat_type));
        }
    }

    public class ChangeMeasureBpmVal : IActionValidator
    {
        private Part part;
        private Measure measure;
        private int? bpm;

        public ChangeMeasureBpmVal(Part p, Measure m, int? b)
        {
            part = p;
            measure = m;
            bpm = b;
        }

        public bool validateAction()
        {
            if (part == null || measure == null || bpm == null) { return false; }
            if (!part.ModelCollection.Contains(measure)) { return false; }
            if (bpm < 1) { return false; }
            return true;
        }
    }

    public class CreateTupletFromNotesVal : IActionValidator
    {
        private Measure measure;
        private List<Chord> chords;
        private TupletType type;
        private List<Length> new_lengths;

        public CreateTupletFromNotesVal(Measure m, List<Chord> c, TupletType t, List<Length> n)
        {
            measure = m;
            chords = c;
            type = t;
            new_lengths = n;
        }

        public bool validateAction()
        {
            if (measure == null || chords == null || chords.Count <= 1 || new_lengths == null || !new_lengths.Any() || type == TupletType.None ) { return false; }
            if (chords.Count != new_lengths.Count) { return false; }
            foreach (Chord chord in chords)
            {
                if (!measure.ModelCollection.Contains(chord)) { return false; }
            }
            if (!canCreateTuple()) { return false; }
            if (measure.getSpaceTaken() - getPreivousLength() + getNextLength() > measure.getTotalSpace()) { return false; }
            return true;
        }

        public bool canCreateTuple()
        {
            NoteLength base_length = NoteLength.None;
            foreach (Chord chord in chords)
            {
                NoteLength prop = getHighestValidTupleBaseLength(chord.Length.NoteType);
                if (base_length == NoteLength.None || prop < base_length) { base_length = prop; }
            }

            int weight = 0;
            foreach (Chord chord in chords)
            {
                weight += getWeightFromBaseLength(chord.Length.NoteType, base_length);
            }
            return (weight == (int)type && base_length != NoteLength.None);
        }

        public NoteLength getHighestValidTupleBaseLength(NoteLength length)
        {
            switch (length)
            {
                case NoteLength.DottedWhole:
                    return NoteLength.Half;
                case NoteLength.DottedHalf:
                    return NoteLength.Quarter;
                case NoteLength.DottedQuarter:
                    return NoteLength.Eighth;
                case NoteLength.DottedEighth:
                    return NoteLength.Sixeteenth;
                case NoteLength.DottedSixeteenth:
                    return NoteLength.ThirtySecond;
                default:
                    return length;
            }
        }

        public int getWeightFromBaseLength(NoteLength note_length, NoteLength base_length) { return (int)note_length / (int)base_length; }

        private int getPreivousLength()
        {
            int length = 0;
            foreach (Chord chord in chords) { length += chord.Length.getLength(); }
            return length;
        }

        private int getNextLength()
        {
            int length = 0;
            foreach (Length l in new_lengths) { length += l.getLength(); }
            return length;
        }
    }
}
