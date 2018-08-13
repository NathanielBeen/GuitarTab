using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IActionAttributes
    {

    }

    public class InitPartAtr : IActionAttributes
    {
        public Part Part { get; }

        public InitPartAtr(CommandSelections selection)
        {
            Part = genPart(selection);
        }

        public Part genPart(CommandSelections selection)
        {
            Part part = Part.createInstance(SongInfo.createDefault(), InstrumentInfo.createDefault(), selection.BPM, selection.NumBeats, selection.BeatType);
            selection.SelectedPart = part;
            return part;
        }
    }

    public class AddNoteChordToMeasureAtr : IActionAttributes
    {
        public Chord Chord { get; }
        public Measure Measure { get; }

        public AddNoteChordToMeasureAtr(CommandSelections selection)
        {
            Measure = selection.SelectedMeasure.FirstOrDefault();
            Chord = genNoteChord(selection);
        }

        public Chord genNoteChord(CommandSelections selection)
        {
            var length = Length.createInstance(selection.SelectedLength, selection.TupletType);
            return NoteChord.createInstance(selection.Position, Measure?.Position, length);
        }
    }

    public class AddRestChordToMeasureAtr : IActionAttributes
    {
        public Chord Chord { get; }
        public Measure Measure { get; }

        public AddRestChordToMeasureAtr(CommandSelections selection)
        {
            Measure = selection.SelectedMeasure.FirstOrDefault();
            Chord = genChord(selection);
        }

        public Chord genChord(CommandSelections selection)
        {
            var length = Length.createInstance(selection.SelectedLength, selection.TupletType);
            return Chord.createInstance(selection.Position, Measure?.Position, length);
        }
    }

    public class AddRestChordToPartAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure Measure { get; }
        public Chord Chord { get; }

        public AddRestChordToPartAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measure = genMeasure(selection);
            Chord = genChord(selection);
        }

        public Measure genMeasure(CommandSelections selection)
        {
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, selection.Position);
        }

        public Chord genChord(CommandSelections selection)
        {
            var length = Length.createInstance(selection.SelectedLength, selection.TupletType);
            return Chord.createInstance(0, Measure?.Position, length);
        }
    }

    public class AddNoteToChordAtr : IActionAttributes
    {
        public NoteChord Chord { get; }
        public Note Note { get; }

        public AddNoteToChordAtr(CommandSelections selection)
        {
            Chord = selection.SelectedChord.FirstOrDefault() as NoteChord;
            Note = genNote(selection);
        }

        public Note genNote(CommandSelections selection)
        {
            return Note.createInstance(selection.Fret, selection.String, Chord?.Position as MultiPosition, Chord?.Length);
        }
    }

    public class AddNoteToMeasureAtr : IActionAttributes
    {
        public Measure Measure { get; }
        public NoteChord Chord { get; }
        public Note Note { get; }

        public AddNoteToMeasureAtr(CommandSelections selection)
        {
            Measure = selection.SelectedMeasure.FirstOrDefault();
            Chord = genChord(selection);
            Note = genNote(selection);
        }

        public NoteChord genChord(CommandSelections selection)
        {
            var length = Length.createInstance(selection.SelectedLength, selection.TupletType);
            return NoteChord.createInstance(selection.Position, Measure?.Position, length);
        }

        public Note genNote(CommandSelections selection)
        {
            return Note.createInstance(selection.Fret, selection.String, Chord?.Position as MultiPosition, Chord?.Length);
        }
    }

    public class AddNoteToPartAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure Measure { get; }
        public NoteChord Chord { get; }
        public Note Note { get; }

        public AddNoteToPartAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measure = genMeasure(selection);
            Chord = genChord(selection);
            Note = genNote(selection);
        }

        public Measure genMeasure(CommandSelections selection)
        {
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, selection.Position);
        }

        public NoteChord genChord(CommandSelections selection)
        {
            var length = Length.createInstance(selection.SelectedLength, selection.TupletType);
            return NoteChord.createInstance(0, Measure?.Position, length);
        }

        public Note genNote(CommandSelections selection)
        {
            return Note.createInstance(selection.Fret, selection.String, Chord?.Position as MultiPosition, Chord?.Length);
        }
    }

    public class RemoveNoteFromChordAtr : IActionAttributes
    {
        public Note Note { get; }
        public NoteChord Chord { get; }
        public Measure Measure { get; }

        public RemoveNoteFromChordAtr(CommandSelections selection)
        {
            Chord = selection.SelectedChord.FirstOrDefault() as NoteChord;
            Note = selection.SelectedNote.FirstOrDefault();
            Measure = selection.SelectedMeasure.FirstOrDefault();
        }
    }

    public class RemoveMultipleNotesFromChordAtr : IActionAttributes
    {
        public List<Note> Notes { get; }
        public NoteChord Chord { get; }
        public Measure Measure { get; }

        public RemoveMultipleNotesFromChordAtr(CommandSelections selection)
        {
            Notes = selection.SelectedNote;
            Chord = selection.SelectedChord.FirstOrDefault() as NoteChord;
            Measure = selection.SelectedMeasure.FirstOrDefault();
        }
    }

    public class RemoveChordFromMeasureAtr : IActionAttributes
    {
        public Chord Chord { get; }
        public Measure Measure { get; }

        public RemoveChordFromMeasureAtr(CommandSelections selection)
        {
            Chord = selection.SelectedChord.FirstOrDefault();
            Measure = selection.SelectedMeasure.FirstOrDefault();
        }
    }

    public class RemoveMultipleChordsFromMeasureAtr : IActionAttributes
    {
        public List<Chord> Chords { get; }
        public Measure Measure { get; }

        public RemoveMultipleChordsFromMeasureAtr(CommandSelections selection)
        {
            Chords = selection.SelectedChord;
            Measure = selection.SelectedMeasure.FirstOrDefault();
        }
    }

    public class ChangeNotePositionAtr : IActionAttributes
    {
        public Measure Measure { get; }
        public NoteChord FirstChord { get; }
        public NoteChord SecondChord { get; }
        public Note Note { get; }
        public int? NoteString { get; }
        
        public ChangeNotePositionAtr(CommandSelections selection)
        {
            Measure = selection.SelectedMeasure.FirstOrDefault();
            FirstChord = selection.SelectedChord[0] as NoteChord;
            SecondChord = selection.SelectedChord[1] as NoteChord;
            Note = selection.SelectedNote.FirstOrDefault();
            NoteString = selection.String;
        }
    }

    public class ChangeNotePositionNewChordAtr : IActionAttributes
    {
        public Measure FirstMeasure { get; }
        public Measure SecondMeasure { get; }
        public NoteChord FirstChord { get; }
        public NoteChord SecondChord { get; }
        public Note Note { get; }
        public int? NoteString { get; }

        public ChangeNotePositionNewChordAtr(CommandSelections selection)
        {
            FirstMeasure = selection.SelectedMeasure[0];
            SecondMeasure = selection.SelectedMeasure[1];
            FirstChord = selection.SelectedChord.FirstOrDefault() as NoteChord;
            SecondChord = genChord(selection);
            Note = selection.SelectedNote.FirstOrDefault();
            NoteString = selection.String;
        }

        public NoteChord genChord(CommandSelections selection)
        {
            return NoteChord.createInstance(selection.Position, SecondMeasure?.Position, FirstChord?.Length);
        }
    }

    public class ChangeNotePositionNewMeasureAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure FirstMeasure { get; }
        public Measure SecondMeasure { get; }
        public NoteChord FirstChord { get; }
        public NoteChord SecondChord { get; }
        public Note Note { get; }
        public int? NoteString { get; }

        public ChangeNotePositionNewMeasureAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            FirstMeasure = selection.SelectedMeasure.FirstOrDefault();
            SecondMeasure = genMeasure(selection);
            FirstChord = selection.SelectedChord.FirstOrDefault() as NoteChord;
            SecondChord = genChord(selection);
            Note = selection.SelectedNote.FirstOrDefault();
            NoteString = selection.String;
        }

        public Measure genMeasure(CommandSelections selection)
        {
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, selection.Position);
        }

        public NoteChord genChord(CommandSelections selection)
        {
            return NoteChord.createInstance(0, SecondMeasure?.Position, FirstChord?.Length);
        }
    }

    public class ChangeChordPositionAtr : IActionAttributes
    {
        public Chord Chord { get; }
        public Measure FirstMeasure { get; }
        public Measure SecondMeasure { get; }
        public int? Position { get; }

        public ChangeChordPositionAtr(CommandSelections selection)
        {
            Chord = selection.SelectedChord.FirstOrDefault();
            FirstMeasure = selection.SelectedMeasure[0];
            SecondMeasure = selection.SelectedMeasure[1];
            Position = selection.Position;
            if (Position < Chord.Position.Index) { Position++; }
        }
    }

    public class ChangeChordPositionNewMeasureAtr : IActionAttributes
    {
        public Part Part { get; }
        public Chord Chord { get; }
        public Measure FirstMeasure { get; }
        public Measure SecondMeasure { get; }
        public int Position { get; }

        public ChangeChordPositionNewMeasureAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Chord = selection.SelectedChord.FirstOrDefault();
            FirstMeasure = selection.SelectedMeasure.FirstOrDefault();
            SecondMeasure = genSecondMeasure(selection);
            Position = 0;
        }

        public Measure genSecondMeasure(CommandSelections selection)
        {
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, selection.Position);
        }
    }

    public class ChangeMultipleChordPositionAtr : IActionAttributes
    {
        public Measure FirstMeasure { get; }
        public Measure SecondMeasure { get; }
        public List<Chord> Chords { get; }
        public int? Position { get; }

        public ChangeMultipleChordPositionAtr(CommandSelections selection)
        {
            FirstMeasure = selection.SelectedMeasure.FirstOrDefault();
            SecondMeasure = selection.SelectedMeasure.LastOrDefault();
            Chords = selection.SelectedChord;
            Position = genCorrectedPosition(selection);
        }

        public int? genCorrectedPosition(CommandSelections selection)
        {
            int init = (int)selection.Position;
            foreach (Chord chord in Chords)
            {
                if (FirstMeasure.Equals(SecondMeasure) && (chord.Position.Index == init || chord.Position.Index + 1 == init)) { return null; }
            }

            int first_position = (from chord in Chords select chord.Position.Index).Min();
            if (init > first_position)
            {
                foreach (Chord chord in Chords)
                {
                    if (SecondMeasure.ModelCollection.Contains(chord)) { init -= 1; }
                }
            }
            return init;
        }
    }

    public class ChangeMultipleChordPositionNewMeasureAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure FirstMeasure { get; }
        public Measure SecondMeasure { get; }
        public List<Chord> Chords { get; }
        public int Position { get; }

        public ChangeMultipleChordPositionNewMeasureAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            FirstMeasure = selection.SelectedMeasure.FirstOrDefault();
            SecondMeasure = genSecondMeasure(selection);
            Chords = selection.SelectedChord;
            Position = 0;
        }

        public Measure genSecondMeasure(CommandSelections selection)
        {
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, selection.Position);
        }
    }

    public class ChangeChordLengthAtr : IActionAttributes
    {
        public Measure Measure { get; }
        public Chord Chord { get; }
        public Length Length { get; }

        public ChangeChordLengthAtr(CommandSelections selection)
        {
            Measure = selection.SelectedMeasure.FirstOrDefault();
            Chord = selection.SelectedChord.FirstOrDefault();
            Length = genLength(selection);
        }

        public Length genLength(CommandSelections selection)
        {
            return Length.createInstance(selection.SelectedLength, selection.TupletType);
        }
    }

    public class ChangeMultipleChordLengthAtr : IActionAttributes
    {
        public Measure Measure { get; }
        public List<Chord> Chords { get; }
        public Length Length { get; }

        public ChangeMultipleChordLengthAtr(CommandSelections selection)
        {
            Measure = selection.SelectedMeasure.FirstOrDefault();
            Chords = selection.SelectedChord;
            Length = genLength(selection);
        }

        public Length genLength(CommandSelections selection)
        {
            return Length.createInstance(selection.SelectedLength, selection.TupletType);
        }
    }

    public class ChangeNoteStringAtr : IActionAttributes
    {
        public NoteChord Chord { get; }
        public Note Note { get; }
        public int? NewString { get; }

        public ChangeNoteStringAtr(CommandSelections selection)
        {
            Chord = selection.SelectedChord.FirstOrDefault() as NoteChord;
            Note = selection.SelectedNote.FirstOrDefault();
            NewString = selection.String;
        }
    }

    public class ChangeNoteFretAtr : IActionAttributes
    {
        public Note Note { get; }
        public int? Fret { get; }

        public ChangeNoteFretAtr(CommandSelections selection)
        {
            Note = selection.SelectedNote.FirstOrDefault();
            Fret = selection.Fret;
        }
    }

    public class AddPalmMuteEffectAtr : IActionAttributes
    {
        public NoteChord Chord { get; }
        public IEffect Effect { get; }
        
        public AddPalmMuteEffectAtr(CommandSelections selection)
        {
            Chord = selection.SelectedChord.FirstOrDefault() as NoteChord;
            Effect = genPalmMute(selection);
        }

        public IEffect genPalmMute(CommandSelections selection)
        {
            return PalmMute.createInstance();
        }
    }

    public class AddSingleNoteEffectAtr : IActionAttributes
    { 
        public Note Note { get; }
        public IEffect Effect { get; }

        public AddSingleNoteEffectAtr(CommandSelections selection)
        {
            Note = selection.SelectedNote.FirstOrDefault();
            Effect = genEffect(selection);
        }

        public IEffect genEffect(CommandSelections selection)
        {
            IEffect effect = null;

            switch (selection.SelectedEffectType)
            {
                case EffectType.Bend:
                    effect = Bend.createInstance(selection.BendAmount ?? .5, selection.Returns ?? false);
                    break;
                case EffectType.Pinch_Harmonic:
                    effect = PinchHarmonic.createInstance();
                    break;
                case EffectType.Vibrato:
                    effect = Vibrato.createInstance(selection.Wide ?? false);
                    break;
            }
            selection.SelectedEffect = effect;
            return effect;
        }
    }

    public class RemoveNoteEffectAtr : IActionAttributes
    {
        public Note Note { get; }
        public IEffect Effect { get; }

        public RemoveNoteEffectAtr(CommandSelections selection)
        {
            Note = selection.SelectedNote.FirstOrDefault();
            Effect = selection.SelectedEffect;
        }
    }

    public class AddMultiNoteEffectAtr : IActionAttributes
    {
        public IMultiEffect Effect { get; }

        public AddMultiNoteEffectAtr(CommandSelections selection)
        {
            Effect = genEffect(selection);
        }

        public IMultiEffect genEffect(CommandSelections selection)
        {
            Note first = selection.SelectedNote[0];
            Note second = selection.SelectedNote[1];
            IMultiEffect effect = null;

            switch (selection.SelectedEffectType)
            {
                case EffectType.Slide:
                    effect = Slide.createInstance(first, second, selection.Legato ?? false);
                    break;
                case EffectType.HOPO:
                    effect = HOPO.createInstance(first, second);
                    break;
                case EffectType.Tie:
                    effect = Tie.createInstance(first, second);
                    break;
            }
            selection.SelectedEffect = effect;
            return effect;
        }
    }

    public class AddMeasureToPartAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure Measure { get; }

        public AddMeasureToPartAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measure = genMeasure(selection);
        }

        public Measure genMeasure(CommandSelections selection)
        {
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, selection.Position);
        }
    }

    public class RemoveMeasureFromPartAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure Measure { get; }

        public RemoveMeasureFromPartAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measure = selection.SelectedMeasure.FirstOrDefault();
        }
    }

    public class RemoveMultipleMeasuresFromPartAtr : IActionAttributes
    {
        public Part Part { get; }
        public List<Measure> Measures { get; }

        public RemoveMultipleMeasuresFromPartAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measures = selection.SelectedMeasure;
        }
    }

    public class ChangeMeasurePositionAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure Measure { get; }
        public int? Position { get; }
        
        public ChangeMeasurePositionAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measure = selection.SelectedMeasure.FirstOrDefault();
            Position = selection.Position;
        }
    }

    public class ChangeMultipleMeasurePositionAtr : IActionAttributes
    {
        public Part Part { get; }
        public List<Measure> Measures { get; }
        public int? Position { get; }

        public ChangeMultipleMeasurePositionAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measures = selection.SelectedMeasure;
            Position = genCorrectedPosition(selection);
        }

        public int? genCorrectedPosition(CommandSelections selection)
        {
            int init = (int)selection.Position;
            foreach (Measure Measure in Measures)
            {
                if (Measure.Position.Index == init) { return null; }
            }

            int first_pos = (from Measure measure in Measures select measure.Position.Index).Min();
            if (init > first_pos) { init -= (Measures.Count() - 1); }

            return init;
        }
    }

    public class ChangeMeasureTimeSigAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure Measure { get; }
        public int? NumBeats { get; }
        public NoteLength? BeatType { get; }

        public ChangeMeasureTimeSigAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measure = selection.SelectedMeasure.FirstOrDefault();
            NumBeats = selection.NumBeats;
            BeatType = selection.BeatType;
        }
    }

    public class ChangeMeasureBpmAtr : IActionAttributes
    {
        public Part Part { get; }
        public Measure Measure { get; }
        public int? Bpm { get; }

        public ChangeMeasureBpmAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            Measure = selection.SelectedMeasure.FirstOrDefault();
            Bpm = selection.BPM;
        }
    }

    public class CreateTupletFromNotesAtr : IActionAttributes
    {
        public Measure Measure { get; }
        public List<Chord> Chords { get; }
        public TupletType Type { get; }
        public List<Length> NewLengths { get; }

        public CreateTupletFromNotesAtr(CommandSelections selections)
        {
            Measure = selections.SelectedMeasure.FirstOrDefault();
            Chords = selections.SelectedChord;
            Type = selections.TupletType;
        }

        public List<Length> genTupletLengths(CommandSelections selections)
        {
            var lengths = new List<Length>();
            foreach (Chord chord in Chords)
            {
                lengths.Add(TupleLength.createInstance(chord.Length.NoteType, Type));
            }
            return lengths;
        }
    }

    public class ChangeSongInfoAtr : IActionAttributes
    {
        public Part Part { get; }
        public string Name { get; }
        public string Artist { get; }
        public string Album { get; }

        public ChangeSongInfoAtr(CommandSelections selections)
        {
            Part = selections.SelectedPart;
            Name = selections.Name;
            Artist = selections.Artist;
            Album = selections.Album;
        }
    }

    public class ChangeInstrumentInfoAtr : IActionAttributes
    {
        public Part Part { get; }
        public InstrumentType? Instrument { get; }
        public int? StringNum { get; }

        public ChangeInstrumentInfoAtr(CommandSelections selections)
        {
            Part = selections.SelectedPart;
            Instrument = selections.Instrument;
            StringNum = selections.StringNum;
        }
    }
}
