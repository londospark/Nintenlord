using Nintenlord.Utility.Primitives;
using System;
using System.Collections.Generic;

namespace Nintenlord.MemoryManagement
{
    [Serializable]
    public struct OffsetSizePair : IEquatable<OffsetSizePair>, IComparable<OffsetSizePair>, IEnumerable<int>
    {
        public int Offset;
        public int Size;

        public OffsetSizePair(int offset, int size)
        {
            this.Offset = offset;
            this.Size = size;
        }

        #region IEquatable<OffsetSizePair> Members

        public bool Equals(OffsetSizePair other) =>
            this.Offset == other.Offset &&
            this.Size == other.Size;

        #endregion

        #region IComparable<OffsetSizePair> Members

        public int CompareTo(OffsetSizePair other) => this.Offset - other.Offset;

        #endregion

        #region IEnumerable<int> Members

        public IEnumerator<int> GetEnumerator()
        {
            for (var i = 0; i < Size; i++)
            {
                yield return Offset + i;
            }
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

        #endregion


        public override bool Equals(object? obj) => obj is OffsetSizePair pair && Equals(pair);

        public override int GetHashCode() => Offset ^ Size;

        public override string ToString() => $"Offset: ${Offset.ToHexString("")} Size: 0x{Size.ToHexString("")}";

        /// <summary>
        ///
        /// </summary>
        /// <param name="memory">Needs to be sorted and non-negative.</param>
        /// <returns></returns>
        public static IEnumerable<OffsetSizePair> EnumerateAsPairs(IEnumerable<int> memory)
        {
            var previous = -2;

            var start = -1;
            foreach (var item in memory)
            {
                if (previous + 1 != item)
                {
                    if (start >= 0)
                    {
                        yield return new OffsetSizePair(start, previous - start + 1);
                    }
                    start = item;
                }
            }
        }


    }
}
