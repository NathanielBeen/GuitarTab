using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public enum EffectType
    {
        No_Type = 0,
        Palm_Mute = 1, //combine (apply to chord -> apply to all notes in chord?)
        Bend = 2, //special stats
        Pinch_Harmonic = 3,
        Vibrato = 4, //special stats
        Slide = 5, //multi
        HOPO = 6, //multi, combine
        Tie = 7 //multi
    }

    public static class EffectTypeExtensions
    {
        public static EffectType getEffectTypeFromString(string name)
        {
            return (from enum_value in Enum.GetValues(typeof(EffectType)).Cast<EffectType>()
                    where enum_value.getMenuName() == name
                    select enum_value).FirstOrDefault();
        }

        public static bool isMultiNote(this EffectType type)
        {
            switch (type)
            {
                case EffectType.Slide:
                case EffectType.HOPO:
                case EffectType.Tie:
                    return true;
                default:
                    return false;
            }
        }

        public static string getMenuName(this EffectType type)
        {
            switch (type)
            {
                case EffectType.No_Type:
                    return "No Type";
                case EffectType.Palm_Mute:
                    return "Palm Mute";
                case EffectType.Bend:
                    return "Bend";
                case EffectType.Pinch_Harmonic:
                    return "Pinch Harmonic";
                case EffectType.Vibrato:
                    return "Vibrato";
                case EffectType.Slide:
                    return "Slide";
                case EffectType.HOPO:
                    return "HO/PO";
                case EffectType.Tie:
                    return "Tie";
                default:
                    return "";
            }
        }

        public static List<string> getSingleEffectNames(EffectPosition position)
        {
            var list = new List<string>();
            list.Add(getMenuName(EffectType.No_Type));
            switch (position)
            {
                case EffectPosition.Into:
                    break;
                case EffectPosition.Strike:
                    list.Add(getMenuName(EffectType.Palm_Mute));
                    list.Add(getMenuName(EffectType.Pinch_Harmonic));
                    break;
                case EffectPosition.After:
                    list.Add(getMenuName(EffectType.Bend));
                    list.Add(getMenuName(EffectType.Vibrato));
                    break;
            }
            return list;
        }

        public static List<string> getAllEffectNames(EffectPosition position)
        {
            var list = new List<string>();
            list.Add(getMenuName(EffectType.No_Type));
            switch (position)
            {
                case EffectPosition.Into:
                    list.Add(getMenuName(EffectType.HOPO));
                    list.Add(getMenuName(EffectType.Slide));
                    list.Add(getMenuName(EffectType.Tie));
                    break;
                case EffectPosition.Strike:
                    list.Add(getMenuName(EffectType.Palm_Mute));
                    list.Add(getMenuName(EffectType.Pinch_Harmonic));
                    break;
                case EffectPosition.After:
                    list.Add(getMenuName(EffectType.Bend));
                    list.Add(getMenuName(EffectType.Vibrato));
                    list.Add(getMenuName(EffectType.HOPO));
                    list.Add(getMenuName(EffectType.Slide));
                    list.Add(getMenuName(EffectType.Tie));
                    break;
            }
            return list;
        }
    }

    public enum EffectPosition
    {
        Into = 0,
        Strike = 1,
        After = 2
    }

    public static class EffectPositionExtensions
    {
        public static string getMenuName(this EffectPosition position)
        {
            switch (position)
            {
                case EffectPosition.Into:
                    return "Into Effect";
                case EffectPosition.Strike:
                    return "Strike Effect";
                case EffectPosition.After:
                    return "After Effect";
                default:
                    return "";
            }
        }
    }

    //put a breakeffect in this interface
    public interface IEffect
    {
        EffectType Type { get; }
        EffectPosition getPosition(Note note);
        void breakEffect(Note note);
    }

    public interface IMultiEffect : IEffect
    {
        Note First { get; }
        Note Second { get; }
    }

    public class PalmMute : IEffect
    {
        public EffectType Type
        {
            get { return EffectType.Palm_Mute; }
        }

        private PalmMute() { }

        public static IEffect createInstance()
        {
            return new PalmMute();
        }

        public EffectPosition getPosition(Note note) { return EffectPosition.Strike; }

        public void breakEffect(Note note) { }
    }

    public class Bend : IEffect
    {
        public EffectType Type
        {
            get { return EffectType.Bend; }
        }

        public double Amount { get; }
        public bool BendReturns { get; }

        private Bend(double amount, bool returns)
        {
            Amount = amount;
            BendReturns = returns;
        }

        public static IEffect createInstance(double? amount, bool? returns)
        {
            if (amount == null || returns == null) { return null; }
            return new Bend((double)amount, (bool)returns);
        }

        public EffectPosition getPosition(Note note) { return EffectPosition.After; }

        public void breakEffect(Note note) { }
    }

    public class PinchHarmonic : IEffect
    {
        public EffectType Type
        {
            get { return EffectType.Pinch_Harmonic; }
        }

        private PinchHarmonic() { }

        public static IEffect createInstance()
        {
            return new PinchHarmonic();
        }

        public EffectPosition getPosition(Note note) { return EffectPosition.Strike; }

        public void breakEffect(Note note) { }
    }

    public class Vibrato : IEffect
    {
        public EffectType Type
        {
            get { return EffectType.Vibrato; }
        }

        public bool Wide { get; }

        private Vibrato(bool wide)
        {
            Wide = wide;
        }

        public static IEffect createInstance(bool? wide)
        {
            if (wide == null) { return null; }
            return new Vibrato((bool)wide);
        }

        public EffectPosition getPosition(Note note) { return EffectPosition.After; }

        public void breakEffect(Note note) { }
    }

    public class Slide : IMultiEffect
    {
        public EffectType Type
        {
            get { return EffectType.Slide; }
        }

        public Note First { get; private set; }
        public Note Second { get; private set; }
        public bool Legato { get; }

        private Slide(Note first, Note second, bool legato)
        {
            First = first;
            Second = second;
            Legato = legato;
        }

        public static IMultiEffect createInstance(Note first, Note second, bool? legato)
        {
            if (first == null || second == null || legato == null) { return null; }
            return new Slide(first, second, (bool)legato);
        }

        public EffectPosition getPosition(Note note) { return (note.Equals(First)) ? EffectPosition.After : EffectPosition.Into; }

        public void breakEffect(Note note)
        {
            if (note.Equals(First))
            {
                First = null;
                Second?.Remove(this);
            }
            else
            {
                Second = null;
                First?.Remove(this);
            }
        }
    }

    public class HOPO : IMultiEffect
    {
        public EffectType Type
        {
            get { return EffectType.HOPO; }
        }

        public Note First { get; private set; }
        public Note Second { get; private set; }

        private HOPO(Note first, Note second)
        {
            First = first;
            Second = second;
        }

        public static IMultiEffect createInstance(Note first, Note second)
        {
            if (first == null || second == null) { return null; }
            return new HOPO(first, second);
        }

        public EffectPosition getPosition(Note note) { return (note.Equals(First)) ? EffectPosition.After : EffectPosition.Into; }

        public void breakEffect(Note note)
        {
            if (note.Equals(First))
            {
                First = null;
                Second?.Remove(this);
            }
            else
            {
                Second = null;
                First?.Remove(this);
            }
        }
    }

    public class Tie : IMultiEffect
    {
        public EffectType Type
        {
            get { return EffectType.Tie; }
        }

        public Note First { get; private set; }
        public Note Second { get; private set; }

        private Tie(Note first, Note second)
        {
            First = first;
            Second = second;
        }

        public static IMultiEffect createInstance(Note first, Note second)
        {
            if (first == null || second == null) { return null; }
            return new Tie(first, second);
        }

        public EffectPosition getPosition(Note note) { return (note.Equals(First)) ? EffectPosition.After : EffectPosition.Into; }

        public void breakEffect(Note note)
        {
            if (note.Equals(First))
            {
                First = null;
                Second?.Remove(this);
            }
            else
            {
                Second = null;
                First?.Remove(this);
            }
        }
    }
}
