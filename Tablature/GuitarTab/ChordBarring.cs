using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public class ChordBar
    {
        private bool dotted;

        public int TotalBars { get; private set; }
        public int RightConnected { get; set; }
        public int LeftConnected { get; set; }
        public int RightSingle { get; private set; }
        public int LeftSingle { get; private set; }
        public bool LeftDot { get; private set; }
        public bool RightDot { get; private set; }

        public ChordBar(NoteLength length) { updateNoteLength(length); }

        public void updateNoteLength(NoteLength length)
        {
            TotalBars = getTotalBars(length);
            dotted = length.isDotted();
        }

        public int getTotalBars(NoteLength length)
        {
            switch (length)
            {
                case NoteLength.ThirtySecond:
                    return 3;
                case NoteLength.Sixeteenth:
                case NoteLength.DottedSixeteenth:
                    return 2;
                case NoteLength.Eighth:
                case NoteLength.DottedEighth:
                    return 1;
                default:
                    return 0;
            }
        }

        public void resetBars(NoteLength length)
        {
            updateNoteLength(length);
            RightConnected = 0;
            LeftConnected = 0;
            RightSingle = 0;
            LeftSingle = 0;
            LeftDot = false;
            RightDot = false;
        }

        public void barChord(bool connected, bool last)
        {
            if (connected) { barInConnectedBeat(); }
            else { barInNonConnectedBeat(); }
            setDot(last);
        }

        public void barInConnectedBeat()
        {
            int to_bar = TotalBars - Math.Max(RightConnected, LeftConnected);
            if (RightConnected >= LeftConnected) { RightSingle = to_bar; }
            else { LeftSingle = to_bar; }
        }

        public void barInNonConnectedBeat()
        {
            RightSingle = TotalBars;
        }

        public void setDot(bool last_note)
        {
            if (!dotted) { return; }
            if (last_note) { LeftDot = true; }
            else { RightDot = true; }
        }
    }

    public class BarredBeat
    {
        private int beat_length;
        private double space_taken;

        public List<ChordBounds> Chords;

        public BarredBeat(int length, List<ChordBounds> measure_chords)
        {
            beat_length = length;
            space_taken = 0;

            Chords = getBeat(measure_chords);
        }

        public List<ChordBounds> getBeat(List<ChordBounds> measure_chords)
        {
            var chords = new List<ChordBounds>();

            while (needsToAddChord())
            {
                ChordBounds to_add = measure_chords.FirstOrDefault();
                if (to_add is null) { return chords; }

                chords.Add(to_add);
                space_taken = NoteLengthExtensions.roundIfWithinDoubleError(space_taken + to_add.getChord().Length.getLength());
                measure_chords.Remove(to_add);
            }

            return chords;
        }

        public bool needsToAddChord() { return ((space_taken % beat_length) != 0 || space_taken == 0); }

        public void barBeat()
        {
            foreach (ChordBounds chord in Chords)
            {
                chord.ChordBar.resetBars(chord.getChord().Length.NoteType);
            }
            bool connected = (beat_length == space_taken);
            if (connected)
            {
                setConnectedBars();
            }
            foreach (ChordBounds chord in Chords)
            {
                bool last = chord.Equals(Chords.Last());
                chord.ChordBar.barChord(connected, last);
            }
        }

        public void setConnectedBars()
        {
            ChordBounds prev_chord = null;
            foreach (ChordBounds chord in Chords)
            {
                if (prev_chord != null && prev_chord.Bounds.Bar == chord.Bounds.Bar)
                {
                    int prev_conn = Math.Min(prev_chord.ChordBar.TotalBars, chord.ChordBar.TotalBars);
                    prev_chord.ChordBar.RightConnected = prev_conn;
                    chord.ChordBar.LeftConnected = prev_conn;
                }
                prev_chord = chord;
            }
        }
    }

    public static class MeasureBarrer
    {
        public static void barMeasure(MeasureTreeNode measure_node)
        {
            var barred_beats = genBeats(measure_node);
            foreach (BarredBeat beat in barred_beats) { beat.barBeat(); }
        }

        public static List<BarredBeat> genBeats(MeasureTreeNode measure_node)
        {
            List<ChordBounds> chords = (from node in measure_node.Children
                                        orderby (node as ChordTreeNode).getChord().Position.Index
                                        select node.ObjectBounds as ChordBounds).ToList();
            int beat_length = (int)measure_node.getMeasure().TimeSignature.BeatType;
            var beats = new List<BarredBeat>();
            while (chords.Any())
            {
                beats.Add(new BarredBeat(beat_length, chords));
            }

            return beats;
        }
    }
}
