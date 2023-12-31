using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChessLibrary.MoveGeneration
{
    public static class SlidingMoveUtilities
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ValidHVMoves(BitBoard b, int index, ulong occupied)
        {
            var square = b.GetSquare(index);
            ulong binaryS = BitBoardConstants.U1 << index;
            ulong fileMask = BitBoardConstants.FileMasks[(int)square.Square.File - 1];
            ulong rankMask = BitBoardConstants.RankMasks[square.Square.Rank - 1];
            ulong possibilitiesHorizontal =
                ((occupied & rankMask) - (2 * binaryS))
                ^ ((occupied & rankMask).ReverseBits() - 2 * binaryS.ReverseBits()).ReverseBits();
            ulong possibilitiesVertical =
                ((occupied & fileMask) - (2 * binaryS))
                ^ Extensions.ReverseBits(
                    Extensions.ReverseBits(occupied & fileMask)
                        - 2 * Extensions.ReverseBits(binaryS)
                ); // ((occupied & fileMask).ReverseBits() - 2 * binaryS.ReverseBits()).ReverseBits();
            return (possibilitiesHorizontal & rankMask) | (possibilitiesVertical & fileMask);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ValidDiagonalMoves(BitBoard b, int index, ulong occupied)
        {
            var square = b.GetSquare(index);
            ulong binaryS = BitBoardConstants.U1 << index;

            ulong diagonalMask = BitBoardConstants.GetDiagonalMask(square.Square);
            ulong antidiagonalMask = BitBoardConstants.GetAntiDiagonalMask(square.Square);

            ulong possibilitiesDiagonal =
                ((occupied & diagonalMask) - (2 * binaryS))
                ^ (
                    (occupied & diagonalMask).ReverseBits() - (2 * binaryS.ReverseBits())
                ).ReverseBits();
            ulong possibilitiesAntidiagonal =
                ((occupied & antidiagonalMask) - (2 * binaryS))
                ^ Extensions.ReverseBits(
                    Extensions.ReverseBits(occupied & antidiagonalMask)
                        - 2 * Extensions.ReverseBits(binaryS)
                );

            return (possibilitiesDiagonal & diagonalMask)
                | (possibilitiesAntidiagonal & antidiagonalMask);
        }
    }
}
