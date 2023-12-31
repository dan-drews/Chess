using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessLibrary.TableStructures
{
    public class RepetititionTracker : ICloneable
    {
        public ulong[] Hashes { get; set; } = new ulong[1024];
        public int Index { get; set; } = 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsRepetition(ulong hash, bool hasMoveBeenMade)
        {
            var starting = hasMoveBeenMade ? Index - 2 : Index - 1;
            for (int i = starting; i >= 0; i--)
            {
                if (Hashes[i] == hash)
                {
                    return true;
                }
            }
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int Count(ulong hash)
        {
            int count = 0;
            for (int i = Index - 1; i >= 0; i--)
            {
                if (Hashes[i] == hash)
                {
                    count++;
                }
            }
            return count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddHash(ulong hash)
        {
            Hashes[Index] = hash;
            Index++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void RemoveHash()
        {
            Index--;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public object Clone()
        {
            RepetititionTracker clone = new RepetititionTracker();
            clone.Hashes = (ulong[])Hashes.Clone();
            clone.Index = Index;
            return clone;
        }
    }
}
