using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public static class BitBoardConstants
    {
        public static ulong[] FileMasks = new ulong[]
        {
            0x8080808080808080, 0x4040404040404040, 0x2020202020202020, 0x1010101010101010,
            0x0808080808080808, 0x0404040404040404, 0x0202020202020202, 0x0101010101010101
        };

        public static ulong[] RankMasks = new ulong[]
        {
            0x00000000000000FF, 0x000000000000FF00, 0x0000000000FF0000, 0x00000000FF000000,
            0x000000FF00000000, 0x0000FF0000000000, 0x00FF000000000000, 0xFF00000000000000
        };

        public static ulong[] DiagonalMasks = new ulong[]
        {
            // A8                        // A7, B8                          // A6, B7, C8                       // A5, B6, C7, D8               // A4, B5, C6, D7, E8
            0x8000000000000000L,         0x4080000000000000L,               0x2040800000000000L,                0x1020408000000000L,            0x810204080000000L, 
            // A3, B4, C5, D6, E7, F8    // A2, B3, C4, D5, E6, F7, G8      // A1, B2, C3, D4, E5, F6, G7, H8   // B1, C2, D3, E4, F5, G6, H7   // C1, D2, E3, F4, G5, H6
            0x408102040800000L,          0x204081020408000L,                0x102040810204080L,                 0x1020408102040L,               0x10204081020L,
            // D1, E2, F3, G4, H5        // E1, F2, G3, H4                  // F1, G2, H3                       // G1, H2                       // H1
            0x102040810L,                0x1020408L,                        0x10204L,                           0x102L,                         0x1L
        };

        public static ulong[] AntiDiagonalMasks = new ulong[]
        {
            // H8                        // G8, H7                          // F8, G7, H6                       // E8, F7, G6, H5               // D8, E7, F6, G5, H4
            0x100000000000000L,          0x201000000000000L,                0x402010000000000L,                 0x804020100000000L,             0x1008040201000000L,
            // C8, D7, E6, F5, G4, H3    // B8, C7, D6, E5, F4, G3, H2      // A8, B7, C6, D5, E4, F3, G2, H1   // A7, B6, C5, D4, E3, F2, G1   // A6, B5, C4, D3, E2, F1
            0x2010080402010000L,         0x4020100804020100L,               0x8040201008040201L,                0x80402010080402L,              0x804020100804L,
            // A5, B4, C3, D2, E1        // A4, B3, C2, D1                  // A3, B2, C1                       // A2, B1                       // A1 
            0x8040201008L,               0x80402010L,                       0x804020L,                          0x8040L,                        0x80L
        };

        public static ulong GetDiagonalMask(Square square)
        {
            return DiagonalMasks[((int)square.File - 1) + (8 - square.Rank)];
        }

        public static ulong GetAntiDiagonalMask(Square square)
        {
            return AntiDiagonalMasks[14 - (square.Rank - 1 + (int)square.File - 1)];
        }

        public static ulong Edges = Rank1 | Rank8 | FileA | FileH;

        public const ulong Rank8 = 0xFF00000000000000;
        public const ulong Rank1 = 0xFF;
        public const ulong Rank4 = 0x00000000FF000000;
        public const ulong Rank5 = 0x000000FF00000000;

        public const ulong FileA = 0x8080808080808080;
        public const ulong FileB = 0x4040404040404040;
        public const ulong FileH = 0x0101010101010101;
        public const ulong FileG = 0x0202020202020202;

        public const int KnightRangeBaseSquare = 18;
        public const ulong KnightSpan = 0x0000000A1100110A;

        public const int KingRangeBaseSquare = 9;
        public const ulong KingSpan = 0x0000000000070507;

        public const ulong U1 = (ulong)1;


        public const ulong Starting_White_Pawns = 0xFF00;
        public const ulong Starting_White_Rooks = 0x81;
        public const ulong Starting_White_Knights = 0x42;
        public const ulong Starting_White_Bishops = 0x24;
        public const ulong Starting_White_Queens = 0x10;
        public const ulong Starting_White_King = 0x8;

        public const ulong Starting_Black_Pawns = 0xFF000000000000;
        public const ulong Starting_Black_Rooks = 0x8100000000000000;
        public const ulong Starting_Black_Knights = 0x4200000000000000;
        public const ulong Starting_Black_Bishops = 0x2400000000000000;
        public const ulong Starting_Black_Queens = 0x1000000000000000;
        public const ulong Starting_Black_King = 0x800000000000000;
    }
}
