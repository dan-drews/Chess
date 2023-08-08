using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
[assembly: InternalsVisibleTo("Chess.Tests")]
namespace ChessLibrary
{
    public class BitBoard : IBoard
    {

        static ulong[] FileMasks = new ulong[]
        {
            0x8080808080808080, 0x4040404040404040, 0x2020202020202020, 0x1010101010101010,
            0x0808080808080808, 0x0404040404040404, 0x0202020202020202, 0x0101010101010101
        };

        static ulong[] RankMasks = new ulong[]
        {
            0x00000000000000FF, 0x000000000000FF00, 0x0000000000FF0000, 0x00000000FF000000,
            0x000000FF00000000, 0x0000FF0000000000, 0x00FF000000000000, 0xFF00000000000000
        };

        static ulong[] DiagonalMasks = new ulong[]
        {
            // A8                        // A7, B8                          // A6, B7, C8                       // A5, B6, C7, D8               // A4, B5, C6, D7, E8
            0x8000000000000000L,         0x4080000000000000L,               0x2040800000000000L,                0x1020408000000000L,            0x810204080000000L, 
            // A3, B4, C5, D6, E7, F8    // A2, B3, C4, D5, E6, F7, G8      // A1, B2, C3, D4, E5, F6, G7, H8   // B1, C2, D3, E4, F5, G6, H7   // C1, D2, E3, F4, G5, H6
            0x408102040800000L,          0x204081020408000L,                0x102040810204080L,                 0x1020408102040L,               0x10204081020L,
            // D1, E2, F3, G4, H5        // E1, F2, G3, H4                  // F1, G2, H3                       // G1, H2                       // H1
            0x102040810L,                0x1020408L,                        0x10204L,                           0x102L,                         0x1L
        };

        static ulong[] AntiDiagonalMasks = new ulong[]
        {
            // H8                        // G8, H7                          // F8, G7, H6                       // E8, F7, G6, H5               // D8, E7, F6, G5, H4
            0x100000000000000L,          0x201000000000000L,                0x402010000000000L,                 0x804020100000000L,             0x1008040201000000L,
            // C8, D7, E6, F5, G4, H3    // B8, C7, D6, E5, F4, G3, H2      // A8, B7, C6, D5, E4, F3, G2, H1   // A7, B6, C5, D4, E3, F2, G1   // A6, B5, C4, D3, E2, F1
            0x2010080402010000L,         0x4020100804020100L,               0x8040201008040201L,                0x80402010080402L,              0x804020100804L,
            // A5, B4, C3, D2, E1        // A4, B3, C2, D1                  // A3, B2, C1                       // A2, B1                       // A1 
            0x8040201008L,               0x80402010L,                       0x804020L,                          0x8040L,                        0x80L
        };

        private const ulong Rank8 = 0xFF00000000000000;
        private const ulong Rank1 = 0xFF;
        private const ulong Rank4 = 0x00000000FF000000;
        private const ulong Rank5 = 0x000000FF00000000;

        private const ulong FileA = 0x8080808080808080;
        private const ulong FileB = 0x4040404040404040;
        private const ulong FileH = 0x0101010101010101;
        private const ulong FileG = 0x0202020202020202;

        private const int KnightRangeBaseSquare = 18;
        private const ulong KnightSpan = 0x0000000A1100110A;

        private const int KingRangeBaseSquare = 9;
        private const ulong KingSpan = 0x0000000000070507;

        private const ulong U1 = (ulong)1;
        private const ulong Starting_White_Pawns = 0xFF00;
        private const ulong Starting_White_Rooks = 0x81;
        private const ulong Starting_White_Knights = 0x42;
        private const ulong Starting_White_Bishops = 0x24;
        private const ulong Starting_White_Queens = 0x10;
        private const ulong Starting_White_King = 0x8;

        private const ulong Starting_Black_Pawns = 0xFF000000000000;
        private const ulong Starting_Black_Rooks = 0x8100000000000000;
        private const ulong Starting_Black_Knights = 0x4200000000000000;
        private const ulong Starting_Black_Bishops = 0x2400000000000000;
        private const ulong Starting_Black_Queens = 0x1000000000000000;
        private const ulong Starting_Black_King = 0x800000000000000;

        private ulong _whitePawns;
        private ulong _whiteRooks;
        private ulong _whiteKnights;
        private ulong _whiteBishops;
        private ulong _whiteQueens;
        private ulong _whiteKing;

