using ProtocolMaster.Model.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulinoDriver.Generator
{
    class Interval : IEquatable<Interval>, IComparable<Interval>
    {
        public uint begin, end;

        public Interval(uint begin, uint end)
        {
            this.begin = begin;
            this.end = end;
        }

        public int CompareTo(Interval other)
        {
            if (other == null) return 1;
            else return this.begin.CompareTo(other.begin);
        }

        public bool Equals(Interval other)
        {
            return (this.begin == other.begin && this.end == other.end);
        }

        public bool Overlap(Interval other)
        {
            if ((this.begin < other.end && this.end >= other.end) ||
                (other.begin < this.end && other.end >= this.end) ||
                (this.begin < other.begin && this.end >= other.begin) ||
                (other.begin < this.begin && other.end >= this.begin))
                return true;
            else return false;
        }
        public virtual ProtocolEvent ToProtocolEvent()
        {
            return null;
        }

    }
}
