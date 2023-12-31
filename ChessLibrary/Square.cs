using System;

namespace ChessLibrary
{
    public class Square
    {
        public int SquareNumber { get; private set; }

        public Square() { }

        public Square(int squareNumber)
        {
            SquareNumber = squareNumber;
        }

        public int Rank
        {
            get { return (SquareNumber / 8) + 1; }
            set { SquareNumber = GetSquareNumber(value, File); }
        }
        public Files File
        {
            get { return (Files)(8 - (SquareNumber % 8)); }
            set { SquareNumber = GetSquareNumber(Rank, value); }
        }

        private static int GetSquareNumber(int rank, Files file)
        {
            return (ushort)(((rank - 1) * 8) + (8 - (int)file));
        }

        public Colors Color
        {
            get
            {
                // Even squares when adding rank + file are black
                if (((int)File + Rank) % 2 == 0)
                {
                    return Colors.Black;
                }
                return Colors.White;
            }
        }
    }
}