        private ulong _blackPawns;
        private ulong _blackRooks;
        private ulong _blackKnights;
        private ulong _blackBishops;
        private ulong _blackQueens;
        private ulong _blackKing;

        public BitBoard()
        {

        }

        public void SetupBoard()
        {
            _whitePawns = Starting_White_Pawns;
            _whiteRooks = Starting_White_Rooks;
            _whiteKnights = Starting_White_Knights;
            _whiteBishops = Starting_White_Bishops;
            _whiteQueens = Starting_White_Queens;
            _whiteKing = Starting_White_King;

            _blackPawns = Starting_Black_Pawns;
            _blackRooks = Starting_Black_Rooks;
            _blackKnights = Starting_Black_Knights;
            _blackBishops = Starting_Black_Bishops;
            _blackQueens = Starting_Black_Queens;
            _blackKing = Starting_Black_King;
        }

        public object Clone()
        {
            var bb = new BitBoard();
            bb._whitePawns = _whitePawns;
            bb._whiteRooks = _whiteRooks;
            bb._whiteKnights = _whiteKnights;
            bb._whiteBishops = _whiteBishops;
            bb._whiteQueens = _whiteQueens;
            bb._whiteKing = _whiteKing;

            bb._blackPawns = _blackPawns;
            bb._blackRooks = _blackRooks;
            bb._blackKnights = _blackKnights;
            bb._blackBishops = _blackBishops;
            bb._blackQueens = _blackQueens;
            bb._blackKing = _blackKing;
            
            bb._emptySquareStates = _emptySquareStates;
            bb._squares = _squares;
            bb._emptySquares = _emptySquares;
            return bb;
        }

        private ulong OccupiedSquares
        {
            get
            {
                return _whitePawns | _blackPawns | _whiteRooks | _blackRooks | _whiteKnights | _blackKnights | _whiteBishops | _blackBishops | _whiteQueens | _blackQueens | _whiteKing | _blackKing;
            }
        }

        private ulong EmptySquares
        {
            get
            {
                return ~OccupiedSquares;
            }
        }

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

