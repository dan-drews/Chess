using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLibrary
{
    public static class Extensions
    {
        public static string GetNotation(this PieceTypes p)
        {
            switch (p)
            {
                case PieceTypes.Bishop:
                    return "B";
                case PieceTypes.King:
                    return "K";
                case PieceTypes.Knight:
                    return "N";
                case PieceTypes.Pawn:
                    return "P";
                case PieceTypes.Queen:
                    return "Q";
                case PieceTypes.Rook:
                    return "R";
            }
            return "";
        }

        public static int NumberOfTrailingZeros(this ulong n)
        {
            ulong mask = 1;
            for (int i = 0; i < 64; i++, mask <<= 1)
                if ((n & mask) != 0)
                    return i;

            return 64;
        }

        //private static ulong Bit(this ulong x, int n)
        //{
        //    return (x & ((ulong)1 << n)) >> n;
        //}

        //public static ulong ReverseBits(this ulong x)
        //{
        //    ulong result = 0;
        //    for (int i = 0; i < 64; i++)
        //        result = result | (x.Bit(64 - i) << i);
        //    return result;
        //}

        public static ulong ReverseBits(this ulong i)
        {
            i = (i & 0x5555555555555555) << 1 | (i >> 1) & 0x5555555555555555L;
            i = (i & 0x3333333333333333L) << 2 | (i >> 2) & 0x3333333333333333L;
            i = (i & 0x0f0f0f0f0f0f0f0fL) << 4 | (i >> 4) & 0x0f0f0f0f0f0f0f0fL;
            i = (i & 0x00ff00ff00ff00ffL) << 8 | (i >> 8) & 0x00ff00ff00ff00ffL;

            i = (i << 48) | ((i & 0xffff0000L) << 16) | ((i >> 16) & 0xffff0000L) | (i >> 48);
            return i;
        }

        //public static ulong ReverseBits(this ulong n)
        //{
        //    int counter = 64;
        //    ulong result = 0;
        //    while (counter > 0)
        //    {
        //        result |= n & 1;
        //        n >>= 1;
        //        result <<= 1;
        //        counter -= 1;
        //    }
        //    return result;
        //}

    }
}
