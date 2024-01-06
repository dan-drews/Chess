using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLibrary.Boards;
using Newtonsoft.Json.Linq;

namespace ChessLibrary.MoveGeneration
{
    // For The mapping on the magic bitboards, the least significant bit will be the right-most
    // We will move from right to left, then bottom to top.

    public static class MagicSlidingImplementation
    {
        public static void InitializeMagicSliders() { }

        private static ulong[][] _rookMoves = new ulong[64][];
        private static Magic[] _rookMagics = new Magic[64];

        private static ulong[][] _bishopMoves = new ulong[64][];
        private static Magic[] _bishopMagics = new Magic[64];

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
        static MagicSlidingImplementation()
        {
            // Iterate through each square
            for (int i = 0; i <= 63; i++)
            {
                GenerateRookBoards(i);
                GenerateBishopBoards(i);
            }
        }

        private static void GenerateBishopBoards(int position)
        {
            // Figure out which diagonals this is in
            var board = (ulong)1 << position;
            int diagonal = 0;
            for (int d = 0; d < BitBoardConstants.DiagonalMasks.Length; d++)
            {
                if ((BitBoardConstants.DiagonalMasks[d] & board) > 0)
                {
                    diagonal = d;
                    break;
                }
            }

            int antidiagonal = 0;
            for (int a = 0; a < BitBoardConstants.AntiDiagonalMasks.Length; a++)
            {
                if ((BitBoardConstants.AntiDiagonalMasks[a] & board) > 0)
                {
                    antidiagonal = a;
                    break;
                }
            }

            ulong movementMask =
                (
                    BitBoardConstants.DiagonalMasks[diagonal]
                    | BitBoardConstants.AntiDiagonalMasks[antidiagonal]
                ) & ~board;
            movementMask &= ~BitBoardConstants.Edges;
            var blockerBitBoards = GetBlockerBitboards(movementMask);

            var blockers = new List<(ulong blockers, ulong moves)>(blockerBitBoards.Length);
            foreach (var blockerBitBoard in blockerBitBoards)
            {
                ulong legalMoves = CreateBishopLegalMoves(position, blockerBitBoard);
                legalMoves &= ~(BitBoardConstants.U1 << position);

                blockers.Add((blockerBitBoard, legalMoves));
            }
            var magic = MagicGenerator.GenerateMagicForSquare(blockers);
            _bishopMagics[position] = magic;
            _bishopMoves[position] = new ulong[(int)Math.Pow(2, magic.Shift)];
            foreach (var blocker in blockers)
            {
                var key = (blocker.blockers * magic.MagicNumber) >> (64 - magic.Shift);
                _bishopMoves[position][key] = blocker.moves;
            }
        }

        private static void GenerateRookBoards(int position)
        {
            // Figure out which rank and file this is in
            var board = (ulong)1 << position;
            int rank = 0;
            for (int r = 0; r < BitBoardConstants.RankMasks.Length; r++)
            {
                if ((BitBoardConstants.RankMasks[r] & board) > 0)
                {
                    rank = r;
                    break;
                }
            }

            int file = 0;
            for (int f = 0; f < BitBoardConstants.FileMasks.Length; f++)
            {
                if ((BitBoardConstants.FileMasks[f] & board) > 0)
                {
                    file = f; // note, file A is 7, File H is 0
                    break;
                }
            }

            ulong movementMask =
                (BitBoardConstants.RankMasks[rank] | BitBoardConstants.FileMasks[file]) & ~board;
            if (rank != 0)
            {
                movementMask &= ~BitBoardConstants.RankMasks[0];
            }
            if (rank != 7)
            {
                movementMask &= ~BitBoardConstants.RankMasks[7];
            }

            if (file != 7)
            {
                movementMask &= ~BitBoardConstants.FileMasks[7];
            }

            if (file != 0)
            {
                movementMask &= ~BitBoardConstants.FileMasks[0];
            }

            var blockerBitBoards = GetBlockerBitboards(movementMask);
            var blockers = new List<(ulong blockers, ulong moves)>(blockerBitBoards.Length);
            foreach (var blockerBitBoard in blockerBitBoards)
            {
                ulong legalMoves = CreateRookLegalMoves(position, blockerBitBoard);
                legalMoves &= ~(BitBoardConstants.U1 << position);

                blockers.Add((blockerBitBoard, legalMoves));
            }
            var magic = MagicGenerator.GenerateMagicForSquare(blockers);
            _rookMagics[position] = magic;
            _rookMoves[position] = new ulong[(int)Math.Pow(2, magic.Shift)];
            foreach (var blocker in blockers)
            {
                var key = (blocker.blockers * magic.MagicNumber) >> (64 - magic.Shift);
                _rookMoves[position][key] = blocker.moves;
            }
        }

