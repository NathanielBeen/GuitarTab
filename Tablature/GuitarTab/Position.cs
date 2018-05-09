using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GuitarTab
{
    public interface IPosition
    {
        Position Position { get; set; }
    }

    public class Position
    {
        public int Index { get; set; }
        public bool IsFirst { get { return Index == 0; } }
        public bool IsLast { get; set; }

        public Position(int index, bool is_last)
        {
            Index = index;
            IsLast = is_last;
        }

        public virtual bool occursBefore(Position other) { return (Index - other.Index == -1); }
        public virtual bool occursAfter(Position other) { return (Index - other.Index == 1); }
        public virtual bool indexMatches(Position other) { return (Index == other.Index && !(other is MultiPosition)); }

        public int getPreviousPosition(int val_if_first)
        {
            if (IsFirst) { return val_if_first; }
            else { return Index - 1; }
        }

        public int getNextPosition()
        {
            if (IsLast) { return 0; }
            else { return Index + 1; }
        }
    }

    public class MultiPosition : Position
    {
        private Position measure_position;

        public int MeasureIndex
        {
            get { return measure_position.Index; }
        }

        public MultiPosition(Position measure_pos, int pos, bool is_last)
            :base(pos, is_last)
        {
            measure_position = measure_pos;
        }

        public void setMeasureReference(Position m_pos)
        {
            if (m_pos != null) { measure_position = m_pos; }
        }

        public override bool occursBefore(Position other)
        {
            var m_other = other as MultiPosition;
            if (m_other is null) { return false; }
            if (MeasureIndex == m_other.MeasureIndex && Index == other.Index - 1) { return true; }
            if (MeasureIndex == m_other.MeasureIndex - 1 && IsLast && m_other.IsFirst) { return true; }
            return false;
        }

        public override bool occursAfter(Position other)
        {
            var m_other = other as MultiPosition;
            if (m_other is null) { return false; }
            if (MeasureIndex == m_other.MeasureIndex && Index == m_other.Index + 1) { return true; }
            if (MeasureIndex == m_other.MeasureIndex + 1 && IsFirst && m_other.IsLast) { return true; }
            return false;
        }

        public override bool indexMatches(Position other)
        {
            var m_other = other as MultiPosition;
            if (m_other is null) { return false; }
            return (Index == m_other.Index && MeasureIndex == m_other.MeasureIndex);
        }

        public int getPreviousMeasurePosition()
        {
            if (IsFirst) { return MeasureIndex - 1; }
            else { return MeasureIndex; }
        }

        public int getNextMeasurePosition()
        { 
            if (IsLast) { return MeasureIndex + 1; }
            else { return MeasureIndex; }
        }
    }

    public class PositionComparer<T> : IComparer<T>
        where T : IPosition
    {
        public int Compare(T x, T y)
        {
            return x.Position.Index - (y.Position.Index);
        }
    }
}
