using ChessLibrary;
using System;
using System.Linq;

namespace ChessConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var g = new Game();
            g.ResetGame();
            for (int j = 8; j >= 1; j--)
            {
                for (int l = 0; l < 8; l++)
                {
                    for (Files i = Files.A; i <= Files.H; i++)
                    {
                        for (int k = 0; k < 12; k++)
                        {
                            var square = g.Board.GetSquare(i, j);
                            if (k > 3 && k < 9 && l > 1 && l < 6)
                            {
                                if(k == 4 || k == 8 || l == 2 || l == 5)
                                {
                                    Console.Write(" ");
                                }
                                else if (square.Piece != null)
                                {
                                    Console.Write(square.Piece.Type.GetNotation());
                                }
                                else
                                {
                                    Console.Write(" ");
                                }
                            }
                            else
                            {
                                Console.Write(square.Square.Color.ToString().First());
                            }
                        }
                        Console.Write(" ");
                    }
                    Console.WriteLine();
                }
                Console.WriteLine();
            }
            Console.ReadLine();
        }
    }
}
