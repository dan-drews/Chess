using System;
using System.Collections.Generic;
using System.Linq;
using ChessLibrary;

namespace Chess.Perft
{
    class Program
    {
        const string path = @"C:\Users\dandr\Documents\peft.txt";

        static void Main(string[] args)
        {
            for (int depth = 0; depth <= 8; depth++)
            {
                var g = new Game(ChessLibrary.Enums.BoardType.BitBoard);
                try
                {
                    g.ResetGame();
                    var result = new PerftResult();
                    long count = GetData(g, depth);
                    Console.WriteLine($"Depth: {depth}, Count; {count}");
                    System.IO.File.AppendAllLines(
                        path,
                        new string[] { $"Depth: {depth}, Count; {count}" }
                    );
                }
                catch (Exception ex)
                {
                    //var moves = g.Moves.Select(x => $"{x.StartingSquare.File} {x.StartingSquare.Rank} {x.DestinationSquare.File} {x.DestinationSquare.Rank}\r\n").ToList();
                    //moves.Add($"Exception: {ex}");
                    //System.IO.File.AppendAllLines(path, moves);
                }
            }
            Console.ReadLine();
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
                g.AddMove(m);
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
