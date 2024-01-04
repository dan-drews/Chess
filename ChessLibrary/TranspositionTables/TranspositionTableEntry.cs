using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessLibrary.TranspositionTables
{
    public class TranspositionTableEntry
    {
        public bool Set;
        public ulong ZobristHash;

        public bool[] DepthSet;
        public int[] Scores;

        public TranspositionTableEntry(int maxDepth)
        {
            DepthSet = new bool[maxDepth];
            Scores = new int[maxDepth];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetHash(ulong hash)
        {
            ZobristHash = hash;
            Set = true;
        }

        public int this[int depth]
        {
            get
            {
                lock (this)
                {
                    return Scores[depth - 1];
                }
            }
            set
            {
                if (depth >= Scores.Length)
                {
                    return;
                }
                lock (this)
                {
                    Scores[depth - 1] = value;
                    DepthSet[depth - 1] = true;
                }
            }
        }

        public bool IsScoreSet(int depth)
        {
            if (depth >= DepthSet.Length)
            {
                return false;
            }
            lock (this)
            {
                return DepthSet[depth-1];
            }
        }
    }
}
