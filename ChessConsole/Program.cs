using ChessLibrary;
using System;
using System.Diagnostics;
using System.Linq;

namespace ChessConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var g = new Game(ChessLibrary.Enums.BoardType.BitBoard);
            var whiteConfig = new ScorerConfiguration()
            {

            };
            var whiteEngine = new Engine(whiteConfig);

            var blackConfig = new ScorerConfiguration()
            {

            };
            var blackEngine = new Engine(blackConfig);
            g.ResetGame();
            RenderBoard(g);

            while (!g.IsGameOver)
            {
                try
                {
                    var move = Console.ReadLine();
                    if (move != null)
                    {
                        if (move.ToLower() == "analyze")
                        {
                            var engine = g.PlayerToMove == Colors.White ? whiteEngine : blackEngine;
                            var bestMove = engine.GetBestMove(g, g.PlayerToMove).node.Move;
                            var promotion = string.Empty;
                            if(bestMove.PromotedPiece != null)
                            {
                                promotion = $" - Promoted to {bestMove.PromotedPiece.Type}";
                            }
                            Console.WriteLine($"{bestMove.StartingSquare.File}{bestMove.StartingSquare.Rank} {bestMove.DestinationSquare.File}{bestMove.DestinationSquare.Rank}{promotion}");
                        }
                        else
                        {
                            var (start, end, promotion, _) = move.Split(' ');
                            var startingSquare = ParseSquare(g.Board, start);
                            var endSquare = ParseSquare(g.Board, end);
                            var m = new Move(startingSquare.Piece, g.PlayerToMove, startingSquare.Square, endSquare.Square)
                            {
                                CapturedPiece = endSquare.Piece
                            };

                            if (startingSquare.Piece.Type == PieceTypes.Pawn && (endSquare.Square.Rank == 1 || endSquare.Square.Rank == 8))
                            {
                                // Promotion. Parse that.
                                switch (promotion.ToLower())
                                {
                                    case "q":
                                        m.PromotedPiece = new Piece() { Color = startingSquare.Piece.Color, Type = PieceTypes.Queen };
                                        break;
                                    case "b":
                                        m.PromotedPiece = new Piece() { Color = startingSquare.Piece.Color, Type = PieceTypes.Bishop };
                                        break;
                                    case "r":
                                        m.PromotedPiece = new Piece() { Color = startingSquare.Piece.Color, Type = PieceTypes.Rook };
                                        break;
                                    case "k":
                                    case "n":
                                        m.PromotedPiece = new Piece() { Color = startingSquare.Piece.Color, Type = PieceTypes.Knight };
                                        break;
                                }
                            }
                            g.AddMove(m);

                            RenderBoard(g);
                        }
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("There was a problem. Try again.");
                }
            }
            Console.WriteLine("GAME OVER");
        }

        static SquareState ParseSquare(IBoard b, string sq)
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
            if (rank < 0 || rank > 8)
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
