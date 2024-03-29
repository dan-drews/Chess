﻿using ChessLibrary.Boards;
using System;
using System.Collections.Generic;
using System.Linq;
using static ChessLibrary.BitBoardConstants;

namespace ChessLibrary.MoveGeneration
{
    public class MoveGenerator : IMoveGenerator
    {
        public Move[] GetAllLegalMoves(
            FullBitBoard b,
            Colors color,
            Files? enPassantFile,
            bool blackCanLongCastle,
            bool blackCanShortCastle,
            bool whiteCanLongCastle,
            bool whiteCanShortCastle,
            bool includeQuietMoves = true
        )
        {
            return AllValidMoves(
                b,
                color,
                enPassantFile,
                blackCanLongCastle,
                blackCanShortCastle,
                whiteCanLongCastle,
                whiteCanShortCastle,
                false,
                includeQuietMoves
            );
            throw new NotImplementedException();
        }

        public bool HasAnyLegalMoves(
            FullBitBoard b,
            Colors color,
            Files? enPassantFile,
            bool blackCanLongCastle,
            bool blackCanShortCastle,
            bool whiteCanLongCastle,
            bool whiteCanShortCastle
        )
        {
            var result = ValidPawnMoves(b, color, enPassantFile, true, false);
            if (result.Any())
            {
                return true;
            }
            ValidKnightMoves(b, color, true, result, false);
            if (result.Any())
            {
                return true;
            }
            ValidQueenMoves(b, color, true, result, false);
            if (result.Any())
            {
                return true;
            }
            ValidRookMoves(b, color, true, result, false);
            if (result.Any())
            {
                return true;
            }
            ValidBishopMoves(b, color, true, result, false);
            if (result.Any())
            {
                return true;
            }

            if (color == Colors.White)
            {
                ValidKingMoves(
                    b,
                    color,
                    whiteCanLongCastle,
                    whiteCanShortCastle,
                    true,
                    result,
                    false
                );
            }
            else
            {
                ValidKingMoves(
                    b,
                    color,
                    blackCanLongCastle,
                    blackCanShortCastle,
                    true,
                    result,
                    false
                );
            }
            return result.Count > 0;

        }

        public Move[]? GetAllLegalMoves(
            FullBitBoard b,
            SquareState squareState,
            Files? enPassantFile,
            bool blackCanLongCastle,
            bool blackCanShortCastle,
            bool whiteCanLongCastle,
            bool whiteCanShortCastle,
            bool ignoreCheck = false,
            bool includeQuietMoves = true
        )
        {
            var allValidMoves = GetAllLegalMoves(
                b,
                squareState.Piece.Color,
                enPassantFile,
                blackCanLongCastle,
                blackCanShortCastle,
                whiteCanLongCastle,
                whiteCanShortCastle,
                includeQuietMoves
            );
            return allValidMoves
                .Where(x => x.StartingSquare == squareState.Square.SquareNumber)
                .ToArray();
        }

        public bool IsKingInCheck(FullBitBoard b, Colors color)
        {
            return (b.Unsafe(color) & (color == Colors.White ? b.WhiteKing : b.BlackKing)) > 0;
        }

        public Move[] AllValidMoves(
            FullBitBoard b,
            Colors color,
            Files? enPassantFile,
            bool blackCanLongCastle,
            bool blackCanShortCastle,
            bool whiteCanLongCastle,
            bool whiteCanShortCastle,
            bool ignoreCheck = false,
            bool includeQuietMoves = true
        )
        {
            var result = ValidPawnMoves(b, color, enPassantFile, includeQuietMoves, false);
            ValidKnightMoves(b, color, includeQuietMoves, result, false);
            ValidRookMoves(b, color, includeQuietMoves, result, false);
            ValidBishopMoves(b, color, includeQuietMoves, result, false);
            ValidQueenMoves(b, color, includeQuietMoves, result, false);

            if (color == Colors.White)
            {
                ValidKingMoves(
                    b,
                    color,
                    whiteCanLongCastle,
                    whiteCanShortCastle,
                    includeQuietMoves,
                    result,
                    false
                );
            }
            else
            {
                ValidKingMoves(
                    b,
                    color,
                    blackCanLongCastle,
                    blackCanShortCastle,
                    includeQuietMoves,
                    result,
                    false
                );
            }
            return result.ToArray();
        }