        private static ulong CreateBishopLegalMoves(int square, ulong blockerBitBoard)
        {
            ulong pieceBitBoard = 1UL << square;
            var edgeSquares = GetEdgesForSquare(square);
            var blockersAndEdges = blockerBitBoard | edgeSquares;

            ulong validSquares = 0;

            if (
                (pieceBitBoard & BitBoardConstants.FileH) == 0
                && (pieceBitBoard & BitBoardConstants.Rank1) == 0
            )
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece >> 9;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }

            if (
                (pieceBitBoard & BitBoardConstants.FileH) == 0
                && (pieceBitBoard & BitBoardConstants.Rank8) == 0
            )
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece << 7;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }

            if (
                (pieceBitBoard & BitBoardConstants.Rank1) == 0
                && (pieceBitBoard & BitBoardConstants.FileA) == 0
            )
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece >> 7;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }

            if (
                (pieceBitBoard & BitBoardConstants.Rank8) == 0
                && (pieceBitBoard & BitBoardConstants.FileA) == 0
            )
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece << 9;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }
            return validSquares;
        }

        private static ulong CreateRookLegalMoves(int square, ulong blockerBitBoard)
        {
            ulong pieceBitBoard = 1UL << square;
            var edgeSquares = GetEdgesForSquare(square);
            var blockersAndEdges = blockerBitBoard | edgeSquares;

            ulong validSquares = 0;

            if ((pieceBitBoard & BitBoardConstants.FileA) == 0)
            {
                var temporaryPiece = pieceBitBoard;
                bool foundBlocker = false;
                while (!foundBlocker)
                {
                    temporaryPiece = temporaryPiece << 1;
                    foundBlocker = (temporaryPiece & blockersAndEdges) != 0;
                    validSquares |= temporaryPiece;
                }
            }

            if ((pieceBitBoard & BitBoardConstants.FileH) == 0)
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

            if ((pieceBitBoard & BitBoardConstants.Rank1) == 0)
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

            if ((pieceBitBoard & BitBoardConstants.Rank8) == 0)
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ValidBishopMoveBitBoard(
            FullBitBoard b,
            Colors color)
        {
            var bishops = color == Colors.White ? b.WhiteBishops : b.BlackBishops;
            ulong result = 0;
            var enumerator = bishops.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int location = enumerator.Current;
                result |= GetBishopValidMoveBitboard(b, color, location);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ValidRookMoveBitBoard(
            FullBitBoard b,
            Colors color)
        {
            var rooks = color == Colors.White ? b.WhiteRooks : b.BlackRooks;
            ulong result = 0;
            var enumerator = rooks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int location = enumerator.Current;
                result |= GetRookValidMoveBitboard(b, color, location);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong ValidQueenMoveBitBoard(
            FullBitBoard b,
            Colors color)
        {
            var rooks = color == Colors.White ? b.WhiteQueens : b.BlackQueens;
            ulong result = 0;
            var enumerator = rooks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int location = enumerator.Current;
                result |= GetRookValidMoveBitboard(b, color, location) | GetBishopValidMoveBitboard(b, color, location);
            }
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GetBishopValidMoveBitboard(FullBitBoard b, Colors color, int location)
        {
            var square = b.GetSquare(location).Square;
            ulong diagonalMask = BitBoardConstants.GetDiagonalMask(square);
            ulong antidiagonalMask = BitBoardConstants.GetAntiDiagonalMask(square);

            return GetMoveBitBoardFromMasksAndSquare(b, location, color, diagonalMask | antidiagonalMask, _bishopMagics, _bishopMoves);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GetRookValidMoveBitboard(FullBitBoard b, Colors color, int location)
        {
            var square = b.GetSquare(location).Square;
            var rank = square.Rank;
            var file = square.File;

            var rankMask = BitBoardConstants.RankMasks[rank - 1];
            var fileMask = BitBoardConstants.FileMasks[(int)file - 1];

            return GetMoveBitBoardFromMasksAndSquare(b, location, color, rankMask | fileMask, _rookMagics, _rookMoves);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ValidBishopMoves(
            FullBitBoard b,
            Colors color,
            bool includeQuietMoves,
            List<Move> result
        )
        {
            var bishops = color == Colors.White ? b.WhiteBishops : b.BlackBishops;

            var enumerator = bishops.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int location = enumerator.Current;

                var moves = GetBishopValidMoveBitboard(b, color, location);
                GetMovesFromMoveBitBoard(b, color, includeQuietMoves, result, location, PieceTypes.Bishop, moves);
            }
        }

        public static void ValidRookMoves(
            FullBitBoard b,
            Colors color,
            bool includeQuietMoves,
            List<Move> result
        )
        {
            var rooks = color == Colors.White ? b.WhiteRooks : b.BlackRooks;

            var enumerator = rooks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int location = enumerator.Current;
                var moves = GetRookValidMoveBitboard(b, color, location);
                GetMovesFromMoveBitBoard(b, color, includeQuietMoves, result, location, PieceTypes.Rook, moves);
                //var square = b.GetSquare(location).Square;
                //var rank = square.Rank;
                //var file = square.File;

                //var rankMask = BitBoardConstants.RankMasks[rank - 1];
                //var fileMask = BitBoardConstants.FileMasks[(int)file - 1];
                //GetMovesFromMasksAndSquare(
                //    b,
                //    color,
                //    includeQuietMoves,
                //    result,
                //    location,
                //    rankMask | fileMask,
                //    _rookMagics,
                //    _rookMoves,
                //    PieceTypes.Rook
                //);
            }
        }

        public static void ValidQueenMoves(
            FullBitBoard b,
            Colors color,
            bool includeQuietMoves,
            List<Move> result
        )
        {
            var queens = color == Colors.White ? b.WhiteQueens : b.BlackQueens;

            var enumerator = queens.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int location = enumerator.Current;

                var moves = GetBishopValidMoveBitboard(b, color, location) | GetRookValidMoveBitboard(b, color, location);
                GetMovesFromMoveBitBoard(b, color, includeQuietMoves, result, location, PieceTypes.Queen, moves);
                //var square = b.GetSquare(location).Square;
                //var rank = square.Rank;
                //var file = square.File;

                //var rankMask = BitBoardConstants.RankMasks[rank - 1];
                //var fileMask = BitBoardConstants.FileMasks[(int)file - 1];
                //ulong diagonalMask = BitBoardConstants.GetDiagonalMask(square);
                //ulong antidiagonalMask = BitBoardConstants.GetAntiDiagonalMask(square);

                //GetMovesFromMasksAndSquare(
                //    b,
                //    color,
                //    includeQuietMoves,
                //    result,
                //    location,
                //    rankMask | fileMask,
                //    _rookMagics,
                //    _rookMoves,
                //    PieceTypes.Queen
                //);
                //GetMovesFromMasksAndSquare(
                //    b,
                //    color,
                //    includeQuietMoves,
                //    result,
                //    location,
                //    diagonalMask | antidiagonalMask,
                //    _bishopMagics,
                //    _bishopMoves,
                //    PieceTypes.Queen
                //);
            }
        }

        public static ulong GetMoveBitBoardFromMasksAndSquare(FullBitBoard b,
            int location,
            Colors color,
            ulong masks,
            Magic[] magics,
            ulong[][] moves
        )
        {
            ulong edges = GetEdgesForSquare(location);

            ulong occupiedMinusEdges = b.OccupiedSquares & ~edges;
            ulong slidingSetMinusCurrent = masks & ~(BitBoardConstants.U1 << location);

            var blockers = occupiedMinusEdges & slidingSetMinusCurrent;

            var magic = magics[location];
            var key = (blockers * magic.MagicNumber) >> (64 - magic.Shift);
            ulong validMoves = moves[location][key];
            return validMoves & ~b.GetAllPieces(color);
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //private static void GetMovesFromMasksAndSquare(
        //    FullBitBoard b,
        //    Colors color,
        //    bool includeQuietMoves,
        //    List<Move> result,
        //    int location,
        //    ulong masks,
        //    Magic[] magics,
        //    ulong[][] moves,
        //    PieceTypes pieceType
        //)
        //{
        //    var validMoves = GetMoveBitBoardFromMasksAndSquare(b, location, color, masks, magics, moves);
        //    GetMovesFromMoveBitBoard(b, color, includeQuietMoves, result, location, pieceType, validMoves);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void GetMovesFromMoveBitBoard(FullBitBoard b, Colors color, bool includeQuietMoves, List<Move> result, int location, PieceTypes pieceType, ulong validMoves)
        {
            var enumerator = validMoves.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var index = enumerator.Current;
                var destinationSquare = b.GetSquare(index);
                var piece = destinationSquare.Piece;
                if (piece != null || includeQuietMoves)
                {
                    var move = new Move(
                        location,
                        index,
                        color,
                        pieceType,
                        piece?.Type
                    );
                    result.Add(move);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static ulong GetEdgesForSquare(int square)
        {
            ulong pieceBitBoard = 1UL << square;
            var edgeSquares = BitBoardConstants.Edges;
            if ((pieceBitBoard & BitBoardConstants.FileA) > 0)
            {
                if ((pieceBitBoard & BitBoardConstants.Rank1) > 0)
                {
                    return BitBoardConstants.FileH | BitBoardConstants.Rank8;
                }
                if ((pieceBitBoard & BitBoardConstants.Rank8) > 0)
                {
                    return BitBoardConstants.FileH | BitBoardConstants.Rank1;
                }
                return BitBoardConstants.FileH | BitBoardConstants.Rank1 | BitBoardConstants.Rank8;
            }

            if ((pieceBitBoard & BitBoardConstants.FileH) > 0)
            {
                if ((pieceBitBoard & BitBoardConstants.Rank1) > 0)
                {
                    return BitBoardConstants.FileA | BitBoardConstants.Rank8;
                }
                if ((pieceBitBoard & BitBoardConstants.Rank8) > 0)
                {
                    return BitBoardConstants.FileA | BitBoardConstants.Rank1;
                }
                return BitBoardConstants.FileA | BitBoardConstants.Rank1 | BitBoardConstants.Rank8;
            }

            if ((pieceBitBoard & BitBoardConstants.Rank1) > 0)
            {
                return BitBoardConstants.FileA | BitBoardConstants.FileH | BitBoardConstants.Rank8;
            }

            if ((pieceBitBoard & BitBoardConstants.Rank8) > 0)
            {
                return BitBoardConstants.FileA | BitBoardConstants.FileH | BitBoardConstants.Rank1;
            }
            return edgeSquares;
        }
    }
}
