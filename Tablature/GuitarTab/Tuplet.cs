using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public enum TupletType
    {
        None = 0,
        Triplet = 3,
        Quintuplet = 5,
        Sextuplet = 6,
        Septuplet = 7,
        Nonuplet = 9
    }

    public static class TupletTypeExtensions
    {
        public static int getNumReplacing(this TupletType type)
        {
            switch (type)
            {
                case TupletType.Triplet:
                    return 2;
                case TupletType.Nonuplet:
                    return 8;
                default:
                    return 4;
            }
        }

        public static bool intIsValidTupletType(int type)
        {
            return Enum.IsDefined(typeof(TupletType), type);
        }

        public static TupletType getTupletTypeFromString(string type)
        {
            switch (type)
            {
                case "Triplet":
                    return TupletType.Triplet;
                case "Quintuplet":
                    return TupletType.Quintuplet;
                case "Sextuplet":
                    return TupletType.Sextuplet;
                case "Septuplet":
                    return TupletType.Septuplet;
                case "Nonuplet":
                    return TupletType.Nonuplet;
                default:
                    return TupletType.None;
            }
        }
    }

    public class ChordTuple
    {
        public bool HasTuple { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool DrawNumber { get; set; }
        public bool NumberRightOffset { get; set; }

        public TupletType Type { get; set; }
        public NoteLength NoteLength { get; set; }

        public ChordTuple(Length length) { readInLength(length); }

        public void readInLength(Length length)
        {
            if (length is TupleLength) { setTupleLength(length as TupleLength); }
            else { setStandardLength(length); }
        }

        public void setTupleLength(TupleLength length)
        {
            HasTuple = true;
            Left = false;
            Right = false;
            DrawNumber = true;
            NumberRightOffset = false;

            Type = length.TupleType;
            NoteLength = length.NoteType;
        }

        public void setStandardLength(Length length)
        {
            HasTuple = false;
            Left = false;
            Right = false;
            DrawNumber = false;
            NumberRightOffset = false;

            Type = default(TupletType);
            NoteLength = length.NoteType;
        }

        public NoteLength getHighestValidTupleBaseLength()
        {
            switch (NoteLength)
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
                    return NoteLength;
            }
        }

        public int getWeightFromBaseLength(NoteLength length) { return (int)NoteLength / (int)length; }
    }

    public class TupleGroup
    {
        public TupletType Type { get; }
        public List<ChordTuple> Tuples { get; }

        public TupleGroup(ChordTuple tuple)
        {
            Type = tuple.Type;
            Tuples = new List<ChordTuple>() { tuple };
        }

        public bool tryAddTuple(ChordTuple tuple)
        {
            if (tuple.HasTuple == true && tuple.Type == Type)
            {
                Tuples.Add(tuple);
                return true;
            }
            else { return false; }
        }

        public bool checkForFilledTuple()
        {
            List<ChordTuple> filled = getFilledTuple();
            if (filled == null) { return false; }
            else
            {
                setFilledTuple(filled);
                return true;
            }
        }

        public List<ChordTuple> getFilledTuple()
        {
            var checked_chord = new List<ChordTuple>();
            checked_chord.AddRange(Tuples);

            while(checked_chord.Count > 1)
            {
                int weight = getWeightOfTuples(checked_chord);

                if (weight > (int)Type) { checked_chord.RemoveAt(0); }
                else if (weight == (int)Type) { return checked_chord; }
                else { return null; }
            }

            return null;
        }

        public int getWeightOfTuples(List<ChordTuple> tuples)
        {
            NoteLength base_length = NoteLength.None;
            foreach (ChordTuple tuple in tuples)
            {
                NoteLength prop = tuple.getHighestValidTupleBaseLength();
                if (base_length == NoteLength.None || prop < base_length) { base_length = prop; }
            }

            int weight = 0;
            foreach (ChordTuple tuple in tuples)
            {
                weight += tuple.getWeightFromBaseLength(base_length);
            }
            return weight;
        }

        public void setFilledTuple(List<ChordTuple> tuples)
        {
            foreach (ChordTuple tuple in tuples)
            {
                tuple.Left = true;
                tuple.Right = true;
                tuple.DrawNumber = false;
            }
            tuples.First().Left = false;
            tuples.Last().Right = false;

            ChordTuple center = tuples.ElementAt((tuples.Count - 1) / 2);
            center.DrawNumber = true;
            if (tuples.Count % 2 == 0) { center.NumberRightOffset = true; }
        }
    }

    public static class TupletBarrer
    {
        public static void barMeasure(MeasureTreeNode measure_node)
        {
            List<ChordBounds> chords = (from node in measure_node.Children
                                        orderby (node as ChordTreeNode).getChord().Position.Index
                                        select node.ObjectBounds as ChordBounds).ToList();

            TupleGroup current_group = null;
            foreach (ChordBounds bounds in chords)
            {
                bounds.ChordTuple.readInLength(bounds.getChord().Length);

                if (current_group == null && bounds.ChordTuple.HasTuple) { current_group = new TupleGroup(bounds.ChordTuple); }
                else if (current_group != null)
                {
                    bool added = current_group.tryAddTuple(bounds.ChordTuple);
                    bool filled = current_group.checkForFilledTuple();

                    if (!added) { current_group = (bounds.ChordTuple.HasTuple) ? new TupleGroup(bounds.ChordTuple) : null; }
                    else if (filled) { current_group = null; }

                }
            }
        }
    }
}