        private SquareState[]? _emptySquareStates = null;
        private Square[]? _emptySquares = null;
        private SquareState GetSquareInternal(int position)
        {
            ulong squareMask = (U1 << position);
            ulong occupiedSquares = OccupiedSquares;
            if (_emptySquareStates == null || _emptySquares == null)
            {
                _emptySquareStates = new SquareState[64];
                _emptySquares = new Square[64];
                for (int i = 0; i < 64; i++)
                {
                    _emptySquares[i] = new Square(i);
                    _emptySquareStates[i] = new SquareState(_emptySquares[i]);
                }
            }

            if ((occupiedSquares & squareMask) == 0)
            {
                return _emptySquareStates[position];
            }
            
            // Casting from enum to int is taking a lot of CPU power
            // So we are avoiding using enum values here

            if ((_whitePawns & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][0]; // White Pawn

            if ((_blackPawns & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][0]; // Black Pawn

            if ((_whiteRooks & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][1]; // White Rook

            if ((_blackRooks & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][1]; // Black Rook

            if ((_whiteKnights & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][2]; // White Knight

            if ((_blackKnights & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][2]; // Black Knight

            if ((_whiteBishops & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][3]; // White Bishop

            if ((_blackBishops & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][3]; // Black Bishop

            if ((_whiteQueens & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][4]; // White Queen

            if ((_blackQueens & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][4]; // Black Queen

            if ((_whiteKing & squareMask) != 0)
                return SquareState.SquareStateMap[position][0][5]; // White King

            if ((_blackKing & squareMask) != 0)
                return SquareState.SquareStateMap[position][1][5]; // Black King

            throw new Exception("Not sure what piece is here.");
        }

        public void MovePiece(Move move)
        {
            // En Passant
            if (move.Flags == Move.Flag.EnPassantCapture) // move.Piece == PieceTypes.Pawn && move.CapturedPiece != null && GetSquare(move.DestinationSquare.File, move.DestinationSquare.Rank).Piece == null)
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
            if(move.Flags == Move.Flag.ShortCastle)
            {
                var targetSquare = new Square(move.TargetSquare);
                ClearPiece(Files.H, targetSquare.Rank);                
                SetPiece(Files.F, targetSquare.Rank, PieceTypes.Rook, move.Color);
            }
            if(move.Flags == Move.Flag.LongCastle)
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

        public bool ResultsInOwnCheck(Move move, Colors color)
        {
            var king = color == Colors.White ? _whiteKing : _blackKing;
            ulong kingBoard;
            if (move.Piece == PieceTypes.King)
            {
                kingBoard = U1 << move.TargetSquare;
            }
            else
            {
                kingBoard = king;
            }
            if ((GetThreatenedSquares(color) & kingBoard)
                > 0)
            {
                var clonedBoard = (BitBoard)Clone();
                // ToDo: Apply Move
                clonedBoard.MovePiece(move);
                return (clonedBoard.Unsafe(color) & (color == Colors.White ? clonedBoard._whiteKing : clonedBoard._blackKing)) > 0;

            }
            return false;
        }

        private ulong GetAllPieces(Colors color)
        {
            switch (color)
            {
                case Colors.Black:
                    return _blackPawns | _blackRooks | _blackKnights | _blackBishops | _blackQueens | _blackKing;
                case Colors.White:
                    return _whitePawns | _whiteRooks | _whiteKnights | _whiteBishops | _whiteQueens | _whiteKing;
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
                unsafeSpaces = (_blackPawns >> 7) & ~FileH; // Pawn Capture Left
                unsafeSpaces |= (_blackPawns >> 9) & ~FileA; // Pawn Capture Right
            }
            else
            {
                unsafeSpaces = (_whitePawns << 7) & ~FileA; // Pawn Capture Left
                unsafeSpaces |= (_whitePawns << 9) & ~FileH; // Pawn Capture Right
            }

            ulong possibilities;
            // knight
            ulong knight = color == Colors.White ? _blackKnights : _whiteKnights;
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
                    possibilities &= ~(FileG | FileH);
                }
                else
                {
                    possibilities &= ~(FileA | FileB);
                }
                unsafeSpaces |= possibilities;
                knight &= ~i;
                i = knight & ~(knight - 1);
            }

            // Bishop/queen
            ulong queensAndBishops = color == Colors.White ? _blackBishops | _blackQueens : _whiteBishops | _whiteQueens;
            i = queensAndBishops & ~(queensAndBishops - 1);
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                possibilities = ValidDiagonalMoves(location, OccupiedSquares);
                unsafeSpaces |= possibilities;
                queensAndBishops &= ~i;
                i = queensAndBishops & ~(queensAndBishops - 1);
            }

            // Rook Queen
            ulong queensAndRooks = color == Colors.White ? _blackRooks | _blackQueens : _whiteRooks | _whiteQueens;
            i = queensAndRooks & ~(queensAndRooks - 1);
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                possibilities = ValidHVMoves(location, OccupiedSquares);
                unsafeSpaces |= possibilities;
                queensAndRooks &= ~i;
                i = queensAndRooks & ~(queensAndRooks - 1);
            }

            // king
            ulong king = color == Colors.White ? _blackKing : _whiteKing;
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
                possibilities &= ~(FileG | FileH);
            }
            else
            {
                possibilities &= ~(FileA | FileB);
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

        public List<Move> AllValidMoves(Colors color, Files? enPassantFile, bool blackCanLongCastle, bool blackCanShortCastle, bool whiteCanLongCastle, bool whiteCanShortCastle, bool ignoreCheck = false, bool includeQuietMoves = true)
        {
            var result = ValidPawnMoves(color, enPassantFile, includeQuietMoves);
            result.AddRange(ValidKnightMoves(color, includeQuietMoves));
            result.AddRange(ValidRookMoves(color, includeQuietMoves));
            result.AddRange(ValidBishopMoves(color, includeQuietMoves));
            result.AddRange(ValidQueenMoves(color, includeQuietMoves));

            if (color == Colors.White)
            {
                result.AddRange(ValidKingMoves(color, whiteCanLongCastle, whiteCanShortCastle, includeQuietMoves));
            }
            else
            {
                result.AddRange(ValidKingMoves(color, blackCanLongCastle, blackCanShortCastle, includeQuietMoves));
            }

            return result.Where(x => !ResultsInOwnCheck(x, color)).ToList();
        }

        private ulong? _threatenedSquares = null;
        private ulong GetThreatenedSquares(Colors color)
        {
            if (_threatenedSquares == null)
            {
                var enemyKings = color == Colors.White ? _blackKing : _whiteKing;
                var enemyQueens = color == Colors.White ? _blackQueens : _whiteQueens;

                var enemyPawns = color == Colors.White ? _blackPawns : _whitePawns;
                var enemyBishops = color == Colors.White ? _blackBishops : _whiteBishops;

                var enemyRooks = color == Colors.White ? _blackRooks : _whiteRooks;
                var enemyKnights = color == Colors.White ? _blackKnights : _whiteKnights;

                var knightSquares = enemyKnights << 6 | enemyKnights >> 6 |
                                    enemyKnights << 10 | enemyKnights >> 10 |
                                    enemyKnights << 15 | enemyKnights >> 15 |
                                    enemyKnights << 17 | enemyKnights >> 17;

                var pawnSquares = color == Colors.White ? enemyPawns >> 9 | enemyPawns >> 7
                                                        : enemyPawns << 9 | enemyPawns << 7;

                var kingSquares = enemyKings >> 1 | enemyKings << 1 |
                                  enemyKings >> 7 | enemyKings << 7 |
                                  enemyKings >> 8 | enemyKings << 8 |
                                  enemyKings >> 9 | enemyKings << 9;

                var slidingAttacks = (ulong)0;
                foreach (var rank in RankMasks)
                {
                    if ((rank & enemyRooks) > 0 || (rank & enemyQueens) > 0)
                    {
                        slidingAttacks |= rank;
                    }
                }
                foreach (var file in FileMasks)
                {
                    if ((file & enemyRooks) > 0 || (file & enemyQueens) > 0)
                    {
                        slidingAttacks |= file;
                    }
                }

                foreach (var diagonal in DiagonalMasks)
                {
                    if ((diagonal & enemyBishops) > 0 || (diagonal & enemyQueens) > 0)
                    {
                        slidingAttacks |= diagonal;
                    }
                }

                foreach (var diagonal in AntiDiagonalMasks)
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

        public bool IsKingInCheck(Colors color)
        {
            return (Unsafe(color) & (color == Colors.White ? _whiteKing : _blackKing)) > 0;
        }

        private ulong ValidHVMoves(int index, ulong occupied)
        {
            var square = GetSquare(index);
            ulong binaryS = U1 << index;
            ulong fileMask = FileMasks[(int)square.Square.File - 1];
            ulong rankMask = RankMasks[square.Square.Rank - 1];
            ulong possibilitiesHorizontal = ((occupied & rankMask) - (2 * binaryS)) ^ ((occupied & rankMask).ReverseBits() - 2 * binaryS.ReverseBits()).ReverseBits();
            ulong possibilitiesVertical = ((occupied & fileMask) - (2 * binaryS)) ^ Extensions.ReverseBits(Extensions.ReverseBits(occupied & fileMask) - 2 * Extensions.ReverseBits(binaryS)); // ((occupied & fileMask).ReverseBits() - 2 * binaryS.ReverseBits()).ReverseBits();
            return (possibilitiesHorizontal & rankMask) | (possibilitiesVertical & fileMask);
        }

        private ulong ValidDiagonalMoves(int index, ulong occupied)
        {
            var square = GetSquare(index);
            ulong binaryS = U1 << index;

            ulong diagonalMask = DiagonalMasks[((int)square.Square.File - 1) + (8 - (int)square.Square.Rank)];
            ulong antidiagonalMask = AntiDiagonalMasks[14 - (square.Square.Rank - 1 + (int)square.Square.File - 1)];

            ulong possibilitiesDiagonal = ((occupied & diagonalMask) - (2 * binaryS)) ^ ((occupied & diagonalMask).ReverseBits() - (2 * binaryS.ReverseBits())).ReverseBits();
            ulong possibilitiesAntidiagonal = ((occupied & antidiagonalMask) - (2 * binaryS)) ^ Extensions.ReverseBits(Extensions.ReverseBits(occupied & antidiagonalMask) - 2 * Extensions.ReverseBits(binaryS));


            return (possibilitiesDiagonal & diagonalMask) | (possibilitiesAntidiagonal & antidiagonalMask);
        }

        private List<Move> ValidBishopMoves(Colors color, bool includeQuietMoves)
        {
            var result = new List<Move>();
            var occupied = OccupiedSquares;
            var currentSquares = color == Colors.White ? _whiteBishops : _blackBishops;
            var notMyPieces = ~GetAllPieces(color);

            ulong i = currentSquares & ~(currentSquares - 1);
            ulong possibility;
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                possibility = ValidDiagonalMoves(location, occupied) & notMyPieces;
                ulong j = possibility & ~(possibility - 1);
                while (j != 0)
                {
                    var index = possibility.NumberOfTrailingZeros();
                    var destinationSquare = GetSquare(index);
                    if (destinationSquare.Piece != null || includeQuietMoves)
                    {
                        result.Add(new Move(location, index, color, PieceTypes.Bishop, destinationSquare.Piece?.Type));
                    }
                    possibility &= ~j;
                    j = possibility & ~(possibility - 1);
                }
                currentSquares &= ~i;
                i = currentSquares & ~(currentSquares - 1);
            }

            return result;
        }

        private List<Move> ValidRookMoves(Colors color, bool includeQuietMoves)
        {
            var result = new List<Move>();
            var occupied = OccupiedSquares;
            var currentSquares = color == Colors.White ? _whiteRooks : _blackRooks;
            var notMyPieces = ~GetAllPieces(color);

            ulong i = currentSquares & ~(currentSquares - 1);
            ulong possibility;
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                possibility = ValidHVMoves(location, occupied) & notMyPieces;
                ulong j = possibility & ~(possibility - 1);
                while (j != 0)
                {
                    var index = possibility.NumberOfTrailingZeros();
                    var destinationSquare = GetSquare(index);
                    if (destinationSquare.Piece != null || includeQuietMoves)
                    {
                        result.Add(new Move(location, index, color, PieceTypes.Rook, destinationSquare?.Piece?.Type));
                    }
                    possibility &= ~j;
                    j = possibility & ~(possibility - 1);
                }
                currentSquares &= ~i;
                i = currentSquares & ~(currentSquares - 1);
            }

            return result;
        }

        private List<Move> ValidQueenMoves(Colors color, bool includeQuietMoves)
        {
            var result = new List<Move>();
            var occupied = OccupiedSquares;
            var currentQueen = color == Colors.White ? _whiteQueens : _blackQueens;
            var notMyPieces = ~GetAllPieces(color);
            ulong i = currentQueen & ~(currentQueen - 1);
            ulong possibility;
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                possibility = (ValidHVMoves(location, occupied) | ValidDiagonalMoves(location, occupied)) & notMyPieces;
                ulong j = possibility & ~(possibility - 1);
                while (j != 0)
                {
                    int index = j.NumberOfTrailingZeros();
                    var destinationSquare = GetSquare(index);
                    if (destinationSquare.Piece != null || includeQuietMoves)
                    {
                        result.Add(new Move(location, index, color, PieceTypes.Queen, destinationSquare?.Piece?.Type));
                    }
                    possibility &= ~j;
                    j = possibility & ~(possibility - 1);
                }
                currentQueen &= ~i;
                i = currentQueen & ~(currentQueen - 1);
            }
            return result;
        }

        private List<Move> ValidKingMoves(Colors color, bool canLongCastle, bool canShortCastle, bool includeQuietMoves)
        {
            var result = new List<Move>();
            ulong currentKing = color == Colors.Black ? _blackKing : _whiteKing;
            var notMyPieces = ~GetAllPieces(color);
            ulong possibility;
            int location = currentKing.NumberOfTrailingZeros();

            if (location > KingRangeBaseSquare)
            {
                possibility = KingSpan << (location - KingRangeBaseSquare);
            }
            else
            {
                possibility = KingSpan >> (KingRangeBaseSquare - location);
            }
            if (location % 8 >= 4)
            {
                possibility &= ~(FileG | FileH) & notMyPieces;
            }
            else
            {
                possibility &= ~(FileA | FileB) & notMyPieces;
            }
            var startingSquare = GetSquare(location);
            ulong j = possibility & ~(possibility - 1);
            while (j != 0)
            {
                int index = j.NumberOfTrailingZeros();
                var destinationSquare = GetSquare(index);
                if (destinationSquare.Piece != null || includeQuietMoves)
                {
                    result.Add(new Move(location, index, color, PieceTypes.King, destinationSquare.Piece?.Type));
                }
                possibility &= ~j;
                j = possibility & ~(possibility - 1);
            }

            var startingKing = color == Colors.Black ? Starting_Black_King : Starting_White_King;
            var startingRooks = color == Colors.Black ? Starting_Black_Rooks : Starting_White_Rooks;
            var currentRooks = color == Colors.Black ? _blackRooks : _whiteRooks;
            if (startingKing == currentKing && (startingRooks & currentRooks) != 0)
            {
                if (canShortCastle || canLongCastle)
                {
                    var dangerous = Unsafe(color);
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
                        var castleSquares = color == Colors.White ? whiteShortCastleSquares : blackShortCastleSquares;
                        var castleStartSquare = color == Colors.White ? whiteShortCastleStartSquares : blackShortCastleStartSquares;
                        if ((castleSquares & dangerous) == 0 && (castleSquares & OccupiedSquares & ~currentKing) == 0 && (castleStartSquare & currentRooks) != 0)
                        {
                            result.Add(new Move(location, GetPositionFromFileAndRank(Files.G, startingSquare.Square.Rank), color, PieceTypes.King, null, Move.Flag.ShortCastle));
                        }
                    }

                    if (canLongCastle)
                    {
                        var castleSquares = color == Colors.White ? whiteLongCastleSquares : blackLongCastleSquares;
                        var castleStartSquare = color == Colors.White ? whiteLongCastleStartSquare : blackLongCastleStartSquare;
                        var squaresThatCannotBeDangerous = color == Colors.White ? whiteLongCastleDangerousSquares : blackLongCastleDangerousSquares;
                        if ((squaresThatCannotBeDangerous & dangerous) == 0 && (castleSquares & OccupiedSquares & ~currentKing) == 0 && (castleStartSquare & currentRooks) != 0)
                        {
                            result.Add(new Move(location, GetPositionFromFileAndRank(Files.C, startingSquare.Square.Rank), color, PieceTypes.King, null, Move.Flag.LongCastle));
                        }
                    }

                }

            }

            return result;
        }

        private List<Move> ValidKnightMoves(Colors color, bool includeQuietMoves)
        {
            var result = new List<Move>();
            ulong currentKnights = color == Colors.Black ? _blackKnights : _whiteKnights;
            ulong i = currentKnights & ~(currentKnights - 1);
            var notMyPieces = ~GetAllPieces(color);
            ulong possibility;
            while (i != 0)
            {
                int location = i.NumberOfTrailingZeros();
                if (location > KnightRangeBaseSquare)
                {
                    possibility = KnightSpan << (location - KnightRangeBaseSquare);
                }
                else
                {
                    possibility = KnightSpan >> (KnightRangeBaseSquare - location);
                }
                if (location % 8 >= 4)
                {
                    possibility &= ~(FileG | FileH) & notMyPieces;
                }
                else
                {
                    possibility &= ~(FileA | FileB) & notMyPieces;
                }
                ulong j = possibility & ~(possibility - 1);
                while (j != 0)
                {
                    int index = j.NumberOfTrailingZeros();
                    var destinationSquare = GetSquare(index);
                    if (destinationSquare.Piece != null || includeQuietMoves)
                    {
                        result.Add(new Move(location, index, color, PieceTypes.Knight, destinationSquare.Piece?.Type));
                    }
                    possibility &= ~j;
                    j = possibility & ~(possibility - 1);
                }

                currentKnights &= ~i;
                i = currentKnights & ~(currentKnights - 1);
            }
            return result;
        }

        private List<Move> ValidPawnMoves(Colors color, Files? enPassantFile, bool includeQuietMoves)
        {
            var result = new List<Move>();
            var opposingColor = color == Colors.White ? Colors.Black : Colors.White;
            var opposingPieces = GetAllPieces(opposingColor);


            switch (color)
            {
                case Colors.White:
                    result = GetPawnMoves(Colors.White, _whitePawns, _blackPawns, opposingPieces, 7, 9, 1, Rank4, 5, enPassantFile, (ulong pieces, int amount) => pieces << amount, (ulong pieces, int amount) => pieces >> amount, includeQuietMoves);
                    break;
                case Colors.Black:
                    result = GetPawnMoves(Colors.Black, _blackPawns, _whitePawns, opposingPieces, 9, 7, -1, Rank5, 4, enPassantFile, (ulong pieces, int amount) => pieces >> amount, (ulong pieces, int amount) => pieces << amount, includeQuietMoves);
                    break;
            }

            var promotionRank = color == Colors.White ? 8 : 1;
            var resultActual = new List<Move>();
            foreach (var m in result)
            {

                var targetBitBoard = 1UL << m.TargetSquare;

                if ((targetBitBoard & RankMasks[promotionRank - 1]) > 0)
                {
                    var piece = m.Piece;
                    // Promotion. Don'd add this move, but add 1 for each promoted piece type

                    resultActual.Add(new Move(m.StartingSquare, m.TargetSquare, color, m.Piece, m.CapturedPiece, Move.Flag.PromoteToQueen));
                    resultActual.Add(new Move(m.StartingSquare, m.TargetSquare, color, m.Piece, m.CapturedPiece, Move.Flag.PromoteToKnight));
                    resultActual.Add(new Move(m.StartingSquare, m.TargetSquare, color, m.Piece, m.CapturedPiece, Move.Flag.PromoteToBishop));
                    resultActual.Add(new Move(m.StartingSquare, m.TargetSquare, color, m.Piece, m.CapturedPiece, Move.Flag.PromoteToRook));

                }
                else
                {
                    resultActual.Add(m);
                }
            }
            result = resultActual;

            return result;
        }

        private List<Move> GetPawnMoves(Colors color, ulong pawns, ulong opposingPawns, ulong opposingPieces, int leftShiftAmount, int rightShiftAmount, int pawnDirection, ulong doublePawnRank, int enPassantRank, Files? enPassantFile, Func<ulong, int, ulong> shiftOperation, Func<ulong, int, ulong> reverseShiftOperation, bool includeQuietMoves)
        {
            var result = new List<Move>();
            ulong pawnMoves = shiftOperation(pawns, leftShiftAmount) & opposingPieces & ~FileA; // Capture Right
            ulong possibility = pawnMoves & ~(pawnMoves - 1);
            while (possibility != 0)
            {
                int index = possibility.NumberOfTrailingZeros();
                var destinationSquare = GetSquare(index);
                var startingSquare = GetSquare(destinationSquare.Square.File - 1, destinationSquare.Square.Rank - pawnDirection);
                result.Add(new Move(startingSquare.Square.SquareNumber, index, color, PieceTypes.Pawn, destinationSquare.Piece?.Type));
                pawnMoves &= ~possibility;
                possibility = pawnMoves & ~(pawnMoves - 1);
            }

            pawnMoves = shiftOperation(pawns, rightShiftAmount) & opposingPieces & ~FileH; // Capture Left
            possibility = pawnMoves & ~(pawnMoves - 1);
            while (possibility != 0)
            {
                int index = possibility.NumberOfTrailingZeros();
                var destinationSquare = GetSquare(index);
                var startingSquare = GetSquare(destinationSquare.Square.File + 1, destinationSquare.Square.Rank - pawnDirection);
                result.Add(new Move(startingSquare.Square.SquareNumber, index, color, PieceTypes.Pawn, destinationSquare.Piece?.Type));
                pawnMoves &= ~possibility;
                possibility = pawnMoves & ~(pawnMoves - 1);
            }

            if (includeQuietMoves)
            {
                pawnMoves = shiftOperation(pawns, 8) & EmptySquares; // Move Forward 1
                possibility = pawnMoves & ~(pawnMoves - 1);
                while (possibility != 0)
                {
                    int index = possibility.NumberOfTrailingZeros();
                    var destinationSquare = GetSquare(index);
                    var startingSquare = GetSquare(destinationSquare.Square.File, destinationSquare.Square.Rank - pawnDirection);
                    result.Add(new Move(startingSquare.Square.SquareNumber, index, color, PieceTypes.Pawn));
                    pawnMoves &= ~possibility;
                    possibility = pawnMoves & ~(pawnMoves - 1);
                }

                pawnMoves = shiftOperation(pawns, 16) & shiftOperation(EmptySquares, 8) & EmptySquares & doublePawnRank; // Move Forward 2
                possibility = pawnMoves & ~(pawnMoves - 1);
                while (possibility != 0)
                {
                    int index = possibility.NumberOfTrailingZeros();
                    var destinationSquare = GetSquare(index);
                    var startingSquare = GetSquare(destinationSquare.Square.File, destinationSquare.Square.Rank - 2 * pawnDirection);
                    result.Add(new Move(startingSquare.Square.SquareNumber, index, color, PieceTypes.Pawn, null, Move.Flag.PawnTwoForward));
                    pawnMoves &= ~possibility;
                    possibility = pawnMoves & ~(pawnMoves - 1);
                }
            }


            if (enPassantFile != null)
            {

                var epSquare = U1 << GetPositionFromFileAndRank(enPassantFile.Value, enPassantRank + pawnDirection);
                var captureLeftOperation = shiftOperation(pawns, rightShiftAmount) & epSquare & ~FileH; // Capture Left

                if (captureLeftOperation != 0)
                {
                    var destinationSquare = GetSquare(captureLeftOperation.NumberOfTrailingZeros());// GetSquare(enPassantFile.Value, square.Square.Rank + pawnDirection);
                    var startingSquare = GetSquare(reverseShiftOperation(captureLeftOperation, rightShiftAmount).NumberOfTrailingZeros());
                    var targetPiece = GetSquare(reverseShiftOperation(captureLeftOperation, 8).NumberOfTrailingZeros()).Piece;
                    result.Add(new Move(startingSquare.Square.SquareNumber, destinationSquare.Square.SquareNumber, color, PieceTypes.Pawn, targetPiece!.Type, Move.Flag.EnPassantCapture));
                }

                var captureRightOperation = shiftOperation(pawns, leftShiftAmount) & epSquare & ~FileA; // Capture Right
                if (captureRightOperation != 0)
                {
                    var destinationSquare = GetSquare(captureRightOperation.NumberOfTrailingZeros());// GetSquare(enPassantFile.Value, square.Square.Rank + pawnDirection);
                    var startingSquare = GetSquare(reverseShiftOperation(captureRightOperation, leftShiftAmount).NumberOfTrailingZeros());
                    var targetPiece = GetSquare(reverseShiftOperation(captureRightOperation, 8).NumberOfTrailingZeros()).Piece;
                    result.Add(new Move(startingSquare.Square.SquareNumber, destinationSquare.Square.SquareNumber, color, PieceTypes.Pawn, targetPiece!.Type, Move.Flag.EnPassantCapture));
                }

            }

            return result;
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
            _whitePawns &= mask;
            _whiteRooks &= mask;
            _whiteKnights &= mask;
            _whiteBishops &= mask;
            _whiteQueens &= mask;
            _whiteKing &= mask;

            _blackPawns &= mask;
            _blackRooks &= mask;
            _blackKnights &= mask;
            _blackBishops &= mask;
            _blackQueens &= mask;
            _blackKing &= mask;
        }

        public void SetPiece(Files f, int rank, PieceTypes type, Colors color)
        {
            int position = GetPositionFromFileAndRank(f, rank);
            SetPiece(position, type, color);
        }

        public void SetPiece(int position, PieceTypes type, Colors color)
        {
            for (int i = 0; i < _squares.Length; i++)
            {
                _squares[i] = null;
            }
            _threatenedSquares = null;
            _whiteUnsafe = null;
            _blackUnsafe = null;
            ClearPiece(position); // Clear the piece first.
            switch (color)
            {
                case Colors.White:
                    switch (type)
                    {
                        case PieceTypes.Pawn:
                            _whitePawns |= (U1 << position);
                            return;
                        case PieceTypes.Rook:
                            _whiteRooks |= (U1 << position);
                            return;
                        case PieceTypes.Knight:
                            _whiteKnights |= (U1 << position);
                            return;
                        case PieceTypes.Bishop:
                            _whiteBishops |= (U1 << position);
                            return;
                        case PieceTypes.Queen:
                            _whiteQueens |= (U1 << position);
                            return;
                        case PieceTypes.King:
                            _whiteKing |= (U1 << position);
                            return;
                    }
                    break;
                case Colors.Black:
                    switch (type)
                    {
                        case PieceTypes.Pawn:
                            _blackPawns |= (U1 << position);
                            return;
                        case PieceTypes.Rook:
                            _blackRooks |= (U1 << position);
                            return;
                        case PieceTypes.Knight:
                            _blackKnights |= (U1 << position);
                            return;
                        case PieceTypes.Bishop:
                            _blackBishops |= (U1 << position);
                            return;
                        case PieceTypes.Queen:
                            _blackQueens |= (U1 << position);
                            return;
                        case PieceTypes.King:
                            _blackKing |= (U1 << position);
                            return;
                    }
                    break;
            }

        }

    }
}
