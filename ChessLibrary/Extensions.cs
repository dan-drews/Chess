using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public static class Extensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string GetNotation(this PieceTypes p)
        {
            return p switch
            {
                PieceTypes.Bishop => "B",
                PieceTypes.King => "K",
                PieceTypes.Knight => "N",
                PieceTypes.Pawn => "P",
                PieceTypes.Queen => "Q",
                PieceTypes.Rook => "R",
                _ => ""
            };            
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int NumberOfTrailingZeros(this ulong n)
        {
            return System.Numerics.BitOperations.TrailingZeroCount(n);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ReverseBits(this ulong i)
        {
            i = (i & 0x5555555555555555) << 1 | (i >> 1) & 0x5555555555555555L;
            i = (i & 0x3333333333333333L) << 2 | (i >> 2) & 0x3333333333333333L;
            i = (i & 0x0f0f0f0f0f0f0f0fL) << 4 | (i >> 4) & 0x0f0f0f0f0f0f0f0fL;
            i = (i & 0x00ff00ff00ff00ffL) << 8 | (i >> 8) & 0x00ff00ff00ff00ffL;

            i = (i << 48) | ((i & 0xffff0000L) << 16) | ((i >> 16) & 0xffff0000L) | (i >> 48);
            return i;
        }

    }
}
