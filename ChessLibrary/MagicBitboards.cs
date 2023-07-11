using ChessLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChessLibrary
{
    public class MagicBitboards
    {
        public Dictionary<(int square, ulong mask), ulong> RookBitboards { get; set; } = new Dictionary<(int square, ulong mask), ulong>();
        public Dictionary<(int square, ulong mask), ulong> BishopBitBoards { get; set; } = new Dictionary<(int square, ulong mask), ulong>();

        public void Initialize()
        {
            InitRooks();
        }

        /// <summary>
        /// o o o x o o o o    x x x  x  x x x x   x  x x x x x x x
        /// o o o x o o o o    x o o  10 o o o x   12 o o o o o o x
        /// o o o x o o o o    x o o  9  o o o x   11 o o o o o o x
        /// o o o x o o o o    x o o  8  o o o x   10 o o o o o o x
        /// x x x R x x x x    x 7 6  R  5 4 3 x   9  o o o o o o x
        /// o o o x o o o o    x o o  2  o o o x   8  o o o o o o x
        /// o o o x o o o o    x o o  1  o o o x   7  o o o o o o x
        /// o o o x o o o o    x x x  x  x x x x   R  6 5 4 3 2 1 x
        /// </summary>                                
        private void InitRooks()
        {
            for (int i = 0; i <= 63; i++)
            {
                // Iterate through each square
                var board = (ulong)1 << i;
                int rank = 0;
                for (int r = 0; r < BitBoardMasks.RankMasks.Length; r++)
                {
                    if ((BitBoardMasks.RankMasks[r] & board) > 0)
                    {
                        rank = r;
                        break;
                    }
                }

                int file = 0;
                for (int f = 0; f < BitBoardMasks.FileMasks.Length; f++)
                {
                    if ((BitBoardMasks.FileMasks[f] & board) > 0)
                    {
                        file = f; // note, file A is 7, File H is 0
                        break;
                    }
                }

                ulong movementMask = (BitBoardMasks.RankMasks[rank] | BitBoardMasks.FileMasks[file]) & ~board;
                if (rank != 0)
                {
                    movementMask &= ~BitBoardMasks.RankMasks[0];
                }
                if (rank != 7)
                {
                    movementMask &= ~BitBoardMasks.RankMasks[7];
                }

                if (file != 7)
                {
                    movementMask &= ~BitBoardMasks.FileMasks[7];
                }

                if (file != 0)
                {
                    movementMask &= ~BitBoardMasks.FileMasks[0];
                }

                var blockerBitBoards = GetBlockerBitboards(movementMask);
                foreach(var blockerBitBoard in blockerBitBoards)
                {
                    ulong legalMoves = CreateRookLegalMoves(i, blockerBitBoard);
                    RookBitboards.Add((i, blockerBitBoard), legalMoves);
                }
            }
        }

        private static ulong CreateRookLegalMoves(int square, ulong blockerBitBoard)
        {
            ulong pieceBitBoard = 1UL << square;
            var edgeSquares = BitBoardMasks.FileA | BitBoardMasks.FileH | BitBoardMasks.Rank1 | BitBoardMasks.Rank8;
            if((pieceBitBoard & BitBoardMasks.FileA) > 0)
            {
                edgeSquares = BitBoardMasks.FileH | BitBoardMasks.Rank1 | BitBoardMasks.Rank8;
            }
            if ((pieceBitBoard & BitBoardMasks.FileH) > 0)
            {
                edgeSquares &= BitBoardMasks.FileA | BitBoardMasks.Rank1 | BitBoardMasks.Rank8;
            }
            if ((pieceBitBoard & BitBoardMasks.Rank1) > 0)
            {
                edgeSquares &= BitBoardMasks.FileA | BitBoardMasks.FileH | BitBoardMasks.Rank8;
            }
            if ((pieceBitBoard & BitBoardMasks.Rank8) > 0)
            {
                edgeSquares &= BitBoardMasks.FileA | BitBoardMasks.FileH | BitBoardMasks.Rank1;
            }
            var blockersAndEdges = blockerBitBoard | edgeSquares;


            ulong validSquares = 0;

            if((pieceBitBoard & BitBoardMasks.FileA) == 0)
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while(!foundBlocker)
                {
                    temporaryPiece = temporaryPiece << 1;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }

            if ((pieceBitBoard & BitBoardMasks.FileH) == 0)
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece >> 1;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }

            if ((pieceBitBoard & BitBoardMasks.Rank1) == 0)
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece >> 8;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }

            if ((pieceBitBoard & BitBoardMasks.Rank8) == 0)
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece << 8;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }
            return validSquares;
        }

        private static ulong[] GetBlockerBitboards(ulong movementMask)
        {
            List<int> moveSquareIndices = new List<int>();
            for (int index = 0; index < 64; index++)
            {
                if (((movementMask >> index) & 1) == 1)
                {
                    moveSquareIndices.Add(index);
                }
            }

            int numPatterns = 1 << moveSquareIndices.Count;
            ulong[] blockerBitboards = new ulong[numPatterns];

            for (int patternNumber = 0; patternNumber < numPatterns; patternNumber++)
            {
                for (int bitIndex = 0; bitIndex < moveSquareIndices.Count; bitIndex++)
                {
                    int bit = (patternNumber >> bitIndex) & 1;
                    blockerBitboards[patternNumber] |= (ulong)bit << moveSquareIndices[bitIndex];
                }
            }
            return blockerBitboards;
        }

        private void InitBishops()
        {
            
        }
    }
}
