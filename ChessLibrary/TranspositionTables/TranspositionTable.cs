using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary.TranspositionTables
{
    public class TranspositionTable
    {
        public const int MASK = 0xFFFFFF;
        public const int MAX_TRANSPOSITION_DEPTH = 3;
        private TranspositionTableEntry[] _internal { get; set; } =
            new TranspositionTableEntry[MASK + 1];

        public TranspositionTable()
        {
            for (int i = 0; i < MASK + 1; i++)
            {
                _internal[i] = new TranspositionTableEntry(MAX_TRANSPOSITION_DEPTH);
            }
        }

        public bool IsHashPopulatedForDepth(ulong zobristHash, int depth)
        {
            return _internal[zobristHash & MASK].Set
                && _internal[zobristHash & MASK].ZobristHash == zobristHash
                && _internal[zobristHash & MASK].IsScoreSet(depth);
        }

        public int this[ulong hash, int depth]
        {
            get
            {

                return _internal[hash & MASK][depth];
            }
            set
            {
                var entry = _internal[hash & MASK];
                if (!entry.Set)
                {
                    entry.Set = true;
                    entry.ZobristHash = hash;
                }
                if (entry.ZobristHash != hash)
                    return;
                entry[depth] = value;
            }
        }
    }
}
