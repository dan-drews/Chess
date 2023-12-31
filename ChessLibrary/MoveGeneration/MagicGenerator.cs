using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary.MoveGeneration
{
    internal static class MagicGenerator
    {
        public static Magic GenerateMagicForSquare(List<(ulong blockers, ulong moves)> values)
        {
            Random random = new Random();
            while (true)
            {
                ulong magicNumber;

                magicNumber = GetRandomUlong(random);
                magicNumber &= GetRandomUlong(random);
                magicNumber &= GetRandomUlong(random);
                magicNumber &= GetRandomUlong(random);

                var magic = new Magic() { MagicNumber = magicNumber, Shift = 12 };
                if (TestMagic(values, magic))
                {
                    return magic;
                }
            }
        }

        private static bool TestMagic(List<(ulong blockers, ulong moves)> values, Magic magic)
        {
            var table = new ulong?[(int)Math.Pow(2, magic.Shift)];
            foreach (var value in values)
            {
                var key = (value.blockers * magic.MagicNumber) >> (64 - magic.Shift);
                if (table[key] == null)
                {
                    table[key] = value.moves;
                }
                else if (table[key] != value.moves)
                {
                    return false;
                }
            }
            return true;
        }

        private static ulong GetRandomUlong(Random random)
        {
            byte[] bytes = new byte[8];
            random.NextBytes(bytes);
            return BitConverter.ToUInt64(bytes, 0);
        }
    }

    public class Magic
    {
        public int Shift { get; set; }
        public ulong MagicNumber { get; set; }
    }
}
