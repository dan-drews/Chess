﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ChessLibrary.MoveGeneration;
using static ChessLibrary.BitBoardConstants;

[assembly: InternalsVisibleTo("Chess.Tests")]

namespace ChessLibrary
{
    public class BitBoard : ICloneable
    {
        public ulong WhitePawns;
        public ulong WhiteRooks;
        public ulong WhiteKnights;
        public ulong WhiteBishops;
        public ulong WhiteQueens;
        public ulong WhiteKing;

        public ulong BlackPawns;
        public ulong BlackRooks;
        public ulong BlackKnights;
        public ulong BlackBishops;
        public ulong BlackQueens;
        public ulong BlackKing;

        private static SquareState[] _emptySquareStates;
        private static Square[] _emptySquares;

        static BitBoard()
        {
            _emptySquareStates = new SquareState[64];
            _emptySquares = new Square[64];
            for (int i = 0; i < 64; i++)
            {
                _emptySquares[i] = new Square(i);
                _emptySquareStates[i] = new SquareState(_emptySquares[i]);
            }
        }

        public void ClearCache()
        {
            for (int i = 0; i < _squares.Length; i++)
            {
                _squares[i] = null;
            }
        }

        public void SetupBoard()
        {
            WhitePawns = Starting_White_Pawns;
            WhiteRooks = Starting_White_Rooks;
            WhiteKnights = Starting_White_Knights;
            WhiteBishops = Starting_White_Bishops;
            WhiteQueens = Starting_White_Queens;
            WhiteKing = Starting_White_King;

            BlackPawns = Starting_Black_Pawns;
            BlackRooks = Starting_Black_Rooks;
            BlackKnights = Starting_Black_Knights;
            BlackBishops = Starting_Black_Bishops;
            BlackQueens = Starting_Black_Queens;
            BlackKing = Starting_Black_King;
        }

        public object Clone()
        {
            var bb = new BitBoard();
            bb.WhitePawns = WhitePawns;
            bb.WhiteRooks = WhiteRooks;
            bb.WhiteKnights = WhiteKnights;
            bb.WhiteBishops = WhiteBishops;
            bb.WhiteQueens = WhiteQueens;
            bb.WhiteKing = WhiteKing;

            bb.BlackPawns = BlackPawns;
            bb.BlackRooks = BlackRooks;
            bb.BlackKnights = BlackKnights;
            bb.BlackBishops = BlackBishops;
            bb.BlackQueens = BlackQueens;
            bb.BlackKing = BlackKing;
            bb._squares = new SquareState[64];
            Array.Copy(_squares, bb._squares, 64);
            return bb;
        }

        public ulong OccupiedSquares
        {
            get
            {
                return WhitePawns
                    | BlackPawns
                    | WhiteRooks
                    | BlackRooks
                    | WhiteKnights
                    | BlackKnights
                    | WhiteBishops
                    | BlackBishops
                    | WhiteQueens
                    | BlackQueens
                    | WhiteKing
                    | BlackKing;
            }
        }