        private static List<Move> ValidPawnMoves(
            FullBitBoard b,
            Colors color,
            Files? enPassantFile,
            bool includeQuietMoves,
            bool includeSelfChecks
        )
        {
            var result = new List<Move>(255); // 220 is approximate max moves per position. Pre-allocate that.
            var opposingColor = color == Colors.White ? Colors.Black : Colors.White;
            var opposingPieces = b.GetAllPieces(opposingColor);

            switch (color)
            {
                case Colors.White:
                    result = GetPawnMoves(
                        b,
                        Colors.White,
                        b.WhitePawns,
                        b.BlackPawns,
                        opposingPieces,
                        7,
                        9,
                        1,
                        BitBoardConstants.Rank4,
                        5,
                        enPassantFile,
                        (ulong pieces, int amount) => pieces << amount,
                        (ulong pieces, int amount) => pieces >> amount,
                        includeQuietMoves,
                        includeSelfChecks
                    );
                    break;
                case Colors.Black:
                    result = GetPawnMoves(
                        b,
                        Colors.Black,
                        b.BlackPawns,
                        b.WhitePawns,
                        opposingPieces,
                        9,
                        7,
                        -1,
                        BitBoardConstants.Rank5,
                        4,
                        enPassantFile,
                        (ulong pieces, int amount) => pieces >> amount,
                        (ulong pieces, int amount) => pieces << amount,
                        includeQuietMoves,
                        includeSelfChecks
                    );
                    break;
            }

            var promotionRank = color == Colors.White ? 8 : 1;
            var resultActual = new List<Move>();
            foreach (var m in result)
            {
                var targetBitBoard = 1UL << m.TargetSquare;

                if ((targetBitBoard & BitBoardConstants.RankMasks[promotionRank - 1]) > 0)
                {
                    var piece = m.Piece;
                    // Promotion. Don'd add this move, but add 1 for each promoted piece type

                    resultActual.Add(
                        new Move(
                            m.StartingSquare,
                            m.TargetSquare,
                            color,
                            m.Piece,
                            m.CapturedPiece,
                            Flag.PromoteToQueen
                        )
                    );
                    resultActual.Add(
                        new Move(
                            m.StartingSquare,
                            m.TargetSquare,
                            color,
                            m.Piece,
                            m.CapturedPiece,
                            Flag.PromoteToKnight
                        )
                    );
                    resultActual.Add(
                        new Move(
                            m.StartingSquare,
                            m.TargetSquare,
                            color,
                            m.Piece,
                            m.CapturedPiece,
                            Flag.PromoteToBishop
                        )
                    );
                    resultActual.Add(
                        new Move(
                            m.StartingSquare,
                            m.TargetSquare,
                            color,
                            m.Piece,
                            m.CapturedPiece,
                            Flag.PromoteToRook
                        )
                    );
                }
                else
                {
                    resultActual.Add(m);
                }
            }
            result = resultActual;

            return result;
        }

        private static List<Move> GetPawnMoves(
            FullBitBoard b,
            Colors color,
            ulong pawns,
            ulong opposingPawns,
            ulong opposingPieces,
            int leftShiftAmount,
            int rightShiftAmount,
            int pawnDirection,
            ulong doublePawnRank,
            int enPassantRank,
            Files? enPassantFile,
            Func<ulong, int, ulong> shiftOperation,
            Func<ulong, int, ulong> reverseShiftOperation,
            bool includeQuietMoves,
            bool includeSelfChecks
        )
        {
            var result = new List<Move>();
            ulong pawnMoves = shiftOperation(pawns, leftShiftAmount) & opposingPieces & ~FileA; // Capture Right
            var enumerator = pawnMoves.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int index = enumerator.Current;
                var destinationSquare = b.GetSquare(index);
                var startingSquare = b.GetSquare(
                    destinationSquare.Square.File - 1,
                    destinationSquare.Square.Rank - pawnDirection
                );
                var move = new Move(
                    startingSquare.Square.SquareNumber,
                    index,
                    color,
                    PieceTypes.Pawn,
                    destinationSquare.Piece?.Type
                );
                if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                {
                    result.Add(move);
                }
            }

