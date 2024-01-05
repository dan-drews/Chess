using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ChessLibrary;

namespace Chess.Perft
{
    class Program
    {
        const string path = @"C:\Users\dandr\Documents\peft.txt";

        static void Main(string[] args)
        {
            ChessLibrary.MoveGeneration.MagicSlidingImplementation.InitializeMagicSliders();
            var stopwatch = new Stopwatch();
            while (true)
            {                
                Console.WriteLine("Provide Depth: ");
                var command = Console.ReadLine();
                int depth = int.Parse(command);
                var g = new Game(ChessLibrary.Enums.BoardType.BitBoard, false);
                g.ResetGame();
                Console.WriteLine($"Running PERFT @ depth {depth}");
                stopwatch.Start();
                var numberOfNodes = ChessLibrary.Perft.ExecutePerft(g, depth, true);
                stopwatch.Stop();
                Console.WriteLine($"Searched {numberOfNodes:n0} nodes ({stopwatch.ElapsedMilliseconds} ms).");
                stopwatch.Reset();

            }
        }

        static long GetData(Game g, int depth)
        {
            if (depth == 0)
            {
                return 1;
            }

            var moves = g.GetAllLegalMoves();

            long result = 0;
            foreach (var m in moves)
            {
                //if (depth > 1)
                //{
                //    // Increment Stuff.
                //    result.Nodes += 1;
                //    if (g.Board.GetSquare(m.DestinationSquare.File, m.DestinationSquare.Rank).Piece != null)
                //    {
                //        result.Captures += 1;
                //    }
                //    if (m.Piece.Type == PieceTypes.Pawn && m.StartingSquare.File != m.DestinationSquare.File && g.Board.GetSquare(m.DestinationSquare.File, m.DestinationSquare.Rank).Piece == null)
                //    {
                //        result.EnPassant += 1;
                //    }
                //    if (m.Piece.Type == PieceTypes.King && m.StartingSquare.File == Files.E && (m.DestinationSquare.File == Files.G || m.DestinationSquare.File == Files.C))
                //    {
                //        result.Castles += 1;
                //    }
                //    if (m.Piece.Type == PieceTypes.Pawn && m.PromotedPiece != null)
                //    {
                //        result.Promotions += 1;
                //    }
                //    g.AddMove(m);
                //    if (g.Evaluator.IsKingInCheck(g.Board, g.PlayerToMove, g.Moves))
                //    {
                //        result.Checks += 1;
                //    }
                //    if (g.IsCheckmate)
                //    {
                //        result.Checkmates += 1;
                //    }
                //    g.UndoLastMove();
                //}
                //else
                //{
                g.AddMove(m, false);
                result += GetData(g, depth - 1);
                g.UndoLastMove();
                //}
            }
            return result;
        }

        public class PerftResult
        {
            public int Depth { get; set; }
            public long Nodes { get; set; }
            public int Captures { get; set; }
            public int EnPassant { get; set; }
            public int Castles { get; set; }
            public int Promotions { get; set; }
            public int Checks { get; set; }
            public int Checkmates { get; set; }
        }
    }
}
