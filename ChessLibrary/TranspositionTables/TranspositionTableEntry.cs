using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary.TranspositionTables
{
    public class TranspositionTableEntry
    {
        public bool Set { get; set; }
        public ulong ZobristHash { get; set; }
        public bool[] DepthSet { get; set; }
        public int[] Scores { get; set; }

        public TranspositionTableEntry(int maxDepth)
        {
            DepthSet = new bool[maxDepth];
            for (int i = 0; i < maxDepth; i++)
            {
                DepthSet[i] = false;
            }
            Scores = new int[maxDepth];
        }

        public int this[int depth]
        {
            get
            {
                lock (this)
                {
                    return Scores[depth];
                }
            }
            set
            {
                lock (this)
                {
                    if (depth >= Scores.Length)
                    {
                        return;
                    }
                    Scores[depth] = value;
                    DepthSet[depth] = true;
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
                return DepthSet[depth];
            }
        }
    }
}
