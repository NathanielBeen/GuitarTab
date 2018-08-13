using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class SimplifiedPart
    {
        public string SongName { get; }
        public string ArtistName { get; }
        public string AlbumName { get; }
        public int InstrumentType { get; }
        public int NumStrings { get; }

        public int BPM { get; }
        public int NumBeats { get; }
        public int BeatLength { get; }
        public SimplifiedMeasure[] Measures { get; }
        public SimplifiedMultiEffect[] Effects { get; }

        public SimplifiedPart(Part part, WriterInfo info)
        {
            SongName = part.SongInfo.SongName;
            ArtistName = part.SongInfo.ArtistName;
            AlbumName = part.SongInfo.AlbumName;
            InstrumentType = (int)part.InstrumentInfo.Type;
            NumStrings = part.InstrumentInfo.Strings;

            BPM = part.DefaultBPM;
            NumBeats = part.TimeSignature.NumberOfBeats;
            BeatLength = (int)part.TimeSignature.BeatType;

            Measures = createMeasures(part, info);
            Effects = info.getMultiEffects();
        }

        public SimplifiedMeasure[] createMeasures(Part part, WriterInfo info)
        {
            var arr = new SimplifiedMeasure[part.ModelCollection.Count()];
            int index = 0;

            foreach (Measure measure in part.ModelCollection.Items())
            {
                arr[index] = new SimplifiedMeasure(measure, info);
                index++;
            }

            return arr;
        }

        public Part createPart(ReaderInfo info)
        {
            var song_info = new SongInfo(SongName, ArtistName, AlbumName);
            var instrument_info = new InstrumentInfo((InstrumentType)InstrumentType, NumStrings);
            var part = Part.createInstance(song_info, instrument_info, BPM, NumBeats, (NoteLength)BeatLength);
            foreach (var s_measure in Measures) { part?.Add(s_measure.createMeasure(info)); }

            foreach(var effect in Effects) { info.readInMultiEffect(effect); }
            info.createMultiEffects();
            return part;
        }
    }

    public class SimplifiedMeasure
    {
        public int BPM { get; }
        public int NumBeats { get; }
        public int BeatLength { get; }
        public int Position { get; }
        public SimplifiedChord[] Chords { get; }

        public SimplifiedMeasure(Measure measure, WriterInfo info)
        {
            BPM = measure.Bpm;
            NumBeats = measure.TimeSignature.NumberOfBeats;
            BeatLength = (int)measure.TimeSignature.BeatType;
            Chords = createChords(measure, info);
        }

        public SimplifiedChord[] createChords(Measure measure, WriterInfo info)
        {
            var arr = new SimplifiedChord[measure.ModelCollection.Count()];
            int index = 0;

            foreach(Chord chord in measure.ModelCollection.Items())
            {
                arr[index] = SimplifiedChord.createSimplifiedChord(chord, info);
                index++;
            }

            return arr;
        }

        public Measure createMeasure(ReaderInfo info)
        {
            var measure = Measure.createInstance(BPM, NumBeats, (NoteLength)BeatLength, Position);
            foreach (var s_chord in Chords) { measure?.Add(s_chord.createChord(measure, info)); }
            return measure;
        }
    }

    public class SimplifiedChord
    {
        public int Position { get; }
        public int NoteLength { get; }
        public int TupletType { get; }

        public SimplifiedChord(Chord chord)
        {
            Position = chord.Position.Index;
            NoteLength = (int)chord.Length.NoteType;
            TupletType = (int)((chord.Length as TupleLength)?.TupleType ?? GuitarTab.TupletType.None);
        }

        public static SimplifiedChord createSimplifiedChord(Chord chord, WriterInfo info)
        {
            if (chord is NoteChord) { return new SimplifiedNoteChord(chord as NoteChord, info); }
            else { return new SimplifiedChord(chord); }
        }

        public virtual Chord createChord(Measure parent, ReaderInfo info)
        {
            var length = Length.createInstance((GuitarTab.NoteLength)NoteLength, (GuitarTab.TupletType)TupletType);
            var chord = Chord.createInstance(Position, parent.Position, length);
            return chord;
        }
    }

    public class SimplifiedNoteChord : SimplifiedChord
    {
        public SimplifiedNote[] Notes { get; }

        public SimplifiedNoteChord(NoteChord chord, WriterInfo info)
            :base(chord)
        {
            Notes = createNotes(chord, info);
        }

        public SimplifiedNote[] createNotes(NoteChord chord, WriterInfo info)
        {
            var arr = new SimplifiedNote[chord.ModelCollection.Count()];
            int index = 0;

            foreach (Note note in chord.ModelCollection.Items())
            {
                arr[index] = new SimplifiedNote(note, info);
                index++;
            }

            return arr;
        }

        public override Chord createChord(Measure parent, ReaderInfo info)
        {
            var length = Length.createInstance((GuitarTab.NoteLength)NoteLength, (GuitarTab.TupletType)TupletType);
            var chord = NoteChord.createInstance(Position, parent.Position, length);
            foreach (var s_note in Notes) { chord?.Add(s_note.createNote(chord, info)); }
            return chord;
        }
    }

    public class SimplifiedNote
    {
        public static Dictionary<int, Note> CreatedNotesToModel;

        public int NoteId { get; set; }

        public int Fret { get; }
        public int String { get; }
        public SimplifiedEffect[] Effects { get; }

        public SimplifiedNote(Note note, WriterInfo info)
        {
            Fret = note.Fret;
            String = note.String;
            Effects = createEffects(note, info);

            info.readInSimplifiedNote(this, note);
        }

        public SimplifiedEffect[] createEffects(Note note, WriterInfo info)
        {
            int num_single_effects = note.ModelCollection.getItemsMatchingCondition(e => !(e is IMultiEffect)).Count();
            var arr = new SimplifiedEffect[num_single_effects];
            int index = 0;

            foreach (var effect in note.ModelCollection.Items())
            {
                var s_effect = SimplifiedEffect.createSimplifiedEffect(effect, info);
                if (s_effect != null)
                {
                    arr[index] = s_effect;
                    index++;
                }
            }
            return arr;
        }

        public Note createNote(Chord parent, ReaderInfo info)
        {
            var note = Note.createInstance(Fret, String, parent.Position as MultiPosition, parent.Length);
            info.readInNote(this, note);
            foreach (var effect in Effects) { note.Add(effect.createEffect()); }
            return note;
        }

        public static void resetStaticMembers()
        {
            CreatedNotesToModel = new Dictionary<int, Note>();
        }
    }

    public class SimplifiedEffect
    {
        public virtual EffectType Type
        {
            get { return EffectType.No_Type; }
        }

        public virtual IEffect createEffect() { return null; }

        public static SimplifiedEffect createSimplifiedEffect(IEffect effect, WriterInfo info)
        {
            if (effect is IMultiEffect) { info.createSimplifiedMultiEffect(effect as IMultiEffect); }
            else if (effect is PalmMute) { return new SimplifiedPalmMute(); }
            else if (effect is Bend) { return new SimplifiedBend(effect as Bend); }
            else if (effect is PinchHarmonic) { return new SimplifiedPinchHarmonic(); }
            else if (effect is Vibrato) { return new SimplifiedVibrato(effect as Vibrato); }
            return null;
        }
    }

    public class SimplifiedMultiEffect : SimplifiedEffect
    {
        public int FirstNoteId { get; set; }
        public int SecondNoteId { get; set; }

        public virtual IMultiEffect createMultiEffect() { return null; }

        public bool canCreateMultiEffect()
        {
            return (SimplifiedNote.CreatedNotesToModel.ContainsKey(FirstNoteId)
                    && SimplifiedNote.CreatedNotesToModel.ContainsKey(SecondNoteId));
        }
    }

    public class SimplifiedPalmMute : SimplifiedEffect
    {
        public override EffectType Type
        {
            get { return EffectType.Palm_Mute; }
        }

        public override IEffect createEffect() { return PalmMute.createInstance(); }
    }

    public class SimplifiedBend : SimplifiedEffect
    {
        public double Amount { get; }
        public bool Returns { get; }

        public override EffectType Type
        {
            get { return EffectType.Bend; }
        }

        public SimplifiedBend(Bend bend)
        {
            Amount = bend.Amount;
            Returns = bend.BendReturns;
        }

        public override IEffect createEffect() { return Bend.createInstance(Amount, Returns); }
    }

    public class SimplifiedPinchHarmonic : SimplifiedEffect
    {
        public override EffectType Type
        {
            get { return EffectType.Pinch_Harmonic; }
        }

        public override IEffect createEffect() { return PinchHarmonic.createInstance(); }
    }

    public class SimplifiedVibrato : SimplifiedEffect
    {
        public bool Wide { get; }

        public override EffectType Type
        {
            get { return EffectType.Vibrato; }
        }

        public SimplifiedVibrato(Vibrato vibrato)
        {
            Wide = vibrato.Wide;
        }

        public override IEffect createEffect() { return Vibrato.createInstance(Wide); }
    }

    public class SimplifiedSlide : SimplifiedMultiEffect
    {
        public bool Legato { get; }

        public override EffectType Type
        {
            get { return EffectType.Slide; }
        }

        public SimplifiedSlide(Slide slide)
        {
            Legato = slide.Legato;
        }

        public override IMultiEffect createMultiEffect()
        {
            if (!canCreateMultiEffect()) { return null; }
            Note first = SimplifiedNote.CreatedNotesToModel[FirstNoteId];
            Note second = SimplifiedNote.CreatedNotesToModel[SecondNoteId];
            return Slide.createInstance(first, second, Legato);
        }
    }

    public class SimplifiedHOPO : SimplifiedMultiEffect
    {
        public override EffectType Type
        {
            get { return EffectType.HOPO; }
        }

        public override IMultiEffect createMultiEffect()
        {
            if (!canCreateMultiEffect()) { return null; }
            Note first = SimplifiedNote.CreatedNotesToModel[FirstNoteId];
            Note second = SimplifiedNote.CreatedNotesToModel[SecondNoteId];
            return HOPO.createInstance(first, second);
        }
    }

    public class SimplifiedTie : SimplifiedMultiEffect
    {
        public override EffectType Type
        {
            get { return EffectType.Tie; }
        }

        public override IMultiEffect createMultiEffect()
        {
            if (!canCreateMultiEffect()) { return null; }
            Note first = SimplifiedNote.CreatedNotesToModel[FirstNoteId];
            Note second = SimplifiedNote.CreatedNotesToModel[SecondNoteId];
            return Tie.createInstance(first, second);
        }
    }
}
