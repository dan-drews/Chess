using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace ChessLibrary
{
    public struct Move : ICloneable
    {
        public readonly struct Flag
        {
            public const int None = 0;
            public const int EnPassantCapture = 1;
            public const int LongCastle = 2;
            public const int ShortCastle = 3;
            public const int PromoteToQueen = 4;
            public const int PromoteToRook = 5;
            public const int PromoteToBishop = 6;
            public const int PromoteToKnight = 7;
            public const int PawnTwoForward = 8;
        }

        public static Move NullMove = new Move(0);

        private uint _moveValue;

        public uint MoveValue { get { return _moveValue; }  set { _moveValue = value; } }

        const uint COLOR_MASK =
            0b00000000010000000000000000000000;
        const uint FLAG_MASK =
            0b00000000001111000000000000000000;
        const uint START_SQUARE_MASK =
            0b00000000000000111111000000000000;
        const uint DEST_SQUARE_MASK =
            0b00000000000000000000111111000000;
        const uint PIECE_MASK =
            0b00000000000000000000000000111000;
        const uint CAPTURE_PIECE_MASK =
            0b00000000000000000000000000000111;

        const int COLOR_SHIFT = 22;
        const int FLAG_SHIFT = 18;
        const int START_SHIFT = 12;
        const int DEST_SHIFT = 6;
        const int PIECE_SHIFT = 3;
        const int CAPTURE_SHIFT = 0;

        [JsonIgnore]
        public ulong Hash { get; set; }

        public override int GetHashCode()
        {
            return (int)_moveValue;
        }

        public Move(uint moveValue)
        {
            _moveValue = moveValue;
        }

        public override bool Equals(object? obj)
        {
            if (obj == null)
            {
                return false;
            }
            return SameMove((Move)obj, this);
        }

        public Move(
            int startSquare,
            int destinationSquare,
            Colors color,
            PieceTypes piece,
            PieceTypes? capturedPiece = null,
            int flags = 0
        )
        {
            _moveValue = 0;
            _moveValue |= (uint)color << COLOR_SHIFT;
            _moveValue |= (uint)flags << FLAG_SHIFT;
            _moveValue |= (uint)startSquare << START_SHIFT;
            _moveValue |= (uint)destinationSquare << DEST_SHIFT;
            _moveValue |= (uint)piece << PIECE_SHIFT;
            _moveValue |= (uint)(capturedPiece ?? 0) << CAPTURE_SHIFT;
        }

        [JsonIgnore]
        public int TargetSquare => (int)((_moveValue & DEST_SQUARE_MASK) >> DEST_SHIFT);
        [JsonIgnore]
        public int StartingSquare => (int)((_moveValue & START_SQUARE_MASK) >> START_SHIFT);
        [JsonIgnore]
        public int Flags => (int)((_moveValue & FLAG_MASK) >> FLAG_SHIFT);
        [JsonIgnore]
        public Colors Color => (Colors)((_moveValue & COLOR_MASK) >> COLOR_SHIFT);
        [JsonIgnore]
        public PieceTypes Piece => (PieceTypes)((_moveValue & PIECE_MASK) >> PIECE_SHIFT);
        [JsonIgnore]
        public PieceTypes? CapturedPiece =>
            (_moveValue & CAPTURE_PIECE_MASK) == 0
                ? null
                : (PieceTypes)(_moveValue & CAPTURE_PIECE_MASK);

        public static bool SameMove(Move a, Move b)
        {
            return a._moveValue == b._moveValue;
        }

        public object Clone()
        {
            return new Move(_moveValue)
            {
                Hash = Hash,
            };
        }

        public static bool operator ==(Move lhs, Move rhs) => lhs._moveValue == rhs._moveValue;
        public static bool operator !=(Move lhs, Move rhs) => lhs._moveValue != rhs._moveValue;

        [JsonIgnore]
        public PieceTypes? PromotedType
        {
            get
            {
                switch (Flags)
                {
                    case Flag.PromoteToQueen:
                        return PieceTypes.Queen;
                    case Flag.PromoteToKnight:
                        return PieceTypes.Knight;
                    case Flag.PromoteToBishop:
                        return PieceTypes.Bishop;
                    case Flag.PromoteToRook:
                        return PieceTypes.Rook;
                }
                return null;
            }
        }
    }
}
