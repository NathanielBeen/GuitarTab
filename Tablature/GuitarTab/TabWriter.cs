using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class ObjectWriter
    {
        public string Tag { get; set; }
        public string[] Attributes { get; set; }
        public List<ObjectWriter> ContainedObjects { get; set; }

        public List<string> generateObjectString()
        {
            List<string> object_strings = new List<string>();
            insertTag(object_strings);
            insertAttributes(object_strings);
            insertContainedObjects(object_strings);

            return object_strings;
        }

        public void insertTag(List<string> object_string)
        {
            string tag_start = "<" + Tag;
            string tag_end = Tag + ">";

            object_string.Insert(0, tag_start);
            object_string.Add(tag_end);
        }

        public void insertAttributes(List<string> object_string)
        {
            string attribute_string = "(";
            foreach (string attribute in Attributes)
            {
                attribute_string += attribute;
                if (!Attributes.Last().Equals(attribute)) { attribute_string += ","; }
            }
            attribute_string += ")";

            object_string.Insert(1, attribute_string);
        }

        public void insertContainedObjects(List<string> object_string)
        {
            foreach (ObjectWriter writer in ContainedObjects)
            {
                object_string.AddRange(writer.generateObjectString());
            }
        }

        public void generateObject()
        {
            Tag = genTag();
            Attributes = genAttributes();
            ContainedObjects = genContainedObjects();
        }

        public virtual string genTag() { return ""; }
        public virtual string[] genAttributes() { return new string[0]; }
        public virtual List<ObjectWriter> genContainedObjects() { return new List<ObjectWriter>(); }
    }

    public class PartWriter : ObjectWriter
    {
        private Part part;
        private NoteDict dict;

        public PartWriter(Part p, NoteDict d)
        {
            part = p;
            dict = d;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.PART;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[3];
            attr[0] = part.DefaultBPM.ToString();
            attr[1] = part.TimeSignature.NumberOfBeats.ToString();
            attr[2] = part.TimeSignature.BeatType.getVisualNoteLength().ToString();

            return attr;
        }

        public override List<ObjectWriter> genContainedObjects()
        {
            var contained = new List<ObjectWriter>();
            foreach (Measure measure in part.ModelCollection.Items())
            {
                contained.Add(new MeasureWriter(measure, dict));
            }

            return contained;
        }
    }

    public class MeasureWriter : ObjectWriter
    {
        private Measure measure;
        private NoteDict dict;

        public MeasureWriter(Measure m, NoteDict d)
        {
            measure = m;
            dict = d;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.MEASURE;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[4];
            attr[0] = measure.Position.Index.ToString();
            attr[1] = measure.Bpm.ToString();
            attr[2] = measure.TimeSignature.NumberOfBeats.ToString();
            attr[3] = measure.TimeSignature.BeatType.getVisualNoteLength().ToString();

            return attr;
        }

        public override List<ObjectWriter> genContainedObjects()
        {
            var contained = new List<ObjectWriter>();
            var multi_effects = new List<IMultiEffect>();

            foreach (Chord chord in measure.ModelCollection.Items())
            {
                contained.Add(new ChordWriter(chord, dict));
                if (chord is NoteChord) { getAllMultiEffects((NoteChord)chord, multi_effects); }
            }
            foreach (IMultiEffect effect in multi_effects)
            {
                getWriterFromEffect(effect);
            }

            return contained;
        }

        public void getAllMultiEffects(NoteChord chord, List<IMultiEffect> effects)
        {
            foreach (Note note in chord.ModelCollection.Items())
            {
                List<IEffect> potential_effects = note.getMultiEffects();
                foreach (IMultiEffect e in potential_effects)
                {
                    if (!effects.Contains(e)) { effects.Add(e); }
                }
            }
        }

        public ObjectWriter getWriterFromEffect(IMultiEffect effect)
        {
            if (effect is Slide) { return new SlideWriter((Slide)effect, dict); }
            if (effect is HOPO) { return new HOPOWriter((HOPO)effect, dict); }
            if (effect is Tie) { return new TieWriter((Tie)effect, dict); }
            return null;
        }
    }

    public class ChordWriter : ObjectWriter
    {
        private Chord chord;
        private NoteDict dict;

        public ChordWriter(Chord c, NoteDict d)
        {
            chord = c;
            dict = d;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.CHORD;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[3];
            attr[0] = (chord is NoteChord).ToString();
            attr[1] = chord.Position.Index.ToString();
            attr[2] = genLengthString();
            return attr;
        }

        public override List<ObjectWriter> genContainedObjects()
        {
            var contained = new List<ObjectWriter>();
            if (chord is NoteChord)
            {
                foreach (Note note in ((NoteChord)chord).ModelCollection.Items())
                {
                    contained.Add(new NoteWriter(note, dict));
                }
            }
            return contained;
        }

        public string genLengthString()
        {
            string length_string = "[";
            if (chord.Length is TupleLength)
            {
                //var frac = ((TupleLength)chord.Length);
                //length_string += "G," + frac.Weight.ToString() + "," + frac.SplitInto.ToString() +
                //                "," + frac.Replacing.ToString() + ","+ chord.Length.NoteType.ToString();
            }
            else if (chord.Length is NonStandardLength)
            {
                var length = (NonStandardLength)chord.Length;
                length_string += "N," + length.getLength().ToString();
            }
            else
            {
                length_string += "G" + chord.Length.NoteType.ToString();
            }
            length_string += "]";
            return length_string;
        }
    }

    public class NoteWriter : ObjectWriter
    {
        private Note note;
        private NoteDict dict;

        public NoteWriter(Note n, NoteDict d)
        {
            note = n;
            dict = d;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.NOTE;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[3];
            attr[0] = note.Fret.ToString();
            attr[1] = note.String.ToString();

            string note_name = "note" + dict.CurrentNoteVal;
            dict.CurrentNoteVal++;
            dict.Dict.Add(note, note_name);
            attr[2] = note_name;

            return attr;
        }

        public override List<ObjectWriter> genContainedObjects()
        {
            var contained_objects = new List<ObjectWriter>();
            foreach (IEffect effect in note.ModelCollection.Items()) { contained_objects.Add(genWriterFromEffect(effect)); }
            return contained_objects;
        }

        public ObjectWriter genWriterFromEffect(IEffect effect)
        {
            if (effect is PalmMute) { return new PalmMuteWriter((PalmMute)effect); }
            if (effect is Bend) { return new BendWriter((Bend)effect); }
            if (effect is PinchHarmonic) { return new PinchHarmonicWriter((PinchHarmonic)effect); }
            if (effect is Vibrato) { return new VibratoWriter((Vibrato)effect); }
            return null;
        }
    }

    public class PalmMuteWriter : ObjectWriter
    {
        private PalmMute palm_mute;

        public PalmMuteWriter(PalmMute p)
        {
            palm_mute = p;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.PALMMUTE;
        }

        public override string[] genAttributes() { return new string[0]; }
    }

    public class BendWriter : ObjectWriter
    {
        private Bend bend;

        public BendWriter(Bend b)
        {
            bend = b;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.BEND;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[2];
            attr[0] = bend.Amount.ToString();
            attr[1] = bend.BendReturns.ToString();
            return attr;
        }
    }

    public class PinchHarmonicWriter : ObjectWriter
    {
        private PinchHarmonic pinch_harmonic;

        public PinchHarmonicWriter(PinchHarmonic ph)
        {
            pinch_harmonic = ph;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.PINCHHARMONIC;
        }

        public override string[] genAttributes() { return new string[0]; }
    }

    public class VibratoWriter : ObjectWriter
    {
        private Vibrato vibrato;

        public VibratoWriter(Vibrato v)
        {
            vibrato = v;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.VIBRATO;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[1];
            attr[0] = vibrato.Wide.ToString();
            return attr;
        }
    }

    public class SlideWriter : ObjectWriter
    {
        private Slide slide;
        private NoteDict dict;

        public SlideWriter(Slide s, NoteDict n)
        {
            slide = s;
            dict = n;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.SLIDE;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[3];
            attr[0] = dict.Dict[slide.First];
            attr[1] = dict.Dict[slide.Second];
            attr[2] = slide.Legato.ToString();
            return attr;
        }
    }

    public class HOPOWriter : ObjectWriter
    {
        private HOPO hopo;
        private NoteDict dict;
        
        public HOPOWriter(HOPO h, NoteDict n)
        {
            hopo = h;
            dict = n;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.HOPO;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[2];
            attr[0] = dict.Dict[hopo.First];
            attr[1] = dict.Dict[hopo.Second];
            return attr;
        }
    }

    public class TieWriter : ObjectWriter
    {
        private Tie tie;
        private NoteDict dict;

        public TieWriter(Tie t, NoteDict n)
        {
            tie = t;
            dict = n;
            generateObject();
        }

        public override string genTag()
        {
            return Interpreter.TIE;
        }

        public override string[] genAttributes()
        {
            string[] attr = new string[2];
            attr[0] = dict.Dict[tie.First];
            attr[1] = dict.Dict[tie.Second];
            return attr;
        }
    }

    public class NoteDict
    {
        public Dictionary<Note, string> Dict { get; set; }
        public int CurrentNoteVal { get; set; }

        public NoteDict()
        {
            Dict = new Dictionary<Note, string>();
            CurrentNoteVal = 0;
        }
    }
}