        public ulong EmptySquares
        {
            get { return ~OccupiedSquares; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal int GetPositionFromFileAndRank(Files file, int rank)
        {
            // In our bitboard, A8 == 63 H1 == 0.
            return ((rank - 1) * 8) + (8 - ((int)file));
        }

        private SquareState?[] _squares = new SquareState[64];

        public SquareState GetSquare(Files file, int rank)
        {
            int position = GetPositionFromFileAndRank(file, rank);
            return GetSquare(position);
        }

        public SquareState GetSquare(int position)
        {
            if (_squares[position] != null)
            {
                return _squares[position]!;
            }
            _squares[position] = GetSquareInternal(position);
            return _squares[position]!;
        }

        private SquareState GetSquareInternal(int position)
        {
            ulong squareMask = (U1 << position);
            ulong occupiedSquares = OccupiedSquares;

            if ((occupiedSquares & squareMask) == 0)
            {
                return _emptySquareStates[position];
            }

            // Casting from enum to int is taking a lot of CPU power
            // So we are avoiding using enum values here

            if ((WhitePawns & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][0]; // White Pawn

            if ((BlackPawns & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][0]; // Black Pawn

            if ((WhiteRooks & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][1]; // White Rook

            if ((BlackRooks & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][1]; // Black Rook

            if ((WhiteKnights & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][2]; // White Knight

            if ((BlackKnights & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][2]; // Black Knight

            if ((WhiteBishops & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][3]; // White Bishop

            if ((BlackBishops & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][3]; // Black Bishop

            if ((WhiteQueens & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][4]; // White Queen

            if ((BlackQueens & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][4]; // Black Queen

            if ((WhiteKing & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][5]; // White King

            if ((BlackKing & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][5]; // Black King

            throw new Exception("Not sure what piece is here.");
        }

        public void MovePiece(Move move)
        {
            // En Passant
            if (move.Flags == Flag.EnPassantCapture) // move.Piece == PieceTypes.Pawn && move.CapturedPiece != null && GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece == null)
            {
                if (move.Color == Colors.White)
                {
                    ClearPiece(move.TargetSquare - 8);
                }
                else
                {
                    ClearPiece(move.TargetSquare + 8);
                }
                ClearPiece(move.TargetSquare);
            }

            // Castle
            if (move.Flags == Flag.ShortCastle)
            {
                var targetSquare = new Square(move.TargetSquare);
                ClearPiece(Files.H, targetSquare.Rank);
                SetPiece(Files.F, targetSquare.Rank, PieceTypes.Rook, move.Color);
            }
            if (move.Flags == Flag.LongCastle)
            {
                // Long Castle
                var targetSquare = new Square(move.TargetSquare);
                ClearPiece(Files.A, targetSquare.Rank);
                SetPiece(Files.D, targetSquare.Rank, PieceTypes.Rook, move.Color);
            }

            ClearPiece(move.TargetSquare);
            var newPiece = move.PromotedType ?? move.Piece;

            SetPiece(move.TargetSquare, newPiece, move.Color);
            ClearPiece(move.StartingSquare);
            _threatenedSquares = null;
            _whiteUnsafe = null;
            _blackUnsafe = null;
            for (int i = 0; i < _squares.Length; i++)
            {
                _squares[i] = null;
            }
        }

        public ulong GetAllPieces(Colors color)
        {
            switch (color)
            {
                case Colors.Black:
                    return BlackPawns
                        | BlackRooks
                        | BlackKnights
                        | BlackBishops
                        | BlackQueens
                        | BlackKing;
                case Colors.White:
                    return WhitePawns
                        | WhiteRooks
                        | WhiteKnights
                        | WhiteBishops
                        | WhiteQueens
                        | WhiteKing;
                default:
                    throw new Exception("");
            }
        }

        private ulong? _whiteUnsafe = null;
        private ulong? _blackUnsafe = null;

        public ulong Unsafe(Colors color)
        {
            if (color == Colors.White && _whiteUnsafe != null)
            {
                return _whiteUnsafe.Value;
            }
            if (color == Colors.Black && _blackUnsafe != null)
            {
                return _blackUnsafe.Value;
            }
            ulong unsafeSpaces;
            if (color == Colors.White)
            {
                unsafeSpaces = (BlackPawns >> 7) & ~BitBoardConstants.FileH; // Pawn Capture Left
                unsafeSpaces |= (BlackPawns >> 9) & ~BitBoardConstants.FileA; // Pawn Capture Right
            }
            else
            {
                unsafeSpaces = (WhitePawns << 7) & ~BitBoardConstants.FileA; // Pawn Capture Left
                unsafeSpaces |= (WhitePawns << 9) & ~BitBoardConstants.FileH; // Pawn Capture Right
            }

            ulong possibilities;
            // knight
            ulong knight = color == Colors.White ? BlackKnights : WhiteKnights;
            ulong i = knight & ~(knight - 1);
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                if (location > KnightRangeBaseSquare)
                {
                    possibilities = KnightSpan << (location - KnightRangeBaseSquare);
                }
                else
                {
                    possibilities = KnightSpan >> (KnightRangeBaseSquare - location);
                }

                if (location % 8 >= 4)
                {
                    possibilities &= ~(BitBoardConstants.FileG | BitBoardConstants.FileH);
                }
                else
                {
                    possibilities &= ~(BitBoardConstants.FileA | BitBoardConstants.FileB);
                }
                unsafeSpaces |= possibilities;
                knight &= ~i;
                i = knight & ~(knight - 1);
            }

            // Bishop/queen
            ulong queensAndBishops =
                color == Colors.White ? BlackBishops | BlackQueens : WhiteBishops | WhiteQueens;
            i = queensAndBishops & ~(queensAndBishops - 1);
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                possibilities = SlidingMoveUtilities.ValidDiagonalMoves(
                    this,
                    location,
                    OccupiedSquares
                );
                unsafeSpaces |= possibilities;
                queensAndBishops &= ~i;
                i = queensAndBishops & ~(queensAndBishops - 1);
            }

            // Rook Queen
            ulong queensAndRooks =
                color == Colors.White ? BlackRooks | BlackQueens : WhiteRooks | WhiteQueens;
            i = queensAndRooks & ~(queensAndRooks - 1);
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                possibilities = SlidingMoveUtilities.ValidHVMoves(this, location, OccupiedSquares);
                unsafeSpaces |= possibilities;
                queensAndRooks &= ~i;
                i = queensAndRooks & ~(queensAndRooks - 1);
            }

            // king
            ulong king = color == Colors.White ? BlackKing : WhiteKing;
            int kingLocation = king.NumberOfTrailingZeros();
            if (kingLocation > KingRangeBaseSquare)
            {
                possibilities = KingSpan << (kingLocation - KingRangeBaseSquare);
            }
            else
            {
                possibilities = KingSpan >> (KingRangeBaseSquare - kingLocation);
            }
            if (kingLocation % 8 >= 4)
            {
                possibilities &= ~(BitBoardConstants.FileG | BitBoardConstants.FileH);
            }
            else
            {
                possibilities &= ~(BitBoardConstants.FileA | BitBoardConstants.FileB);
            }
            unsafeSpaces |= possibilities;

            if (color == Colors.White)
            {
                _whiteUnsafe = unsafeSpaces;
            }

            if (color == Colors.Black)
            {
                _blackUnsafe = unsafeSpaces;
            }

            return unsafeSpaces;
        }

        private ulong? _threatenedSquares = null;

        public ulong GetThreatenedSquares(Colors color)
        {
            if (_threatenedSquares == null)
            {
                var enemyKings = color == Colors.White ? BlackKing : WhiteKing;
                var enemyQueens = color == Colors.White ? BlackQueens : WhiteQueens;

                var enemyPawns = color == Colors.White ? BlackPawns : WhitePawns;
                var enemyBishops = color == Colors.White ? BlackBishops : WhiteBishops;

                var enemyRooks = color == Colors.White ? BlackRooks : WhiteRooks;
                var enemyKnights = color == Colors.White ? BlackKnights : WhiteKnights;

                var knightSquares =
                    enemyKnights << 6
                    | enemyKnights >> 6
                    | enemyKnights << 10
                    | enemyKnights >> 10
                    | enemyKnights << 15
                    | enemyKnights >> 15
                    | enemyKnights << 17
                    | enemyKnights >> 17;

                var pawnSquares =
                    color == Colors.White
                        ? enemyPawns >> 9 | enemyPawns >> 7
                        : enemyPawns << 9 | enemyPawns << 7;

                var kingSquares =
                    enemyKings >> 1
                    | enemyKings << 1
                    | enemyKings >> 7
                    | enemyKings << 7
                    | enemyKings >> 8
                    | enemyKings << 8
                    | enemyKings >> 9
                    | enemyKings << 9;

                var slidingAttacks = (ulong)0;
                foreach (var rank in BitBoardConstants.RankMasks)
                {
                    if ((rank & enemyRooks) > 0 || (rank & enemyQueens) > 0)
                    {
                        slidingAttacks |= rank;
                    }
                }
                foreach (var file in BitBoardConstants.FileMasks)
                {
                    if ((file & enemyRooks) > 0 || (file & enemyQueens) > 0)
                    {
                        slidingAttacks |= file;
                    }
                }

                foreach (var diagonal in BitBoardConstants.DiagonalMasks)
                {
                    if ((diagonal & enemyBishops) > 0 || (diagonal & enemyQueens) > 0)
                    {
                        slidingAttacks |= diagonal;
                    }
                }

                foreach (var diagonal in BitBoardConstants.AntiDiagonalMasks)
                {
                    if ((diagonal & enemyBishops) > 0 || (diagonal & enemyQueens) > 0)
                    {
                        slidingAttacks |= diagonal;
                    }
                }
                _threatenedSquares = knightSquares | pawnSquares | kingSquares | slidingAttacks;
            }
            return _threatenedSquares.Value;
        }

        public void ClearPiece(Files f, int rank)
        {
            int position = GetPositionFromFileAndRank(f, rank);
            ClearPiece(position);
        }

        public void ClearPiece(int position)
        {
            for (int i = 0; i < _squares.Length; i++)
            {
                _squares[i] = null;
            }
            _threatenedSquares = null;
            _whiteUnsafe = null;
            _blackUnsafe = null;
            var mask = ~(U1 << position);
            WhitePawns &= mask;
            WhiteRooks &= mask;
            WhiteKnights &= mask;
            WhiteBishops &= mask;
            WhiteQueens &= mask;
            WhiteKing &= mask;

            BlackPawns &= mask;
            BlackRooks &= mask;
            BlackKnights &= mask;
            BlackBishops &= mask;
            BlackQueens &= mask;
            BlackKing &= mask;
        }

        public void SetPiece(Files f, int rank, PieceTypes type, Colors color)
        {
            int position = GetPositionFromFileAndRank(f, rank);
            SetPiece(position, type, color);
        }

        public int PawnsInFile(Files file, Colors color)
        {
            var pawns = color == Colors.White ? WhitePawns : BlackPawns;
            var fileMask = BitBoardConstants.FileMasks[(int)file - 1];
            return (pawns & fileMask).CountSetBits();
        }

        public void SetPiece(int position, PieceTypes type, Colors color)
        {
            ClearPiece(position); // Clear the piece first.
            switch (color)
            {
                case Colors.White:
                    switch (type)
                    {
                        case PieceTypes.Pawn:
                            WhitePawns |= (U1 << position);
                            return;
                        case PieceTypes.Rook:
                            WhiteRooks |= (U1 << position);
                            return;
                        case PieceTypes.Knight:
                            WhiteKnights |= (U1 << position);
                            return;
                        case PieceTypes.Bishop:
                            WhiteBishops |= (U1 << position);
                            return;
                        case PieceTypes.Queen:
                            WhiteQueens |= (U1 << position);
                            return;
                        case PieceTypes.King:
                            WhiteKing |= (U1 << position);
                            return;
                    }
                    break;
                case Colors.Black:
                    switch (type)
                    {
                        case PieceTypes.Pawn:
                            BlackPawns |= (U1 << position);
                            return;
                        case PieceTypes.Rook:
                            BlackRooks |= (U1 << position);
                            return;
                        case PieceTypes.Knight:
                            BlackKnights |= (U1 << position);
                            return;
                        case PieceTypes.Bishop:
                            BlackBishops |= (U1 << position);
                            return;
                        case PieceTypes.Queen:
                            BlackQueens |= (U1 << position);
                            return;
                        case PieceTypes.King:
                            BlackKing |= (U1 << position);
                            return;
                    }
                    break;
            }
        }
    }
}
