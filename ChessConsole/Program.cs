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
            while (!g.IsGameOver) 
            {
                var move = Console.ReadLine();
                if(move != null)
                {
                    var (start, end, _) = move.Split(' ');
                    var startingSquare = ParseSquare(g.Board, start);
                    var endSquare = ParseSquare(g.Board, end);

                    g.AddMove(new Move(startingSquare.Piece, g.PlayerToMove, startingSquare.Square, endSquare.Square)
                    {
                        CapturedPiece = endSquare.Piece
                    });
                }
                RenderBoard(g);
            }
            Console.WriteLine("GAME OVER");
        }

        static SquareState ParseSquare(Board b, string sq)
        {
            char fileStr = sq.ToUpper().ToCharArray()[0];
            Files f;
            switch (fileStr)
            {
                case 'A':
                    f = Files.A;
                    break;
                case 'B':
                    f = Files.B;
                    break;
                case 'C':
                    f = Files.C;
                    break;
                case 'D':
                    f = Files.D;
                    break;
                case 'E':
                    f = Files.E;
                    break;
                case 'F':
                    f = Files.F;
                    break;
                case 'G':
                    f = Files.G;
                    break;
                case 'H':
                    f = Files.H;
                    break;
                default:
                    throw new Exception("Invalid File");
            }
            char rankStr = sq.ToCharArray()[1];
            int rank = int.Parse(rankStr.ToString());
            if(rank < 0 || rank > 8)
            {
                throw new Exception("Invalid Rank");
            }

            return b.GetSquare(f, rank);

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
                                    if (square.Piece.Color == Colors.Black)
                                    {
                                        Console.BackgroundColor = ConsoleColor.White;
                                        Console.ForegroundColor = ConsoleColor.Black;
                                    }
                                    else
                                    {
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    Console.Write(square.Piece.Type.GetNotation());
                                    if (square.Square.Color == Colors.Black)
                                    {
                                        Console.BackgroundColor = ConsoleColor.Black;
                                        Console.ForegroundColor = ConsoleColor.White;
                                    }
                                    else
                                    {
                                        Console.BackgroundColor = ConsoleColor.White;
                                        Console.ForegroundColor = ConsoleColor.Black;
                                    }
                                }
                                else
                                {
                                    Console.Write(" ");
                                }
                            }
                            else
                            {
                                if (square.Square.Color == Colors.Black)
                                {
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.ForegroundColor = ConsoleColor.White;
                                }
                                else
                                {
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.ForegroundColor = ConsoleColor.Black;
                                }
                                Console.Write(" ");
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
