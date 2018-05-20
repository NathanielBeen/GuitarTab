﻿using System;
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
            Part part = Part.createInstance(selection.BPM, selection.NumBeats, selection.BeatType);
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
            return NoteChord.createInstance(selection.Position, Measure?.Position, selection.SelectedLength);
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
            return Chord.createInstance(selection.Position, Measure?.Position, selection.SelectedLength);
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
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, Part?.TimeSignature, Part?.DefaultBPM, selection.Position);
        }

        public Chord genChord(CommandSelections selection)
        {
            return Chord.createInstance(selection.Position, Measure?.Position, selection.SelectedLength);
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
            return NoteChord.createInstance(selection.Position, Measure?.Position, selection.SelectedLength);
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
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, Part?.TimeSignature, Part?.DefaultBPM, selection.Position);
        }

        public NoteChord genChord(CommandSelections selection)
        {
            return NoteChord.createInstance(selection.Position, Measure?.Position, selection.SelectedLength);
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
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, Part?.TimeSignature, Part?.DefaultBPM, selection.Position);
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
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, Part?.TimeSignature, Part?.DefaultBPM, selection.Position);
        }
    }

    public class ChangeMultipleChordPositionAtr : IActionAttributes
    {
        public Dictionary<Chord, Measure> ChordDict { get; }
        public Measure SecondMeasure { get; }
        public int? Position { get; }

        public ChangeMultipleChordPositionAtr(CommandSelections selection)
        {
            ChordDict = genChordDict(selection);
            SecondMeasure = selection.SelectedMeasure.LastOrDefault();
            Position = selection.Position;
        }

        public Dictionary<Chord, Measure> genChordDict(CommandSelections selection)
        {
            var dict = new Dictionary<Chord, Measure>();
            foreach (Chord chord in selection.SelectedChord)
            {
                foreach (Measure measure in selection.SelectedMeasure)
                {
                    if (measure.ModelCollection.Contains(chord))
                    {
                        dict.Add(chord, measure);
                        break;
                    }
                }
            }
            return dict;
        }
    }

    public class ChangeMultipleChordPositionNewMeasureAtr : IActionAttributes
    {
        public Part Part { get; }
        public Dictionary<Chord, Measure> ChordDict { get; }
        public Measure SecondMeasure { get; }
        public int Position { get; }

        public ChangeMultipleChordPositionNewMeasureAtr(CommandSelections selection)
        {
            Part = selection.SelectedPart;
            ChordDict = genChordDict(selection);
            SecondMeasure = genSecondMeasure(selection);
            Position = 0;
        }

        public Dictionary<Chord, Measure> genChordDict(CommandSelections selection)
        {
            var dict = new Dictionary<Chord, Measure>();
            foreach (Chord chord in selection.SelectedChord)
            {
                foreach (Measure measure in selection.SelectedMeasure)
                {
                    if (measure.ModelCollection.Contains(chord))
                    {
                        dict.Add(chord, measure);
                        break;
                    }
                }
            }
            return dict;
        }

        public Measure genSecondMeasure(CommandSelections selection)
        {
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, Part.TimeSignature, Part.DefaultBPM, selection.Position);
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
            Length = selection.SelectedLength;
        }
    }

    public class ChangeMultipleChordLengthAtr : IActionAttributes
    {
        public Dictionary<Chord, Measure> ChordDict { get; }
        public Length Length { get; }

        public ChangeMultipleChordLengthAtr(CommandSelections selection)
        {
            ChordDict = genChordDict(selection);
            Length = selection.SelectedLength;
        }

        public Dictionary<Chord, Measure> genChordDict(CommandSelections selection)
        {
            var dict = new Dictionary<Chord, Measure>();
            foreach (Chord chord in selection.SelectedChord)
            {
                foreach (Measure measure in selection.SelectedMeasure)
                {
                    if (measure.ModelCollection.Contains(chord))
                    {
                        dict.Add(chord, measure);
                        break;
                    }
                }
            }
            return dict;
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
            return Measure.createInstance(selection.BPM, selection.NumBeats, selection.BeatType, Part?.TimeSignature, Part?.DefaultBPM, selection.Position);
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
            Position = selection.Position;
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
}