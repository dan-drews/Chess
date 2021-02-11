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
            RenderBoard(g);
            Console.ReadLine();
            g.AddMove(new Move(new Piece()
            {
                Color = Colors.White,
                Type = PieceTypes.Pawn
            }, Colors.White, new Square() { File = Files.E, Rank = 2 }, new Square() { File = Files.E, Rank = 4 }));
            RenderBoard(g);
            Console.ReadLine();
        }

        static void RenderBoard(Game g)
        {
            Console.Clear();
            for (int j = 8; j >= 1; j--)
            {
                for (int l = 0; l < 6; l++)
                {
                    for (Files i = Files.A; i <= Files.H; i++)
                    {
                        for (int k = 0; k < 8; k++)
                        {
                            var square = g.Board.GetSquare(i, j);
                            if (k > 1 && k < 6 && l > 0 && l < 5)
                            {
                                if (k == 2 || k == 5 || l == 1 || l == 4)
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
        }
    }
}
