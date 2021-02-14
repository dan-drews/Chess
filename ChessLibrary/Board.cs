using System;
using System.Collections.Generic;
using System.Text;

namespace ChessLibrary
{
    public class Board : ICloneable
    {
        private SquareState[,] Squares { get; }

        public Board(bool setup = true)
        {
            Squares = new SquareState[8, 8];

            if (setup)
            {
                for (Files i = Files.A; i <= Files.H; i++)
                {
                    // Counting from 1 to 8 rather than 0 to 7 here to match the board labels
                    for (int j = 1; j <= 8; j++)
                    {
                        Squares[(int)i - 1, j - 1] = new SquareState(new Square()
                        {
                            File = i,
                            Rank = j
                        });
                    }
                }
            }
        }

        public SquareState GetSquare(Files file, int rank)
        {
            return Squares[(int)file - 1, rank - 1];
        }

        public object Clone()
        {
            var newBoard = new Board(false);
            for (Files file = Files.A; file <= Files.H; file++)
            {
                // Counting from 1 to 8 rather than 0 to 7 here to match the board labels
                for (int rank = 1; rank <= 8; rank++)
                {
                    newBoard.Squares[(int)file - 1, rank - 1] = (SquareState)GetSquare(file, rank).Clone();
                }
            }
            return newBoard;
        }
    }
}
