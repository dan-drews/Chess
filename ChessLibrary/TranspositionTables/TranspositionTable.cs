using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ChessLibrary.TranspositionTables
{
    public class TranspositionTable
    {
        public const int MASK = 0xFFFFFF;
        public const int MAX_TRANSPOSITION_DEPTH = 6;
        private TranspositionTableEntry[] _internal { get; set; } =
            new TranspositionTableEntry[MASK];

        public long HitCount;
        public long MissCount;
        public long CollissionCount;

        public TranspositionTable()
        {
            for (int i = 0; i < _internal.Length; i++)
            {
                _internal[i] = new TranspositionTableEntry(MAX_TRANSPOSITION_DEPTH);
            }
        }

        public bool IsHashPopulatedForDepth(ulong zobristHash, int depth)
        {
            var entry = _internal[(int)zobristHash & MASK];
            bool isPopulated = entry.Set
                && entry.ZobristHash == zobristHash
                && entry.IsScoreSet(depth);
            if(isPopulated)
            {
                Interlocked.Increment(ref HitCount);
            }
            else
            {
                Interlocked.Increment(ref MissCount);
            }
            return isPopulated;
        }

        public int this[ulong hash, int depth]
        {
            get
            {
                return _internal[(int)hash & MASK][depth];
            }
            set
            {
                if(depth >= MAX_TRANSPOSITION_DEPTH)
                {
                    return;
                }

                var entry = _internal[(int)hash & MASK];
                
                if (!entry.Set)
                    entry.SetHash(hash);

                if (entry.ZobristHash != hash)
                {
                    Interlocked.Increment(ref CollissionCount);
                    return;
                }

                entry[depth] = value;
            }
        }
    }
}
