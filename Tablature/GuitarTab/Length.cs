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
        DottedWhole = 48,
        Whole = 32,
        DottedHalf = 24,
        Half = 16,
        DottedQuarter = 12,
        Quarter = 8,
        DottedEighth = 6,
        Eighth = 4,
        DottedSixeteenth = 3,
        Sixeteenth = 2,
        ThirtySecond = 1
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

        public static double roundIfWithinDoubleError(double length)
        {
            int target = (int)Math.Round(length);
            if (length <= target + .01 && length >= target - .01) { return target; }
            return length;
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

        public Length(NoteLength note_type)
        {
            NoteType = note_type;
        }

        public virtual double getLength()
        {
            return (double)NoteType;
        }
    }

    public class NonStandardLength : Length
    {
        private double non_standard_length;
        public NonStandardLength(double ns_length)
            :base(NoteLength.None)
        {
            non_standard_length = ns_length;
        }

        public override double getLength()
        {
            return non_standard_length;
        }
    }

    public class GroupedLength : Length
    {
        public GroupFraction GroupFraction { get; }

        public GroupedLength(NoteLength length, GroupFraction fraction)
            :base(length)
        {
            GroupFraction = fraction;
        }

        public override double getLength()
        {
            return GroupFraction.getLength();
        }
    }

    public struct GroupFraction
    {
        public int Weight { get; }
        public int SplitInto { get; }
        public NoteLength Replacing { get; }

        public GroupFraction(int weight, int split, NoteLength replacing)
        {
            Weight = weight;
            SplitInto = split;
            Replacing = replacing;
        }

        public bool mayAddFraction(GroupFraction other)
        {
            return (Replacing == other.Replacing && SplitInto == other.SplitInto);
        }

        public GroupFraction addFraction(GroupFraction other)
        {
            return new GroupFraction(Weight + other.Weight, SplitInto, Replacing);
        }

        public double getLength()
        {
            return (int)Replacing / SplitInto * Weight;
        }
    }

    public static class GroupFractionCalulator
    {
        public static List<GroupFraction> simplifyFractionList(List<GroupFraction> to_simplify)
        {
            var simplified_list = new List<GroupFraction>();
            foreach (GroupFraction fraction in to_simplify) { addFractionToList(simplified_list, fraction); }
            return simplified_list;
        }

        public static void addFractionToList(List<GroupFraction> list, GroupFraction fraction)
        {
            foreach (GroupFraction other_fraction in list)
            {
                if (fraction.mayAddFraction(other_fraction))
                {
                    GroupFraction new_fraction = fraction.addFraction(other_fraction);
                    list.Remove(other_fraction);
                    list.Add(new_fraction);
                    return;
                }
            }

            list.Add(fraction);
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

        public double SpaceTaken { get; set; }

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
            SpaceTaken = NoteLengthExtensions.roundIfWithinDoubleError(calcNonGroupedLength(chords) + calcGroupedLength(chords));
        }

        /*
        * Helper Methods
        */
        public double calcNonGroupedLength(IEnumerable<Chord> chords)
        {
            double total = 0;
            foreach (Chord chord in chords)
            {
                if (!(chord.Length is GroupedLength)) { total += chord.Length.getLength(); }
            }
            return total;
        }

        public double calcGroupedLength(IEnumerable<Chord> chords)
        {
            List<GroupFraction> grouped_chords = (from chord in chords
                                                  where chord.Length is GroupedLength
                                                  select ((GroupedLength)chord.Length).GroupFraction).ToList();
            List<GroupFraction> simplified_list = GroupFractionCalulator.simplifyFractionList(grouped_chords);

            double total = 0;
            foreach (GroupFraction frac in simplified_list){ total += frac.getLength(); }
            return total;
        }
    }
}
