using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class Interpreter
    {
        public const string PART = "P";
        public const string MEASURE = "M";
        public const string CHORD = "C";
        public const string NOTE = "N";
        public const string PALMMUTE = "PME";
        public const string BEND = "BE";
        public const string PINCHHARMONIC = "PHE";
        public const string VIBRATO = "VE";
        public const string SLIDE = "SE";
        public const string HOPO = "HPE";
        public const string TIE = "TE";

        public void writeTabToFile(string file_name, Part part)
        {
            var dict = new NoteDict();
            var writer = new PartWriter(part, dict);
            List<string> to_write = writer.generateObjectString();

            System.IO.File.WriteAllLines(file_name, to_write);
        }

        public Part readTabFromFile(string file_name)
        {
            List<string> to_read = new List<string>(System.IO.File.ReadAllLines(file_name));
            List<string> sanitized_to_read = sanitizeLines(to_read);
            var reader = new PartReader(sanitized_to_read);

            ReaderAttributes attr = new ReaderAttributes();
            reader.buildObject(attr);
            return attr.CurrentPart;
        }

        public List<string> sanitizeLines(List<string> lines)
        {
            var sanitized_lines = new List<string>();
            foreach (string line in lines)
            {
                sanitized_lines.Add(line.Replace("\t","").Trim());
            }
            return sanitized_lines;
        }
    }

    public class ReaderAttributes
    {
        public Dictionary<string, Note> NoteDict { get; set; }
        public Part CurrentPart { get; set; }
        public Measure CurrentMeasure { get; set; }
        public Chord CurrentChord { get; set; }
        public Note CurrentNote { get; set; }

        public ReaderAttributes()
        {
            NoteDict = new Dictionary<string, Note>();
            CurrentPart = null;
            CurrentMeasure = null;
            CurrentChord = null;
            CurrentNote = null;
        }
    }

    public class ObjectReader
    {
        public string Tag { get; set; }
        public string[] Attributes { get; set; }
        public List<ObjectReader> ContainedObjects { get; set; }

        public ObjectReader(List<string> lines)
        {
            constructAttributes(lines);
        }

        public void constructAttributes(List<string> lines)
        {
            Tag = getTag(lines);
            stripTag(lines);
            Attributes = getAttributes(lines);
            stripAttributes(lines);

            ContainedObjects = getAllContainedObjects(lines);
        }

        public string getTag(List<string> lines) { return lines.First().Replace("<", ""); }

        public void stripTag(List<string> lines)
        {
            if (lines.First().Contains(Tag)) { lines.Remove(lines.First()); }
            if (lines.Last().Contains(Tag)) { lines.Remove(lines.Last()); }
        }

        public string[] getAttributes(List<string> lines)
        {
            string attribute_line = lines.First().Replace("(", "").Replace(")", "");
            return attribute_line.Split(',');
        }

        public void stripAttributes(List<string> lines)
        {
            if (lines.First().StartsWith("(")) { lines.Remove(lines.First()); }
        }

        public List<ObjectReader> getAllContainedObjects(List<string> lines)
        {
            var object_list = new List<ObjectReader>();

            while (true) //im sorry
            {
                List<string> object_lines = getNextObject(lines);
                if (object_lines is null) { break; }

                stripObject(lines, object_lines);
                ObjectReader reader = getInterpreter(lines);
                if (reader != null) { object_list.Add(reader); }
            }

            return object_list;
        }

        public List<string> getNextObject(List<string> lines)
        {
            var object_lines = new List<string>();
            bool start_found = false;
            foreach (string line in lines)
            {
                if (line.StartsWith("<")) { start_found = true; }
                if (start_found) { object_lines.Add(line); }
                if (line.EndsWith(">")) { return object_lines; }
            }
            return null;
        }

        public void stripObject(List<string> lines, List<string> object_lines)
        {
            foreach (string line in object_lines) { lines.Remove(line); }
        }

        public ObjectReader getInterpreter(List<string> lines)
        {
            switch (getTag(lines))
            {
                case Interpreter.PART:
                    return new PartReader(lines);
                case Interpreter.MEASURE:
                    return new MeasureReader(lines);
                case Interpreter.CHORD:
                    return new ChordReader(lines);
                case Interpreter.NOTE:
                    return new NoteReader(lines);
                case Interpreter.PALMMUTE:
                    return new PalmMuteReader(lines);
                case Interpreter.BEND:
                    return new BendReader(lines);
                case Interpreter.PINCHHARMONIC:
                    return new PinchHarmonicReader(lines);
                case Interpreter.VIBRATO:
                    return new VibratoReader(lines);
                case Interpreter.SLIDE:
                    return new SlideReader(lines);
                case Interpreter.HOPO:
                    return new HOPOReader(lines);
                case Interpreter.TIE:
                    return new TieReader(lines);
                default:
                    return null;
            }
        }

        public virtual bool canBuildObject() { return false; }

        public virtual void buildObject(ReaderAttributes attributes)
        {
            buildContainedObjects(attributes);
        }

        public virtual void buildContainedObjects(ReaderAttributes attributes)
        {
            foreach (ObjectReader reader in ContainedObjects)
            {
                if (reader.canBuildObject()) { reader.buildObject(attributes); }
            }
        }
    }

    public class PartReader : ObjectReader
    {
        public PartReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            int bpm = Convert.ToInt32(Attributes[0]);
            int num_beats = Convert.ToInt32(Attributes[1]);
            NoteLength beat_length = NoteLengthExtensions.getNoteLengthFromVisualLength(Convert.ToInt32(Attributes[2]));

            Part part = Part.createInstance(bpm, num_beats, beat_length);
            attributes.CurrentPart = part;

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            try
            {
                Convert.ToInt32(Attributes[0]);
                Convert.ToInt32(Attributes[1]);
                Convert.ToInt32(Attributes[2]);
                return true;
            }
            catch (Exception e) { return false; }
        }
    }

    public class MeasureReader : ObjectReader
    {
        public MeasureReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            int pos = Convert.ToInt32(Attributes[0]);
            int bpm = Convert.ToInt32(Attributes[1]);
            int num_beats = Convert.ToInt32(Attributes[2]);
            NoteLength beat_length = NoteLengthExtensions.getNoteLengthFromVisualLength(Convert.ToInt32(Attributes[3]));

            Measure measure = Measure.createInstance(bpm, num_beats, beat_length, pos);

            attributes.CurrentMeasure = measure;
            attributes.CurrentPart.Add(measure);

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            try
            {
                Convert.ToInt32(Attributes[0]);
                Convert.ToInt32(Attributes[1]);
                Convert.ToInt32(Attributes[2]);
                Convert.ToInt32(Attributes[3]);
                return true;
            }
            catch (Exception e) { return false; }
        }
    }

    public class ChordReader : ObjectReader
    {
        public ChordReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            bool is_note_chord = Convert.ToBoolean(Attributes[0]);
            int position = Convert.ToInt32(Attributes[1]);
            Length length = buildLength(Attributes[2]);

            Chord chord;
            if (is_note_chord) { chord = NoteChord.createInstance(position, attributes.CurrentMeasure.Position, length); }
            else { chord = Chord.createInstance(position, attributes.CurrentMeasure.Position, length); }

            attributes.CurrentChord = chord;
            attributes.CurrentMeasure.Add(chord);

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            if (Attributes.Length != 3) { return false; }
            if (Attributes[2].Split(',').Length != 2 && Attributes[2].Split(',').Length != 5) { return false; }
            try
            {
                Convert.ToBoolean(Attributes[0]);
                Convert.ToInt32(Attributes[1]);
                return true;
            }
            catch (Exception e) { return false; }
        }

        public Length buildLength(string length_string)
        {
            string[] length_attributes = length_string.Replace("[", "").Replace("]", "").Split(',');
            switch (length_attributes[0])
            {
                case "L":
                    int note_type = Convert.ToInt32(length_attributes[1]);
                    if (NoteLengthExtensions.intIsValidNoteLength(note_type)) { return Length.createInstance((NoteLength)note_type); }
                    return null;
                case "N":
                    int non_standard_length = Convert.ToInt32(length_attributes[1]);
                    return new NonStandardLength(non_standard_length);
                case "T":
                    int note_length = Convert.ToInt32(length_attributes[0]);
                    int type = Convert.ToInt32(length_attributes[1]);
                    int base_length = Convert.ToInt32(length_attributes[2]);
                    return TupleLength.createInstance(note_length, type);
                default:
                    return null;
            }
        }
    }

    public class NoteReader : ObjectReader
    {
        public NoteReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            int fret = Convert.ToInt32(Attributes[0]);
            int note_string = Convert.ToInt32(Attributes[1]);
            string note_name = Attributes[2];

            var note = Note.createInstance(fret, note_string, attributes.CurrentChord.Position as MultiPosition, attributes.CurrentChord.Length);
            if (attributes.CurrentChord is NoteChord) { ((NoteChord)attributes.CurrentChord).Add(note); }

            attributes.NoteDict.Add(note_name, note);
            attributes.CurrentNote = note;

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            if (Attributes.Length != 3) { return false; }
            try
            {
                Convert.ToInt32(Attributes[0]);
                Convert.ToInt32(Attributes[1]);
                return true;
            }
            catch(Exception e) { return false; }
        }
    }

    public class PalmMuteReader : ObjectReader
    {
        public PalmMuteReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            var palm_mute = PalmMute.createInstance();
            attributes.CurrentNote.Add(palm_mute);

            base.buildObject(attributes);
        }

        public override bool canBuildObject() { return true; }
    }

    public class BendReader : ObjectReader
    {
        public BendReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            double amount = Convert.ToDouble(Attributes[0]);
            bool returns = Convert.ToBoolean(Attributes[1]);

            var bend = Bend.createInstance(amount, returns);
            attributes.CurrentNote.Add(bend);

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            try
            {
                Convert.ToDouble(Attributes[0]);
                Convert.ToInt32(Attributes[1]);
                return true;
            }
            catch (Exception e) { return false; }
        }
    }

    public class PinchHarmonicReader : ObjectReader
    {
        public PinchHarmonicReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            var pinch_harmonic = PinchHarmonic.createInstance();
            attributes.CurrentNote.Add(pinch_harmonic);

            base.buildObject(attributes);
        }

        public override bool canBuildObject() { return true; }
    }

    public class VibratoReader : ObjectReader
    {
        public VibratoReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            bool wide = Convert.ToBoolean(Attributes[0]);

            var vibrato = Vibrato.createInstance(wide);
            attributes.CurrentNote.Add(vibrato);

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            try
            {
                Convert.ToBoolean(Attributes[0]);
                return true;
            }
            catch (Exception e) { return false; }
        }
    }

    public class SlideReader : ObjectReader
    {
        public SlideReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            Note first = attributes.NoteDict[Attributes[0]];
            Note second = attributes.NoteDict[Attributes[1]];
            bool legato = Convert.ToBoolean(Attributes[2]);

            IMultiEffect slide = Slide.createInstance(first, second, legato);
            first.Add(slide);
            second.Add(slide);

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            if (Attributes.Length != 3) { return false; }
            try
            {
                Convert.ToBoolean(Attributes[2]);
                return true;
            }
            catch (Exception e) { return false; }
        }
    }

    public class HOPOReader : ObjectReader
    {
        public HOPOReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            Note first = attributes.NoteDict[Attributes[0]];
            Note second = attributes.NoteDict[Attributes[1]];

            var hopo = HOPO.createInstance(first, second);
            first.Add(hopo);
            second.Add(hopo);

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            return (Attributes.Length == 2);
        }
    }

    public class TieReader : ObjectReader
    {
        public TieReader(List<string> lines) : base(lines) { }

        public override void buildObject(ReaderAttributes attributes)
        {
            Note first = attributes.NoteDict[Attributes[0]];
            Note second = attributes.NoteDict[Attributes[1]];

            var tie = Tie.createInstance(first, second);
            first.Add(tie);
            second.Add(tie);

            base.buildObject(attributes);
        }

        public override bool canBuildObject()
        {
            return (Attributes.Length == 2);
        }
    }
}
