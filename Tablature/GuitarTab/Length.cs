using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public enum NoteLength
    {
        None = 0,
        DottedWhole = 60480,
        Whole = 40320,
        DottedHalf = 30240,
        Half = 20160,
        DottedQuarter = 15120,
        Quarter = 10080,
        DottedEighth = 7560,
        Eighth = 5040,
        DottedSixeteenth = 3780,
        Sixeteenth = 2520,
        ThirtySecond = 1260
    }

    //create an extension method that converts the length into a string for menus
    public static class NoteLengthExtensions
    {
        public static bool intIsValidNoteLength(int input)
        {
            return Enum.IsDefined(typeof(NoteLength), input);
        }

        public static bool isDotted(this NoteLength length)
        {
            switch (length)
            {
                case NoteLength.DottedHalf:
                case NoteLength.DottedEighth:
                case NoteLength.DottedSixeteenth:
                    return true;
                default:
                    return false;
            }
        }

        public static int getVisualNoteLength(this NoteLength length)
        {
            switch (length)
            {
                case NoteLength.Half:
                    return 2;
                case NoteLength.Quarter:
                    return 4;
                case NoteLength.Eighth:
                    return 8;
                case NoteLength.Sixeteenth:
                    return 16;
                case NoteLength.ThirtySecond:
                    return 32;
                default:
                    return 0;
            }
        }

        public static NoteLength getNoteLengthFromVisualLength(int length)
        {
            switch (length)
            {
                case 2:
                    return NoteLength.Half;
                case 4:
                    return NoteLength.Quarter;
                case 8:
                    return NoteLength.Eighth;
                case 16:
                    return NoteLength.Sixeteenth;
                case 32:
                    return NoteLength.ThirtySecond;
                default:
                    return NoteLength.None;
            }
        }

        public static NoteLength getNoteLengthFromString(string name)
        {
            switch (name)
            {
                case "Dotted Whole":
                    return NoteLength.DottedWhole;
                case "Whole":
                    return NoteLength.Whole;
                case "Dotted Half":
                    return NoteLength.DottedHalf;
                case "Half":
                    return NoteLength.Half;
                case "Dotted Quarter":
                    return NoteLength.DottedQuarter;
                case "Quarter":
                    return NoteLength.Quarter;
                case "Dotted Eighth":
                    return NoteLength.DottedEighth;
                case "Eighth":
                    return NoteLength.Eighth;
                case "Dotted Sixteenth":
                    return NoteLength.DottedSixeteenth;
                case "Sixteenth":
                    return NoteLength.Sixeteenth;
                case "Thirty-Second":
                    return NoteLength.ThirtySecond;
                default:
                    return NoteLength.None;
            }
        }

        public static string getStringFromNoteLength(this NoteLength length)
        {
            switch (length)
            {
                case NoteLength.DottedWhole:
                    return "Dotted Whole";
                case NoteLength.Whole:
                    return "Whole";
                case NoteLength.DottedHalf:
                    return "Dotted Half";
                case NoteLength.Half:
                    return "Half";
                case NoteLength.DottedQuarter:
                    return "Dotted Quarter";
                case NoteLength.Quarter:
                    return "Quarter";
                case NoteLength.DottedEighth:
                    return "Dotted Eighth";
                case NoteLength.Eighth:
                    return "Egihth";
                case NoteLength.DottedSixeteenth:
                    return "Dotted Sixteenth";
                case NoteLength.Sixeteenth:
                    return "Sixteenth";
                case NoteLength.ThirtySecond:
                    return "Thirty-Second";
                default:
                    return "None";
            }
        }

        public static List<string> getAllLengthStrings()
        {
            return (from length in Enum.GetValues(typeof(NoteLength)).Cast<NoteLength>()
                    where length != NoteLength.None
                    select length.getStringFromNoteLength()).ToList();
        }
    }

    public class Length
    {
        public NoteLength NoteType { get; set; }

        public static Length createInstance(NoteLength length)
        {
            if (length == NoteLength.None) { return null; }
            else { return new Length(length); }
        }

        public static Length createInstance(NoteLength length, TupletType type)
        {
            if (type != TupletType.None) { return TupleLength.createInstance(length, type); }
            else { return createInstance(length); }
        }

        protected Length(NoteLength note_type)
        {
            NoteType = note_type;
        }

        public virtual int getLength()
        {
            return (int)NoteType;
        }
    }

    public class NonStandardLength : Length
    {
        private int non_standard_length;
        public NonStandardLength(int ns_length)
            :base(NoteLength.None)
        {
            non_standard_length = ns_length;
        }

        public override int getLength()
        {
            return non_standard_length;
        }
    }

    public class TupleLength : Length
    {
        public TupletType TupleType { get; }

        public static TupleLength createInstance(int note_length, int type)
        {
            if (NoteLengthExtensions.intIsValidNoteLength(note_length) && 
                TupletTypeExtensions.intIsValidTupletType(type))
            {
                return new TupleLength((NoteLength)note_length, (TupletType)type);
            }
            else { return null; }
        }

        public static TupleLength createInstance(NoteLength note_length, TupletType type)
        {
            if (note_length != NoteLength.None && type != TupletType.None)
            {
                return new TupleLength(note_length, type);
            }
            else { return null; }
        }

        private TupleLength(NoteLength note_length, TupletType type)
            :base(note_length)
        {
            TupleType = type;
        }

        public override int getLength()
        {
            return (int)NoteType * TupleType.getNumReplacing() / (int)TupleType;
        }
    }

    public class MeasureLength
    {
        private TimeSignature time_signature;

        public bool MeasureFull
        {
            get { return SpaceTaken >= TotalSpace; }
        }

        public int TotalSpace
        {
            get { return time_signature.MeasureLength; }
        }

        public int SpaceTaken { get; set; }

        /*
        * Constructor
        */
        public MeasureLength(TimeSignature sig)
        {
            time_signature = sig;
            SpaceTaken = 0;
        }

        /*
        * update methods
        */
        
        public void updateSpaceTaken(IEnumerable<Chord> chords)
        {
            int total = 0;
            foreach (Chord chord in chords)
            {
                total += chord.Length.getLength();
            }
            SpaceTaken = total;
        }
    }
}