            pawnMoves = shiftOperation(pawns, rightShiftAmount) & opposingPieces & ~FileH; // Capture Left
            enumerator = pawnMoves.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int index = enumerator.Current;
                var destinationSquare = b.GetSquare(index);
                var startingSquare = b.GetSquare(
                    destinationSquare.Square.File + 1,
                    destinationSquare.Square.Rank - pawnDirection
                );
                var move = new Move(
                    startingSquare.Square.SquareNumber,
                    index,
                    color,
                    PieceTypes.Pawn,
                    destinationSquare.Piece?.Type
                );

                if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                {
                    result.Add(move);
                }
            }

            if (includeQuietMoves)
            {
                pawnMoves = shiftOperation(pawns, 8) & b.EmptySquares; // Move Forward 1
                enumerator = pawnMoves.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    int index = enumerator.Current;
                    var destinationSquare = b.GetSquare(index);
                    var startingSquare = b.GetSquare(
                        destinationSquare.Square.File,
                        destinationSquare.Square.Rank - pawnDirection
                    );
                    var move = new Move(
                        startingSquare.Square.SquareNumber,
                        index,
                        color,
                        PieceTypes.Pawn
                    );

                    if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                    {
                        result.Add(move);
                    }
                }

                pawnMoves =
                    shiftOperation(pawns, 16)
                    & shiftOperation(b.EmptySquares, 8)
                    & b.EmptySquares
                    & doublePawnRank; // Move Forward 2

                enumerator = pawnMoves.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    int index = enumerator.Current;
                    var destinationSquare = b.GetSquare(index);
                    var startingSquare = b.GetSquare(
                        destinationSquare.Square.File,
                        destinationSquare.Square.Rank - 2 * pawnDirection
                    );
                    var move = new Move(
                        startingSquare.Square.SquareNumber,
                        index,
                        color,
                        PieceTypes.Pawn,
                        null,
                        Flag.PawnTwoForward
                    );
                    if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                    {
                        result.Add(move);
                    }
                }
            }

            if (enPassantFile != null)
            {
                var epSquare =
                    U1
                    << b.GetPositionFromFileAndRank(
                        enPassantFile.Value,
                        enPassantRank + pawnDirection
                    );
                var captureLeftOperation =
                    shiftOperation(pawns, rightShiftAmount) & epSquare & ~FileH; // Capture Left

                if (captureLeftOperation != 0)
                {
                    var destinationSquare = b.GetSquare(
                        captureLeftOperation.FirstBit()
                    ); // b.GetSquare(enPassantFile.Value, square.Square.Rank + pawnDirection);
                    var startingSquare = b.GetSquare(
                        reverseShiftOperation(captureLeftOperation, rightShiftAmount)
                            .NumberOfTrailingZeros()
                    );
                    var targetPiece = b.GetSquare(
                        reverseShiftOperation(captureLeftOperation, 8).NumberOfTrailingZeros()
                    ).Piece;
                    var move = new Move(
                        startingSquare.Square.SquareNumber,
                        destinationSquare.Square.SquareNumber,
                        color,
                        PieceTypes.Pawn,
                        targetPiece!.Type,
                        Flag.EnPassantCapture
                    );
                    if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                    {
                        result.Add(move);
                    }
                }

                var captureRightOperation =
                    shiftOperation(pawns, leftShiftAmount) & epSquare & ~FileA; // Capture Right
                if (captureRightOperation != 0)
                {
                    var destinationSquare = b.GetSquare(
                        captureRightOperation.FirstBit()
                    ); // b.GetSquare(enPassantFile.Value, square.Square.Rank + pawnDirection);
                    var startingSquare = b.GetSquare(
                        reverseShiftOperation(captureRightOperation, leftShiftAmount)
                            .NumberOfTrailingZeros()
                    );
                    var targetPiece = b.GetSquare(
                        reverseShiftOperation(captureRightOperation, 8).NumberOfTrailingZeros()
                    ).Piece;
                    var move = new Move(
                        startingSquare.Square.SquareNumber,
                        destinationSquare.Square.SquareNumber,
                        color,
                        PieceTypes.Pawn,
                        targetPiece!.Type,
                        Flag.EnPassantCapture
                    );
                    if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                    {
                        result.Add(move);
                    }
                }
            }

            return result;
        }

        private static void ValidKnightMoves(
            FullBitBoard b,
            Colors color,
            bool includeQuietMoves,
            List<Move> result,
            bool includeSelfChecks
        )
        {
            ulong currentKnights = color == Colors.Black ? b.BlackKnights : b.WhiteKnights;
            var enumerator = currentKnights.GetEnumerator();
            var notMyPieces = ~b.GetAllPieces(color);
            while (enumerator.MoveNext())
            {
                int location = enumerator.Current;
                ulong possibility = AttackTables.KnightAttacks[location] & notMyPieces;
                var possibleMoveEnumerator = possibility.GetEnumerator();
                while (possibleMoveEnumerator.MoveNext())
                {
                    int index = possibleMoveEnumerator.Current;
                    var destinationSquare = b.GetSquare(index);
                    var destinationPiece = destinationSquare.Piece;
                    if (destinationPiece != null || includeQuietMoves)
                    {
                        var move = new Move(
                            location,
                            index,
                            color,
                            PieceTypes.Knight,
                            destinationPiece?.Type
                        );

                        if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                        {
                            result.Add(move);
                        }
                    }
                }
            }
        }

        private static void ValidBishopMoves(
            FullBitBoard b,
            Colors color,
            bool includeQuietMoves,
            List<Move> result,
            bool includeSelfChecks
        )
        {
            var existingMoveCount = result.Count;
            MagicSlidingImplementation.ValidBishopMoves(b, color, includeQuietMoves, result);
            for (int i = result.Count - 1; i >= existingMoveCount; i--)
            {
                var move = result[i];
                if (!includeSelfChecks && ResultsInOwnCheck(b, move, color))
                {
                    result.RemoveAt(i);
                }
            }
        }

        private static void ValidRookMoves(
            FullBitBoard b,
            Colors color,
            bool includeQuietMoves,
            List<Move> result,
            bool includeSelfChecks
        )
        {
            var existingMoveCount = result.Count;
            MagicSlidingImplementation.ValidRookMoves(b, color, includeQuietMoves, result);
            for (int i = result.Count - 1; i >= existingMoveCount; i--)
            {
                var move = result[i];
                if (!includeSelfChecks && ResultsInOwnCheck(b, move, color))
                {
                    result.RemoveAt(i);
                }
            }
        }

        private static void ValidQueenMoves(
            FullBitBoard b,
            Colors color,
            bool includeQuietMoves,
            List<Move> result,
            bool includeSelfChecks
        )
        {
            var existingMoveCount = result.Count;
            MagicSlidingImplementation.ValidQueenMoves(b, color, includeQuietMoves, result);
            for (int i = result.Count - 1; i >= existingMoveCount; i--)
            {
                var move = result[i];
                if (!includeSelfChecks && ResultsInOwnCheck(b, move, color))
                {
                    result.RemoveAt(i);
                }
            }
        }

        private static void ValidKingMoves(
            FullBitBoard b,
            Colors color,
            bool canLongCastle,
            bool canShortCastle,
            bool includeQuietMoves,
            List<Move> result,
            bool includeSelfChecks
        )
        {
            ulong currentKing = color == Colors.Black ? b.BlackKing : b.WhiteKing;
            var notMyPieces = ~b.GetAllPieces(color);
            int location = currentKing.FirstBit();
            ulong possibility = AttackTables.KingAttacks[location] & notMyPieces;
            var startingSquare = b.GetSquare(location);
            var enumerator = possibility.GetEnumerator();
            while (enumerator.MoveNext())
            {
                int index = enumerator.Current;
                var destinationSquare = b.GetSquare(index);
                var destinationPiece = destinationSquare.Piece;
                if (destinationPiece != null || includeQuietMoves)
                {
                    var move = new Move(
                        location,
                        index,
                        color,
                        PieceTypes.King,
                        destinationPiece?.Type
                    );
                    if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                    {
                        result.Add(move);
                    }
                }
            }

            var startingKing = color == Colors.Black ? Starting_Black_King : Starting_White_King;
            var startingRooks = color == Colors.Black ? Starting_Black_Rooks : Starting_White_Rooks;
            var currentRooks = color == Colors.Black ? b.BlackRooks : b.WhiteRooks;
            if (startingKing == currentKing && (startingRooks & currentRooks) != 0)
            {
                if (canShortCastle || canLongCastle)
                {
                    var dangerous = b.Unsafe(color);
                    if ((dangerous & currentKing) != 0)
                    {
                        canShortCastle = false;
                        canLongCastle = false;
                    }

                    const ulong whiteLongCastleSquares = 0x78;
                    const ulong whiteLongCastleDangerousSquares = 0x38;

                    const ulong whiteShortCastleSquares = 0X0E;
                    const ulong whiteLongCastleStartSquare = 0x80;
                    const ulong whiteShortCastleStartSquares = 0X01;

                    const ulong blackLongCastleSquares = 0x7800000000000000;
                    const ulong blackLongCastleDangerousSquares = 0x3800000000000000;
                    const ulong blackShortCastleSquares = 0x0E00000000000000;

                    const ulong blackLongCastleStartSquare = 0x8000000000000000;
                    const ulong blackShortCastleStartSquares = 0x0100000000000000;

                    if (canShortCastle)
                    {
                        var castleSquares =
                            color == Colors.White
                                ? whiteShortCastleSquares
                                : blackShortCastleSquares;
                        var castleStartSquare =
                            color == Colors.White
                                ? whiteShortCastleStartSquares
                                : blackShortCastleStartSquares;
                        if (
                            (castleSquares & dangerous) == 0
                            && (castleSquares & b.OccupiedSquares & ~currentKing) == 0
                            && (castleStartSquare & currentRooks) != 0
                        )
                        {
                            var move = new Move(
                                location,
                                b.GetPositionFromFileAndRank(Files.G, startingSquare.Square.Rank),
                                color,
                                PieceTypes.King,
                                null,
                                Flag.ShortCastle
                            );
                            if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                            {
                                result.Add(move);
                            }
                        }
                    }

                    if (canLongCastle)
                    {
                        var castleSquares =
                            color == Colors.White ? whiteLongCastleSquares : blackLongCastleSquares;
                        var castleStartSquare =
                            color == Colors.White
                                ? whiteLongCastleStartSquare
                                : blackLongCastleStartSquare;
                        var squaresThatCannotBeDangerous =
                            color == Colors.White
                                ? whiteLongCastleDangerousSquares
                                : blackLongCastleDangerousSquares;
                        if (
                            (squaresThatCannotBeDangerous & dangerous) == 0
                            && (castleSquares & b.OccupiedSquares & ~currentKing) == 0
                            && (castleStartSquare & currentRooks) != 0
                        )
                        {
                            var move = new Move(
                                location,
                                b.GetPositionFromFileAndRank(Files.C, startingSquare.Square.Rank),
                                color,
                                PieceTypes.King,
                                null,
                                Flag.LongCastle
                            );
                            if (includeSelfChecks || !ResultsInOwnCheck(b, move, color))
                            {
                                result.Add(move);
                            }
                        }
                    }
                }
            }
        }

        public static bool ResultsInOwnCheck(FullBitBoard b, Move move, Colors color)
        {
            var king = color == Colors.White ? b.WhiteKing : b.BlackKing;
            ulong kingBoard;
            if (move.Piece == PieceTypes.King)
            {
                kingBoard = U1 << move.TargetSquare;
            }
            else
            {
                kingBoard = king;
            }
            if ((b.GetThreatenedSquares(color) & kingBoard) > 0)
            {
                var clonedBoard = (FullBitBoard)b.Clone();
                clonedBoard.MovePiece(move);
                return (
                        clonedBoard.Unsafe(color)
                        & (color == Colors.White ? clonedBoard.WhiteKing : clonedBoard.BlackKing)
                    ) > 0;
            }
            return false;
        }
    }
}
